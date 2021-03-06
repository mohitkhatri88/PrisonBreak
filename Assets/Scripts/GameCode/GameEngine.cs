﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/*
 * Controls global game state
 */
public static class GameEngine {
	// time game started
	static DateTime GameStartTime = DateTime.Now;
	
	// agents
	public static PlayerAgent player = new PlayerAgent();
	public static CellmateAgent cellmate = new CellmateAgent();
	public static List<GuardAgent> guards = new List<GuardAgent>();
	public static List<RatAgent> rats = new List<RatAgent>();
	
	public static List<Coin> coins = new List<Coin>();
	public static int RemainingCellmateLives { get; set; }
	public static int RemainingSpeedBoostSeconds { get; set; }
	public static int GuardsAvoided  { get; set; }
	public static int NumberOfPlayerDeaths { get; set; }
	public static int NumberOfgamesPlayed  { get; set; }
	public static int NumberOfCoinsCollected { get; set; }
	
	// pcg values
	public static int NumberOfPrisonCells_PCG { get; set; }
	public static int NumberOfOpenPrisonCells_PCG { get; set; }
	public static int NumberOfCoins_PCG { get; set; }
	public static int NumberOfCoinsRequiredToWin_PCG { get; set; }
	public static int PlayerPixelStartDistanceFromExit_PCG { get; set; }
	public static int NumberOfCellmateLives_PCG { get; set; }
	public static int NumberOfGuards_PCG { get; set; }
	public static int SpeedBoostSeconds_PCG { get; set; }
	public static int NumberOfFloorSensors_PCG { get; set; }
	public static int NumberOfRats_PCG { get; set; }
	
	
	// pcg helper values
	public static int AvgNumberOfCoinsPlaced { get; set; }
	public static int AvgNumberOfCoinsCollected { get; set; }
	public static int TotalTimesPlayerInPrisonCell  { get; set; }
	public static bool PlayerPrevInPrisonCell { get; set; }
	
	
	// AI
	public static ParticleFilteringEstimator estimator = new ParticleFilteringEstimator();
	public static ReinforcementLearner learner = new ReinforcementLearner();
	
	
	public static bool WasPlayerPrevOnFloorSensor = false;
	
	public static int[] test = new int[53244];
	
	
	
	public static void InitGame() {
		// start timer
		GameStartTime = DateTime.Now;
		RemainingCellmateLives = NumberOfCellmateLives_PCG;
		RemainingSpeedBoostSeconds = SpeedBoostSeconds_PCG;
		
		// initial PCG values
		if (NumberOfgamesPlayed==0) {
			NumberOfPrisonCells_PCG = 21;
			NumberOfOpenPrisonCells_PCG = 21;
			NumberOfCoins_PCG = 493;  // 10% of walkable floor cells
			NumberOfCoinsRequiredToWin_PCG = 49; // 10% of coins placed
			PlayerPixelStartDistanceFromExit_PCG = 50; // start off halfway
			NumberOfCellmateLives_PCG = 3;
			NumberOfGuards_PCG = 15; // 1 for each turning cell
			SpeedBoostSeconds_PCG = 10;
			NumberOfFloorSensors_PCG  = 493; // 10% of walkable floor cells
			NumberOfRats_PCG = 26; // 1 for each guard 
		}
		
		// create game map
		//GameMap.CreateMap ();
		// TODO: are all maps initialized?
		
		// Create agents
		System.Random random = new System.Random();
		bool playerPositionFound = false;
		
		int[] up = new int[2];
		up [0] = (random.Next (0, 6) + PlayerPixelStartDistanceFromExit_PCG);
		up [1] = (random.Next (0, 6) + PlayerPixelStartDistanceFromExit_PCG);
		
		int[] down = new int[2];
		down [0] = (random.Next (0, 6) + PlayerPixelStartDistanceFromExit_PCG);
		down [1] = (random.Next (0, 6) + PlayerPixelStartDistanceFromExit_PCG);
		
		int[] right = new int[2];
		right [0] = (random.Next (0, 6) + PlayerPixelStartDistanceFromExit_PCG);
		right [1] = (random.Next (0, 6) + PlayerPixelStartDistanceFromExit_PCG);
		
		int[] left = new int[2];
		left [0] = (random.Next (0, 6) + PlayerPixelStartDistanceFromExit_PCG);
		left [1] = (random.Next (0, 6) + PlayerPixelStartDistanceFromExit_PCG);
		
		
		while (!playerPositionFound) {
			if (GameMap.GameMapArray[up[0],up[1]]==GameConstants.MapHallwayFloorcell) {
				player.LocationX = up[0];
				player.LocationY = up[1];
				playerPositionFound = true;
			} else if (GameMap.GameMapArray[down[0],down[1]]==GameConstants.MapHallwayFloorcell) {
				player.LocationX = down[0];
				player.LocationY = down[1];
				playerPositionFound = true;
			} else if (GameMap.GameMapArray[right[0],right[1]]==GameConstants.MapHallwayFloorcell) {
				player.LocationX = right[0];
				player.LocationY = right[1];
				playerPositionFound = true;
			} else if (GameMap.GameMapArray[left[0],left[1]]==GameConstants.MapHallwayFloorcell) {
				player.LocationX = left[0];
				player.LocationY = left[1];
				playerPositionFound = true;
			}
			
			up[1] = (up[1]-1);
			down[1] = (down[1]+1);
			right[1] = (right[0]+1);
			left[1] = (left[0]-1);
		}
		
		// set player alive and id
		player.AgentId = GameConstants.PlayerAgentId;
		player.Alive = 1;
		
		/*
		 * Cellmate initialization. 
		 */
		cellmate.LocationX = GameConstants.TurningFloorCellHeightPixels/2+1;
		cellmate.LocationY = GameConstants.TurningFloorCellHeightPixels/2+1;
		cellmate.AgentId = GameConstants.CellmateAgentId;
		cellmate.Alive = 0;
		cellmate.Respawned = false;
		cellmate.MovingDirection = GameConstants.Down;
		cellmate.distanceFromNewCell = GameConstants.TurningFloorCellHeightPixels/2+1;
		RemainingCellmateLives = 5;
		
		//Marking the first cell as explored. 
		/*for (int i=(cellmate.LocationY-GameConstants.TurningFloorCellWidthPixels);i<cellmate.LocationY+GameConstants.TurningFloorCellWidthPixels;i++) {
			for (int j=(cellmate.LocationX-GameConstants.TurningFloorCellHeightPixels);j<cellmate.LocationX+GameConstants.TurningFloorCellHeightPixels;j++) {
				cellmate.CellmateExploredMap[i,j] = 1;
			}
		}*/
		
		// set guards
		for (int i = 0; i<NumberOfGuards_PCG; i++) {
			GuardAgent guard = new GuardAgent();
			guard.AgentId = GameConstants.GuardAgentId;
			guard.Alive = 1;
			guard.MovingDirection = random.Next (0,4);
			
			switch (random.Next(0,4)) {
			case 0:
				guard.Speed = GameConstants.GuardSpeedMedium;
				break;
			case 1:
				guard.Speed = GameConstants.GuardSpeedFast;
				break;
			case 2:
				guard.Speed = GameConstants.GuardSpeedSlow;
				break;
			}
			
			
			bool guardPosFound = false;
			while (!guardPosFound) {
				int xCoord = random.Next (0,GameConstants.MapWidthPixels);
				int yCoord = random.Next (0,GameConstants.MapHeightPixels);
				double distance = Math.Sqrt (((xCoord-player.LocationX)*(xCoord-player.LocationX))+((yCoord-player.LocationY)*(yCoord-player.LocationY)));
				
				if(distance > GameConstants.GuardStartDistanceFromPlayer && GameMap.GameMapArray[xCoord,yCoord]==GameConstants.MapHallwayFloorcell) {
					guard.LocationX = xCoord;
					guard.LocationY = yCoord;
					guardPosFound = true;
				}
			}
			
			System.Random random2 = new System.Random();
			switch(random2.Next(0, 4)) {
			case 0:
				guard.MovingDirection = GameConstants.Up;
				break;
			case 1:
				guard.MovingDirection = GameConstants.Down;
				break;
			case 2:
				guard.MovingDirection = GameConstants.Left;
				break;
			case 3:
				guard.MovingDirection = GameConstants.Right;
				break;
			}
			guards.Add(guard);
		}
		
		// set rats
		for (int i = 0; i<NumberOfRats_PCG; i++) {
			RatAgent rat = new RatAgent();
			rat.AgentId = GameConstants.RatAgentId;
			rat.Alive = 1;
			rat.MovingDirection = random.Next (0,4);
			
			bool ratPosFound = false;
			while (!ratPosFound) {
				int xCoord = random.Next (0,GameConstants.MapWidthPixels);
				int yCoord = random.Next (0,GameConstants.MapHeightPixels);
				
				if(GameMap.GameMapArray[xCoord,yCoord]==GameConstants.MapHallwayFloorcell) {
					rat.LocationX = xCoord;
					rat.LocationY = yCoord;
					ratPosFound = true;
				}
			}
			
			rats.Add(rat);			
		}
		
		
		// add floor sensors
		int numberOfFloorSensorsPlace = 0;
		while (numberOfFloorSensorsPlace < NumberOfFloorSensors_PCG) {
			int xCoord = random.Next (0,GameConstants.MapWidthPixels);
			int yCoord = random.Next (0,GameConstants.MapHeightPixels);
			
			if (GameMap.GameMapArray[xCoord,yCoord]==GameConstants.MapHallwayFloorcell) {
				++numberOfFloorSensorsPlace;
				estimator.AddFloorSensor(xCoord, yCoord);
			}
		}
		
		
		// check if player is in prison cell (increment NumberOfTimesPlayerEnteredPrisonCells)
		// toggle PlayerPrevInPrisonCell
		if (GameMap.GameMapArray[player.LocationX,player.LocationY]==GameConstants.MapPrisonFloorcell && !PlayerPrevInPrisonCell) {
			PlayerPrevInPrisonCell = true;
			++TotalTimesPlayerInPrisonCell;
		}
		
		if (GameMap.GameMapArray[player.LocationX,player.LocationY]==GameConstants.MapHallwayFloorcell && PlayerPrevInPrisonCell) {
			PlayerPrevInPrisonCell = false;
		}
	}
	
	public static void setPlayerPosition(int locationX, int locationY) {
		player.LocationX = locationX;
		player.LocationY = locationY;
	}
	
	public static bool RunGame() {
		// update guards knowledge
		estimator.UpdateEstimator ();
		
		// update cellmate knowledge'
		if (cellmate.Alive == 1) {
			cellmate.updateLocation ();
		} else {
			if (cellmate.Respawned == true) {
				cellmate.Alive=1;
				cellmate.Respawned = false;
				//INitialize the location of the cellmate. 
			}
		}
		//learner.UpdateLearner ();
		
		// update game enviornment
		bool result = UpdateGameEnvironment ();
		
		
		//Debug.Log ("Before part");
		for (int a = 0; a<test.Length; a++) {
			int z = test[a];
		}
		//Debug.Log ("After part");
		
		// return game over condition
		//return result;
		return true;
	}
	
	private static bool UpdateGameEnvironment () {
		// lowest distance Player is to Guard
		double lowestPlayeristanceToGuard = GameConstants.GuardStartDistanceFromPlayer + 10;
		
		// update cellmate (location updated in reinforcement leaner)
		double lowestCellmateDistanceToGuard = GameConstants.GuardStartDistanceFromPlayer + 10;
		
		
		// update guards
		// TODO: add guard speeds
		for (int i = 0; i<guards.Count; i++) {
			int[] newLocation = estimator.GetGuardTargetLocation(guards[i]);
			
		
				int newX = newLocation[0];
				int newY = newLocation[1];
				int oldX = guards[i].LocationX;
				int oldY = guards[i].LocationY;
				
			bool[] directions = new bool[4];
				double[] distances = new double[4];
				distances[GameConstants.Up] = Math.Sqrt (((newX-oldX-1)*(newX-oldX-1))+((newY-oldY)*(newY-oldY)));
				distances[GameConstants.Down] = Math.Sqrt (((newX-oldX+1)*(newX-oldX+1))+((newY-oldY)*(newY-oldY)));
				distances[GameConstants.Left] = Math.Sqrt (((newX-oldX)*(newX-oldX))+((newY-oldY-1)*(newY-oldY-1)));
				distances[GameConstants.Right] = Math.Sqrt (((newX-oldX)*(newX-oldX))+((newY-oldY+1)*(newY-oldY+1)));

				/*double distanceUp = Math.Sqrt (((newX-oldX-1)*(newX-oldX-1))+((newY-oldY)*(newY-oldY)));
				double distanceDown = Math.Sqrt (((newX-oldX+1)*(newX-oldX+1))+((newY-oldY)*(newY-oldY)));
				double distanceLeft = Math.Sqrt (((newX-oldX)*(newX-oldX))+((newY-oldY-1)*(newY-oldY-1)));
				double distanceRight = Math.Sqrt (((newX-oldX)*(newX-oldX))+((newY-oldY+1)*(newY-oldY+1)));
				*/
				
				bool canGoUp = false;
				bool canGoDown = false;
				bool canGoRight = false;
				bool canGoLeft = false;
				
				try {
					canGoUp = GameMap.GameMapArray[guards[i].LocationX-4,guards[i].LocationY] == GameConstants.MapHallwayFloorcell
						|| GameMap.GameMapArray[guards[i].LocationX-4,guards[i].LocationY] == GameConstants.MapTurningFloorcell;

				//Debug.Log ("TRYING TO GO UP, VALUE: "+canGoUp);	
				} catch (Exception e) {
					//Debug.Log ("ERROR TRYING TO GO UP");	
				}
				try {
					canGoDown = GameMap.GameMapArray[guards[i].LocationX+4,guards[i].LocationY] == GameConstants.MapHallwayFloorcell
						|| GameMap.GameMapArray[guards[i].LocationX+4,guards[i].LocationY] == GameConstants.MapTurningFloorcell;
				//Debug.Log ("TRYING TO GO DOWN, VALUE: "+canGoDown);	
			} catch (Exception e) {
				//Debug.Log ("ERROR TRYING TO GO DOWN");	
				}
				
				try {
					canGoRight = GameMap.GameMapArray[guards[i].LocationX,guards[i].LocationY+4] == GameConstants.MapHallwayFloorcell
						|| GameMap.GameMapArray[guards[i].LocationX,guards[i].LocationY+4] == GameConstants.MapTurningFloorcell;
				//Debug.Log ("TRYING TO GO RIGHT, VALUE: "+canGoRight);	
			} catch (Exception e) {
				//Debug.Log ("ERROR TRYING TO GO RIGHT");	
				}
				try {
					canGoLeft = GameMap.GameMapArray[guards[i].LocationX,guards[i].LocationY-4] == GameConstants.MapHallwayFloorcell
						|| GameMap.GameMapArray[guards[i].LocationX,guards[i].LocationY-4] == GameConstants.MapTurningFloorcell;
				//Debug.Log ("TRYING TO GO LEFT, VALUE: "+canGoLeft);	
			} catch (Exception e) {
				//Debug.Log ("ERROR TRYING TO GO LEFT");	
				} 

			directions[GameConstants.Up] = canGoUp;
			directions[GameConstants.Down] = canGoDown;
			directions[GameConstants.Left] = canGoLeft;
			directions[GameConstants.Right] = canGoRight;




				
		    bool locationSet = false;
			//if (newLocation[2]==0) {
			if (false) {


				//Debug.Log ("Design up: "+distanceUp+", down: "+distanceDown+", left: "+distanceLeft+", right: "+distanceRight);

				double bestDistance = 320000;
				int bestDirection = 0;



				for (int k = 0; k<4; k++) {
					if (distances[k] <= bestDistance && directions[k] == true) {
						bestDistance = distances[k];
						bestDirection = k;
					}
				}


				switch (bestDirection) {
					case 3: // up 
						guards[i].LocationX -= 1;
						break;
					case 1: // down
						guards[i].LocationX += 1;
						break;
					case 0: // left
						guards[i].LocationY -= 1;
						break;
					case 2: // right
						guards[i].LocationY += 1;
						break;
				}

				//Debug.Log("Best direction: "+bestDirection);




				/*

				if (distanceUp < distanceDown && distanceUp < distanceLeft && distanceUp < distanceRight && !locationSet) { // up

					Debug.Log("IN UP");
						if (canGoUp) {
							guards[i].LocationX -= 1;
							locationSet = true;
							Debug.Log("up up up up HAS PLAYER MOVED?");
						} else {
							distanceUp = (32000);
						}
						
					}
					if (distanceDown < distanceUp && distanceDown < distanceLeft && distanceDown < distanceRight && !locationSet) { // down
					Debug.Log("IN DOWN");		
					if (canGoDown) {
								guards[i].LocationX += 1;
								locationSet = true;
								Debug.Log("down down down down HAS PLAYER MOVED?");
							} else {
								distanceDown = (32000);
							}
							
						}
					if (distanceLeft < distanceUp && distanceLeft < distanceDown && distanceLeft < distanceRight && !locationSet) { // left
					Debug.Log("IN LEFT");		
					if (canGoLeft) {
								guards[i].LocationY -= 1;
								locationSet = true;
								Debug.Log("left left left left HAS PLAYER MOVED?");
							} else {
								distanceLeft = (32000);
							}
							
						}
					if (distanceRight < distanceUp && distanceRight < distanceDown && distanceRight < distanceLeft && !locationSet) { // right
					Debug.Log("IN RIGHT");		
					if (canGoRight) {
								guards[i].LocationY += 1;
								locationSet = true;
								Debug.Log("right right right right HAS PLAYER MOVED?");
							} else {
								distanceRight = (32000);
							}
							
					}

				*/
				

				} else {
					System.Random random = new System.Random();
					if (guards[i].MovingDirection == GameConstants.Up) {
						
						if (canGoUp) {
							guards[i].LocationX -= 1;
						} else {
							guards[i].MovingDirection = random.Next(0, 4);
						}
					}
					
					if (guards[i].MovingDirection == GameConstants.Down) {
						if (canGoDown) {
							guards[i].LocationX += 1;
						} else {
							guards[i].MovingDirection = random.Next(0, 4);
						}
					}
					
					if (guards[i].MovingDirection == GameConstants.Left) {
						if (canGoLeft) {
							guards[i].LocationY -= 1;
						} else {
							guards[i].MovingDirection = random.Next(0, 4);
						}
					}
					
					if (guards[i].MovingDirection == GameConstants.Right) {
						if (canGoRight) {
							guards[i].LocationY += 1;
						} else {
							guards[i].MovingDirection = random.Next(0, 4);
						}
					}
				}
				
				if (locationSet) {
					//ParticleFilteringEstimator.FloorCellProbabilities[guards[i].LocationX, guards[i].LocationY] = 0;
				}
				
				
				// lowest cellmate to guard distance
				int gLocationX = guards[i].LocationX;
				int gLocationY = guards[i].LocationY;
				/*int cLocationX = cellmate.LocationX;
				int cLocationY = cellmate.LocationY;
				int cellmateToGuardDistance = Math.Sqrt (((gLocationX-cLocationX)*(gLocationX-cLocationX))+((gLocationY-cLocationY)*(gLocationY-cLocationY)));;
				if (lowestCellmateDistanceToGuard > cellmateToGuardDistance) {
					lowestCellmateDistanceToGuard = cellmateToGuardDistance;
				}*/
				
				// lowest player to guard sitance
				int pLocationX = player.LocationX;
				int pLocationY = player.LocationY;
				double playerToGuardDistance = Math.Sqrt (((gLocationX-pLocationX)*(gLocationX-pLocationX))+((gLocationY-pLocationY)*(gLocationY-pLocationY)));;
				if (lowestPlayeristanceToGuard > playerToGuardDistance) {
					lowestPlayeristanceToGuard = playerToGuardDistance;
				}
				
				
		
			
			
		}
		
		
		// update rats
		/*System.Random random = new System.Random ();
		for (int i = 0; i<rats.Count; i++) {
			int direction = random.Next(0,4);

			bool canGoUp = GameMap.GameMapArray[rats[i].LocationX,rats[i].LocationY+1] == GameConstants.MapHallwayFloorcell;
			bool canGoDown = GameMap.GameMapArray[rats[i].LocationX,rats[i].LocationY-1] == GameConstants.MapHallwayFloorcell;
			bool canGoRight = GameMap.GameMapArray[rats[i].LocationX+1,rats[i].LocationY] == GameConstants.MapHallwayFloorcell;
			bool canGoLeft = GameMap.GameMapArray[rats[i].LocationX-1,rats[i].LocationY] == GameConstants.MapHallwayFloorcell;

			switch (direction) {
				case GameConstants.Up:
					if (canGoUp) { rats[i].LocationY += 1; }
					break;
				case GameConstants.Down:
					if (canGoDown) { rats[i].LocationY -= 1; }
					break;
				case GameConstants.Left:
					if (canGoLeft) { rats[i].LocationX -= 1; }
					break;
				case GameConstants.Right:
					if (canGoRight) { rats[i].LocationX += 1; }
					break;
			}

			// check if rat is on floor sensor
			if (estimator.IsFloorSensorAtLocation(rats[i].LocationX, rats[i].LocationY)) {
				estimator.CreateParticle(GameConstants.RatAgentId, rats[i].LocationX, rats[i].LocationY);
			}
		}*/
		
		
		// check if cellmate is caught
		if (lowestCellmateDistanceToGuard < GameConstants.PlayerCaughtDistancePixels) {
			cellmate.Alive = 0;
			RemainingCellmateLives -=1;
		}
		
		
		// check if Player is on floor sensor
		if (estimator.IsFloorSensorAtLocation(player.LocationX, player.LocationY) && !WasPlayerPrevOnFloorSensor) {
			Debug.Log ("At floor sensor");
			estimator.CreateParticle(GameConstants.PlayerAgentId, player.LocationX, player.LocationY);
			WasPlayerPrevOnFloorSensor = true;
		}
		
		if (!estimator.IsFloorSensorAtLocation(player.LocationX, player.LocationY) && WasPlayerPrevOnFloorSensor) {
			WasPlayerPrevOnFloorSensor = false;
			Debug.Log("Left floor sensor");
		}
		
		// check if Player collected a coin
		for (int j = 0; j<coins.Count; j++) {
			if (Math.Abs (coins[j].LocationX-player.LocationX) < 2 && Math.Abs (coins[j].LocationY-player.LocationY) < 2) {
				coins[j].Collected = true;
				++NumberOfCoinsCollected;
			}
		}
		
		
		// check if Player is caught by Guard
		if (lowestPlayeristanceToGuard < GameConstants.PlayerCaughtDistancePixels) {
			player.Alive = 0;
			Debug.Log ("CAUGHT!!!!");
			
			return false;
		}
		
		
		// TODO: check if Player won - is this correct?
		int u = player.LocationX;
		if (GameMap.GameMapArray[player.LocationX,player.LocationY]==GameConstants.MapExitFloorcell) {
			return false;
		}
		
		return true;
	}
	
	/*
	 * isGameWin - whether or not player won game
	 */
	private static void EndGame(bool isGameWin) {
		// increment number of games played
		++NumberOfgamesPlayed;
		
		// check if player died
		if (!isGameWin) {
			++NumberOfPlayerDeaths;
		}
		
		
		// set number of prison cells open
		NumberOfOpenPrisonCells_PCG = (NumberOfPrisonCells_PCG * (TotalTimesPlayerInPrisonCell/(NumberOfPrisonCells_PCG*NumberOfgamesPlayed)));
		if (NumberOfOpenPrisonCells_PCG > NumberOfPrisonCells_PCG) {
			NumberOfOpenPrisonCells_PCG = NumberOfPrisonCells_PCG;
		} else if (NumberOfOpenPrisonCells_PCG < 5) {
			NumberOfOpenPrisonCells_PCG = 5;
		}
		
		// set number of coins placed
		AvgNumberOfCoinsCollected = (((AvgNumberOfCoinsCollected * (NumberOfgamesPlayed)) + NumberOfCoinsCollected)/(NumberOfgamesPlayed));
		AvgNumberOfCoinsPlaced = (((AvgNumberOfCoinsCollected * (NumberOfgamesPlayed)) + coins.Count)/(NumberOfgamesPlayed));
		NumberOfCoins_PCG = ((AvgNumberOfCoinsCollected / AvgNumberOfCoinsPlaced) * GameConstants.NumberOfHallwayFloorTiles);
		NumberOfCoinsRequiredToWin_PCG = (int)(NumberOfCoins_PCG*.75);
		
		// TODO: use game time somehow
		// set distance from exit
		int dieWinDiff = ((NumberOfgamesPlayed/2)-NumberOfPlayerDeaths);
		if (dieWinDiff < 0) {
			PlayerPixelStartDistanceFromExit_PCG = (50-(10*dieWinDiff));
		} else {
			PlayerPixelStartDistanceFromExit_PCG = (50+(10*dieWinDiff));
		}
		
		// TODO: how do we use Percentage of cellmate route explored?
		// set number of cellmate lives
		if (isGameWin) {
			NumberOfCellmateLives_PCG -= 1;
		} else {
			NumberOfCellmateLives_PCG += 1;
		}
		
		// set number of guards
		if (isGameWin) {
			NumberOfGuards_PCG -= 5;
		} else {
			NumberOfGuards_PCG += 5;
		}
		
		// set amount of speed boost
		if (isGameWin) {
			SpeedBoostSeconds_PCG += 3;
		} else {
			SpeedBoostSeconds_PCG -= 3;
		}
		
		// set number of floor sensors
		if (isGameWin) {
			NumberOfFloorSensors_PCG += (int)(NumberOfFloorSensors_PCG*.15);
		} else {
			NumberOfFloorSensors_PCG -= (int)(NumberOfFloorSensors_PCG*.15);
		}
		
		// set number of rats
		if (isGameWin) {
			NumberOfRats_PCG += (int)(NumberOfRats_PCG*.05);
		} else {
			NumberOfRats_PCG += (int)(NumberOfRats_PCG*.05);
		}
		
		
		// reset agents and coins
		player = new PlayerAgent();
		cellmate = new CellmateAgent();
		guards = new List<GuardAgent>();
		rats = new List<RatAgent>();		
		coins = new List<Coin>();
		
		
		// init new game
		InitGame ();
	}
}
