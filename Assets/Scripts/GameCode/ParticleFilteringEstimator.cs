using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/*
 * Represents all GuardAgents knowledge
 */
public class ParticleFilteringEstimator {
	/* Particle */
	public List<Particle> Particles { get; set; }

	/* Probability of PlayerAgent being at a GameMap xy location (noise caused by RatAgents) */
	public static short[,] FloorCellProbabilities { get; set; }

	/* Contains GameMap positions that contain floor sensors (hash value of floor cell)   */
	List<ulong> FloorSensors;

	/* Contains GameMap positions that a respective particle have covered (key: hash of particle number, locationX, and locationY)  */
	List<ulong> FloorcellPartcileCover;

	/*
	 * Construtor
	 */
	public ParticleFilteringEstimator () {
		Particles = new List<Particle>();
		FloorCellProbabilities = new short[GameConstants.MapWidthPixels, GameConstants.MapHeightPixels];
		FloorSensors = new List<ulong>();
		FloorcellPartcileCover = new List<ulong>();
	}

	// TODO: fix x,y location cooridnates - make sure they are indexed right

	/*
	 * Updates Agents knowledge of the environment
	 */	
	public void UpdateEstimator() {
		// decrement particle locations
		for (int x = 0; x<GameConstants.MapWidthPixels; x++) {
			for (int y = 0; y<GameConstants.MapHeightPixels; y++) {
				if (GameMap.GameMapArray[x,y] == 0 || GameMap.GameMapArray[x,y] == 0) {
					if (FloorCellProbabilities[x,y] > 0) {
						FloorCellProbabilities[x,y] = (short)(FloorCellProbabilities[x,y] - 1);
					}
				}	
			}
		}

		// create and delete particles
		for (int i = 0; i<Particles.Count; i++) {
			// get particle
			Particle p = Particles[i];

			// decrement particle life span
			p.TimeToLive = (short)(p.TimeToLive - 1);

			// check if particle was created during current game step
			bool justCreatedChanged = false;
			if (p.JustCreated) {
				p.JustCreated = false;
				justCreatedChanged = true;
			}

			// check if particle needs to die
			else if (p.TimeToLive < 1) {
				Particles.Remove(p);
			}

			// update particle locations and avoid adjacent duplicates
			else {
				ulong hashUp = ParticleCoverHash(p, p.CurrentLocationX, (short)(p.CurrentLocationY+1));
				ulong hashDown = ParticleCoverHash(p, p.CurrentLocationX, (short)(p.CurrentLocationY-1));
				ulong hashLeft = ParticleCoverHash(p, (short)(p.CurrentLocationX-1), p.CurrentLocationY);
				ulong hashRight = ParticleCoverHash(p, (short)(p.CurrentLocationX+1), p.CurrentLocationY);


				bool canMoveUpOnMap = GameMap.GameMapArray[p.CurrentLocationX,(short)(p.CurrentLocationY+1)] == GameConstants.MapHallwayFloorcell
					|| GameMap.GameMapArray[p.CurrentLocationX,(short)(p.CurrentLocationY+1)] == GameConstants.TurningFloorcell;

				bool canMoveDownOnMap = GameMap.GameMapArray[p.CurrentLocationX,(short)(p.CurrentLocationY-1)] == GameConstants.MapHallwayFloorcell
					|| GameMap.GameMapArray[p.CurrentLocationX,(short)(p.CurrentLocationY-1)] == GameConstants.TurningFloorcell;

				bool canMoveLeftOnMap = GameMap.GameMapArray[(short)(p.CurrentLocationX-1),p.CurrentLocationY] == GameConstants.MapHallwayFloorcell
					|| GameMap.GameMapArray[(short)(p.CurrentLocationX-1),p.CurrentLocationY] == GameConstants.TurningFloorcell;

				bool canMoveRightOnMap = GameMap.GameMapArray[(short)(p.CurrentLocationX+1),p.CurrentLocationY] == GameConstants.MapHallwayFloorcell
					|| GameMap.GameMapArray[(short)(p.CurrentLocationX+1),p.CurrentLocationY] == GameConstants.TurningFloorcell;


				// get value
				short value = p.ParticleValue;

				// move particles
				if (p.MovingDirection == GameConstants.Up) { // up direction

					if (canMoveUpOnMap && !FloorcellPartcileCover.Contains(hashUp)) {
						Particles.Add (new Particle (GameConstants.Up, p.CurrentLocationX, (short)(p.CurrentLocationY+1), value, GameConstants.ParticleLifeSpan, true));
					}
					if (canMoveLeftOnMap && !FloorcellPartcileCover.Contains(hashLeft)) {
						Particles.Add (new Particle (GameConstants.Left, (short)(p.CurrentLocationX-1), p.CurrentLocationY, value, GameConstants.ParticleLifeSpan, true));
					}
					if (canMoveRightOnMap && !FloorcellPartcileCover.Contains(hashRight)) {
						Particles.Add (new Particle (GameConstants.Right, (short)(p.CurrentLocationX+1), p.CurrentLocationY, value, GameConstants.ParticleLifeSpan, true));
					}

				} else if (Particles[i].MovingDirection == GameConstants.Down) { // down direction
					
					if (canMoveDownOnMap && FloorcellPartcileCover.Contains(hashDown)) {
						Particles.Add (new Particle (GameConstants.Down, p.CurrentLocationX, (short)(p.CurrentLocationY-1), value, GameConstants.ParticleLifeSpan, true));
					}
					if (canMoveLeftOnMap && !FloorcellPartcileCover.Contains(hashLeft)) {
						Particles.Add (new Particle (GameConstants.Left, (short)(p.CurrentLocationX-1), p.CurrentLocationY, value, GameConstants.ParticleLifeSpan, true));
					}
					if (canMoveRightOnMap && !FloorcellPartcileCover.Contains(hashRight)) {
						Particles.Add (new Particle (GameConstants.Right, (short)(p.CurrentLocationX+1), p.CurrentLocationY, value, GameConstants.ParticleLifeSpan, true));
					}

				} else if (Particles[i].MovingDirection == GameConstants.Left) { // left direction
					
					if (canMoveLeftOnMap && !FloorcellPartcileCover.Contains(hashLeft)) {
						Particles.Add (new Particle (GameConstants.Left, (short)(p.CurrentLocationX-1), p.CurrentLocationY, value, GameConstants.ParticleLifeSpan, true));
					}
					if (canMoveUpOnMap && !FloorcellPartcileCover.Contains(hashUp)) {
						Particles.Add (new Particle (GameConstants.Up, p.CurrentLocationX, (short)(p.CurrentLocationY+1), value, GameConstants.ParticleLifeSpan, true));
					}
					if (canMoveDownOnMap && FloorcellPartcileCover.Contains(hashDown)) {
						Particles.Add (new Particle (GameConstants.Down, p.CurrentLocationX, (short)(p.CurrentLocationY-1), value, GameConstants.ParticleLifeSpan, true));
					}

				} else if (Particles[i].MovingDirection == GameConstants.Right) { // right direction
					
					if (canMoveRightOnMap && !FloorcellPartcileCover.Contains(hashRight)) {
						Particles.Add (new Particle (GameConstants.Right, (short)(p.CurrentLocationX+1), p.CurrentLocationY, value, GameConstants.ParticleLifeSpan, true));
					}
					if (canMoveUpOnMap && !FloorcellPartcileCover.Contains(hashUp)) {
						Particles.Add (new Particle (GameConstants.Up, p.CurrentLocationX, (short)(p.CurrentLocationY+1), value, GameConstants.ParticleLifeSpan, true));
					}
					if (canMoveDownOnMap && FloorcellPartcileCover.Contains(hashDown)) {
						Particles.Add (new Particle (GameConstants.Down, p.CurrentLocationX, (short)(p.CurrentLocationY-1), value, GameConstants.ParticleLifeSpan, true));
					}

				}

				// delete current particle
				if (!justCreatedChanged) {
					KillParticle(p);
				}
			}
		}

		// increment particle locations
		for (int i = 0; i<Particles.Count; i++) {
			short newFloorcellValue = (short)(FloorCellProbabilities [Particles[i].CurrentLocationX, Particles[i].CurrentLocationY] + Particles[i].ParticleValue);
			FloorCellProbabilities[Particles[i].CurrentLocationX, Particles[i].CurrentLocationY] = newFloorcellValue;
		}
	}

	/*
	 * Adds a floor sensor to GameMap
	 * 
	 * locationX - x coordinate of where to place floor sensor
	 * locationY - y coordinate of where to place floor floor sensor
	 * 
	 * Returns true if floor sensor did not exist prior otherwise returns true
	 */	
	public bool AddFloorSensor(short locationX, short locationY){
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
	public bool IsFloorSensorAtLocation(short locationX, short locationY){
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
	public void CreateParticle(short agentId, short locationX, short locationY) {
		short value = 0;
		if (agentId == GameConstants.PlayerAgentId) {
			value = GameConstants.PlayerAgentParticleValue;
		} else if (agentId == GameConstants.RatAgentId) {
			value = GameConstants.RatAgentParticleValue;
		}

		// add up, down, left, right particle directions
		Particles.Add (new Particle (GameConstants.Up, locationX, locationY, value, GameConstants.ParticleLifeSpan, true));
		Particles.Add (new Particle (GameConstants.Down, locationX, locationY, value, GameConstants.ParticleLifeSpan, true));
		Particles.Add (new Particle (GameConstants.Left, locationX, locationY, value, GameConstants.ParticleLifeSpan, true));
		Particles.Add (new Particle (GameConstants.Right, locationX, locationY, value, GameConstants.ParticleLifeSpan, true));	
	
		short newFloorcellValue = (short)(FloorCellProbabilities [locationX, locationY] + value);
		FloorCellProbabilities[locationX, locationY] = newFloorcellValue;
	}

	/*
	 * Determines where PlayerAgent is most likely to be within range of Guard's limited sight.
	 * 
	 * Returns xy location value in short[0] = x, short[1] = y
	 */	
	public short[] GetGuardTargetLocation(GuardAgent guard) {
		int startX = guard.LocationX-(GameConstants.GuardSearchDistancePixels/2);
		int startY = guard.LocationY-(GameConstants.GuardSearchDistancePixels/2);;
		int endX = guard.LocationX+(GameConstants.GuardSearchDistancePixels/2);
		int endY = guard.LocationY+(GameConstants.GuardSearchDistancePixels/2);

		short highestParticleValue = -1;
		short xToSave = -1;
		short yToSave = -1;

		for (int x = startX; x<endX; x++) {
			for (int y = startY; y<endY; y++) {
				if (x>=0 && x<GameConstants.MapWidthPixels && y>=0 && y<GameConstants.MapHeightPixels){
					if (FloorCellProbabilities[x,y] >= highestParticleValue) {
						highestParticleValue = FloorCellProbabilities[x,y];
						xToSave = (short)x;
						yToSave = (short)y;
					}
				}
			}			
		}

		short[] points = new short[2];
		points[0] = xToSave;
		points[1] = yToSave;
		return points;
	}


	/*
	 * Kills particle and removes hash cover value
	 */	
	public void KillParticle(Particle p){
		FloorcellPartcileCover.Remove (ParticleCoverHash(p, p.CurrentLocationX, p.CurrentLocationY));
		Particles.Remove (p);
	}

	/*
	 * Adds particle to cover hash list
	 */	
	public void AddParticleToCoverHash(Particle particle, short locationX, short locationY){
		FloorcellPartcileCover.Add (ParticleCoverHash(particle, locationX, locationY));
    }

	/*
	 * Creates unique value for floor cell and particle combination
	 */	
	public ulong ParticleCoverHash(Particle particle, short locationX, short locationY) {
		return (ulong)(((ulong)locationX)*((ulong)123) + ((ulong)locationY)*((ulong)23413));
	}
}
