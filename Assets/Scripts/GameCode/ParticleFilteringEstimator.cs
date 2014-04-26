using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/*
 * Represents all GuardAgents knowledge
 */
using System.Linq;


public class ParticleFilteringEstimator {
	/* Particle */
	public List<Particle> Particles { get; set; }

	/* Probability of PlayerAgent being at a GameMap xy location (noise caused by RatAgents) */
	public static int[,] FloorCellProbabilities { get; set; }

	/* Contains GameMap positions that contain floor sensors (hash value of floor cell)   */
	List<ulong> FloorSensors;

	/* Contains GameMap positions that a respective particle have covered (key: hash of particle number, locationX, and locationY)  */
	List<ulong> FloorcellPartcileCover;

	public ulong ParticleUniqueIdCount { get; set; }


	public List<Particle> particlesToRemove { get; set; }
	public List<Particle> particlesToAdd { get; set; }

	public int NumberOfParticlesSetOff { get; set; }

	/*
	 * Construtor
	 */
	public ParticleFilteringEstimator () {
		Particles = new List<Particle>();
		FloorCellProbabilities = new int[GameConstants.MapWidthPixels, GameConstants.MapHeightPixels];
		FloorSensors = new List<ulong>();
		FloorcellPartcileCover = new List<ulong>();
		particlesToRemove = new List<Particle> ();
		particlesToAdd = new List<Particle> ();
	}

	// TODO: fix x,y location cooridnates - make sure they are indexed right

	/*
	 * Updates Agents knowledge of the environment
	 */	
	public void UpdateEstimator() {
		// increment particle locations
		for (int i = 0; i<Particles.Count; i++) {
			int newFloorcellValue = (FloorCellProbabilities [Particles[i].CurrentLocationX, Particles[i].CurrentLocationY] + Particles[i].ParticleValue);
			FloorCellProbabilities[Particles[i].CurrentLocationX, Particles[i].CurrentLocationY] = newFloorcellValue;
		}

		// decrement particle locations
		for (int x = 0; x<GameConstants.MapWidthPixels; x++) {
			for (int y = 0; y<GameConstants.MapHeightPixels; y++) {
				if (FloorCellProbabilities[x,y] > 0) {
					FloorCellProbabilities[x,y] = (FloorCellProbabilities[x,y] - 1);
				}
			}
		}

		// create and delete particles
		ulong pCount = 0;
		for (int i = 0; i<Particles.Count; i++) {
			// get particle
			Particle p = Particles[i];

			// decrement particle life span
			Particles[i].TimeToLive -= 1;

			// check if particle was created during current game step
			/*bool justCreatedChanged = false;
			if (p.JustCreated) {
				p.JustCreated = false;
				justCreatedChanged = true;
			}*/

			// check if particle needs to die
			if (p.TimeToLive < 1) {
				particlesToRemove.Add(Particles[i]);
			}

			// update particle locations and avoid adjacent duplicates
			else {


				ulong hashUp = ParticleCoverHash(p, p.CurrentLocationX, (p.CurrentLocationY+1));
				ulong hashDown = ParticleCoverHash(p, p.CurrentLocationX, (p.CurrentLocationY-1));
				ulong hashLeft = ParticleCoverHash(p, (p.CurrentLocationX-1), p.CurrentLocationY);
				ulong hashRight = ParticleCoverHash(p, (p.CurrentLocationX+1), p.CurrentLocationY);

				bool canMoveUpOnMap = GameMap.GameMapArray[p.CurrentLocationX,(p.CurrentLocationY+1)] == GameConstants.MapHallwayFloorcell
					|| GameMap.GameMapArray[p.CurrentLocationX,(p.CurrentLocationY+1)] == GameConstants.MapTurningFloorcell;
				
				bool canMoveDownOnMap = GameMap.GameMapArray[p.CurrentLocationX,(p.CurrentLocationY-1)] == GameConstants.MapHallwayFloorcell
					|| GameMap.GameMapArray[p.CurrentLocationX,(p.CurrentLocationY-1)] == GameConstants.MapTurningFloorcell;
				
				bool canMoveLeftOnMap = GameMap.GameMapArray[(p.CurrentLocationX-1),p.CurrentLocationY] == GameConstants.MapHallwayFloorcell
					|| GameMap.GameMapArray[(p.CurrentLocationX-1),p.CurrentLocationY] == GameConstants.MapTurningFloorcell;
				
				bool canMoveRightOnMap = GameMap.GameMapArray[(p.CurrentLocationX+1),p.CurrentLocationY] == GameConstants.MapHallwayFloorcell
					|| GameMap.GameMapArray[(p.CurrentLocationX+1),p.CurrentLocationY] == GameConstants.MapTurningFloorcell;


				if (canMoveUpOnMap && !FloorcellPartcileCover.Contains(hashUp)) {
					Particle newParticleAgain = new Particle (GameConstants.Up, p.CurrentLocationX, (p.CurrentLocationY+1), p.ParticleValue, p.TimeToLive, false, p.UniqueId);
					particlesToAdd.Add (newParticleAgain);
					AddParticleToCoverHash(newParticleAgain,  newParticleAgain.CurrentLocationX, newParticleAgain.CurrentLocationY);
				}

				if (canMoveDownOnMap && !FloorcellPartcileCover.Contains(hashDown)) {
					Particle newParticleAgain = new Particle (GameConstants.Up, p.CurrentLocationX, (p.CurrentLocationY-1), p.ParticleValue, p.TimeToLive, false, p.UniqueId);
					particlesToAdd.Add (newParticleAgain);
					AddParticleToCoverHash(newParticleAgain,  newParticleAgain.CurrentLocationX, newParticleAgain.CurrentLocationY);
				}

				if (canMoveLeftOnMap && !FloorcellPartcileCover.Contains(hashLeft)) {
					Particle newParticleAgain = new Particle (GameConstants.Up, (p.CurrentLocationX-1), (p.CurrentLocationY), p.ParticleValue, p.TimeToLive, false, p.UniqueId);
					particlesToAdd.Add (new Particle (GameConstants.Up, (p.CurrentLocationX-1), (p.CurrentLocationY), p.ParticleValue, p.TimeToLive, false, p.UniqueId));
					AddParticleToCoverHash(newParticleAgain,  newParticleAgain.CurrentLocationX, newParticleAgain.CurrentLocationY);
				}

				if (canMoveRightOnMap && !FloorcellPartcileCover.Contains(hashRight)) {
					Particle newParticleAgain = new Particle (GameConstants.Up, (p.CurrentLocationX+1), (p.CurrentLocationY), p.ParticleValue, p.TimeToLive, false, p.UniqueId);
					particlesToAdd.Add (newParticleAgain);
					AddParticleToCoverHash(newParticleAgain,  newParticleAgain.CurrentLocationX, newParticleAgain.CurrentLocationY);
				}
			}


			KillParticle(Particles[i]);
			Particles[i].ParticleRemoved = true;
			particlesToRemove.Add(Particles[i]);
		}

		for (int u = 0; u<particlesToRemove.Count; u++) {
			Particles.Remove(particlesToRemove[u]);
		}



		var item = Particles.SingleOrDefault(x => x.ParticleRemoved == true);
		if (item != null) {
			//Debug.Log("PARTICLE LIST STUFF REMOVED!");
			Particles.Remove (item);
		} else {
			//Debug.Log("PARTICLE LIST IS NULL!");
		}


		for (int u = 0; u<particlesToRemove.Count; u++) {
			Particles.Remove(particlesToRemove[u]);
		}


		for (int u = 0; u<particlesToAdd.Count; u++) {
			Particles.Add(particlesToAdd[u]);
			AddParticleToCoverHash (particlesToAdd[u], particlesToAdd[u].CurrentLocationX, particlesToAdd[u].CurrentLocationY);
		}

		Debug.Log ("Particle Count: "+ Particles.Count);
		Debug.Log ("Particles Set Off: "+ NumberOfParticlesSetOff);

		//particlesToRemove.Clear ();
		particlesToAdd.Clear ();
		particlesToRemove.Clear ();
	}

	/*
	 * Adds a floor sensor to GameMap
	 * 
	 * locationX - x coordinate of where to place floor sensor
	 * locationY - y coordinate of where to place floor floor sensor
	 * 
	 * Returns true if floor sensor did not exist prior otherwise returns true
	 */	
	public bool AddFloorSensor(int locationX, int locationY){
		ulong hash = GameMap.HashFloorCell (locationX, locationY);
		if (!FloorSensors.Contains(hash)) {
			FloorSensors.Add(hash);
			return true;
		}

		return false;
	}

	/*
	 * Determines where PlayerAgent is most likely to be within range of Guard's limited sight.
	 * 
	 * locationX - x coordinate of where to check for floor sensor
	 * locationY - y coordinate of where to check for floor sensor
	 */	
	public bool IsFloorSensorAtLocation(int locationX, int locationY){
		ulong hash = GameMap.HashFloorCell (locationX, locationY);
		if (FloorSensors.Contains(hash)) {
			return true;
		}

		return false;
	}

	/*
	 * Determines where PlayerAgent is most likely to be within range of Guard's limited sight.
	 * 
	 * agentId - Uniquely identifies Agent type with constants from GameConstants
	 * locationX - x coordinate of where to create particle
	 * locationY - y coordinate of where to create particle
	 */	
	public void CreateParticle(int agentId, int locationX, int locationY) {
		int value = 0;
		if (agentId == GameConstants.PlayerAgentId) {
			value = GameConstants.PlayerAgentParticleValue;
		} else if (agentId == GameConstants.RatAgentId) {
			value = GameConstants.RatAgentParticleValue;
		}

		// add up, down, left, right particle directions
		Particle newParticle = new Particle (GameConstants.Up, locationX, locationY, value, GameConstants.ParticleLifeSpan, true, ++ParticleUniqueIdCount);
		Particles.Add (newParticle);
		++NumberOfParticlesSetOff;


		AddParticleToCoverHash (newParticle, newParticle.CurrentLocationX, newParticle.CurrentLocationY);
		//Particles.Add (new Particle (GameConstants.Down, locationX, locationY, value, GameConstants.ParticleLifeSpan, true));
		//Particles.Add (new Particle (GameConstants.Left, locationX, locationY, value, GameConstants.ParticleLifeSpan, true));
		//Particles.Add (new Particle (GameConstants.Right, locationX, locationY, value, GameConstants.ParticleLifeSpan, true));	
	
		//int newFloorcellValue = (FloorCellProbabilities [locationX, locationY] + value);
		//FloorCellProbabilities[locationX, locationY] = newFloorcellValue;
	}

	/*
	 * Determines where PlayerAgent is most likely to be within range of Guard's limited sight.
	 * 
	 * Returns xy location value in int[0] = x, int[1] = y
	 */	
	public int[] GetGuardTargetLocation(GuardAgent guard) {
		int startX = guard.LocationX-(GameConstants.GuardSearchDistancePixels/2);
		int startY = guard.LocationY-(GameConstants.GuardSearchDistancePixels/2);
		int endX = guard.LocationX+(GameConstants.GuardSearchDistancePixels/2);
		int endY = guard.LocationY+(GameConstants.GuardSearchDistancePixels/2);

		int highestParticleValue = -1;
		int xToSave = -1;
		int yToSave = -1;

		for (int x = startX; x<endX; x++) {
			for (int y = startY; y<endY; y++) {
				if (x>=0 && x<GameConstants.MapWidthPixels && y>=0 && y<GameConstants.MapHeightPixels){
					if (FloorCellProbabilities[x,y] >= highestParticleValue) {
						highestParticleValue = FloorCellProbabilities[x,y];
						xToSave = x;
						yToSave = y;
					}
				}
			}			
		}

		int[] points = new int[2];
		points[0] = xToSave;
		points[1] = yToSave;
		return points;
	}


	/*
	 * Kills particle and removes hash cover value
	 */	
	public void KillParticle(Particle p){
		//FloorcellPartcileCover.Remove (ParticleCoverHash(p, p.CurrentLocationX, p.CurrentLocationY));
		//particlesToRemove.Add (p);
	}

	/*
	 * Adds particle to cover hash list
	 */	
	public void AddParticleToCoverHash(Particle particle, int locationX, int locationY){
		FloorcellPartcileCover.Add (ParticleCoverHash(particle, locationX, locationY));
    }

	/*
	 * Creates unique value for floor cell and particle combination
	 */	
	public ulong ParticleCoverHash(Particle particle, int locationX, int locationY) {
		return (ulong)(((ulong)locationX)*((ulong)123) + ((ulong)locationY)*((ulong)23413) + particle.UniqueId);
	}
}
