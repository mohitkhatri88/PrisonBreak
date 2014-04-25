using UnityEngine;
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
	static PlayerAgent player = new PlayerAgent();
	static CellmateAgent cellmate = new CellmateAgent();
	static List<GuardAgent> guards = new List<GuardAgent>();
	static List<RatAgent> rats = new List<RatAgent>();

	static List<Coin> coins = new List<Coin>();
	public static short RemainingCellmateLives { get; set; }
	public static short RemainingSpeedBoostSeconds { get; set; }
	public static short GuardsAvoided  { get; set; }
	public static short NumberOfPlayerDeaths { get; set; }
	public static short NumberOfgamesPlayed  { get; set; }

	// pcg values
	public static short NumberOfPrisonCells_PCG { get; set; }
	public static short NumberOfOpenPrisonCells_PCG { get; set; }
	public static short NumberOfCoins_PCG { get; set; }
	public static short NumberOfCoinsRequiredToWin_PCG { get; set; }
	public static short PlayerPixelStartDistanceFromExit_PCG { get; set; }
	public static short NumberOfCellmateLives_PCG { get; set; }
	public static short NumberOfGuards_PCG { get; set; }
	public static short SpeedBoostSeconds_PCG { get; set; }
	public static short NumberOfFloorSensors_PCG { get; set; }
	public static short NumberOfRats_PCG { get; set; }


	// pcg helper values
	public static short AvgNumberOfCoinsPlaced { get; set; }
	public static short AvgNumberOfCoinsCollected { get; set; }
	public static short TotalTimesPlayerInPrisonCell  { get; set; }
	public static bool PlayerPrevInPrisonCell { get; set; }


	// AI
	public static ParticleFilteringEstimator estimator = new ParticleFilteringEstimator();
	public static ReinforcementLearner learner = new ReinforcementLearner();


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
			NumberOfGuards_PCG = 26; // 1 for each turning cell
			SpeedBoostSeconds_PCG = 10;
			NumberOfFloorSensors_PCG  = 493; // 10% of walkable floor cells
			NumberOfRats_PCG = 26; // 1 for each guard 
		}

		// create game map
		GameMap.CreateMap ();
		// TODO: are all maps initialized?

		// Create agents
		System.Random random = new System.Random();
		bool playerPositionFound = false;

		short[] up = new short[2];
		up [0] = (short)(random.Next (0, 6) + PlayerPixelStartDistanceFromExit_PCG);
		up [1] = (short)(random.Next (0, 6) + PlayerPixelStartDistanceFromExit_PCG);

		short[] down = new short[2];
		down [0] = (short)(random.Next (0, 6) + PlayerPixelStartDistanceFromExit_PCG);
		down [1] = (short)(random.Next (0, 6) + PlayerPixelStartDistanceFromExit_PCG);

		short[] right = new short[2];
		right [0] = (short)(random.Next (0, 6) + PlayerPixelStartDistanceFromExit_PCG);
		right [1] = (short)(random.Next (0, 6) + PlayerPixelStartDistanceFromExit_PCG);

		short[] left = new short[2];
		left [0] = (short)(random.Next (0, 6) + PlayerPixelStartDistanceFromExit_PCG);
		left [1] = (short)(random.Next (0, 6) + PlayerPixelStartDistanceFromExit_PCG);


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

			up[1] = (short)(up[1]+(short)1);
			down[1] = (short)(down[1]-(short)1);
			right[1] = (short)(right[0]+(short)1);
			left[1] = (short)(left[0]-(short)1);
		}

		// set player alive and id
		player.AgentId = GameConstants.PlayerAgentId;
		player.Alive = 1;

		// set cellmate
		cellmate.LocationX = player.LocationX;
		cellmate.LocationY = player.LocationY;
		cellmate.AgentId = GameConstants.CellmateAgentId;
		cellmate.Alive = 1;
		cellmate.MovingDirection = (short)random.Next (0,4);

		// set guards
		for (int i = 0; i<NumberOfGuards_PCG; i++) {
			GuardAgent guard = new GuardAgent();
			guard.AgentId = GameConstants.GuardAgentId;
			guard.Alive = 1;
			guard.MovingDirection = (short)random.Next (0,4);

			bool guardPosFound = false;
			while (!guardPosFound) {
				short xCoord = (short)random.Next (0,GameConstants.MapWidthPixels);
				short yCoord = (short)random.Next (0,GameConstants.MapHeightPixels);
				short distance = (short)Math.Sqrt (((xCoord-player.LocationX)*(xCoord-player.LocationX))+((yCoord-player.LocationY)*(yCoord-player.LocationY)));

				if(distance > GameConstants.GuardStartDistanceFromPlayer && GameMap.GameMapArray[xCoord,yCoord]==GameConstants.MapHallwayFloorcell) {
					guard.LocationX = xCoord;
					guard.LocationY = yCoord;
					guardPosFound = true;
				}
			}

			guards.Add(guard);
		}

		// set rats
		for (int i = 0; i<NumberOfRats_PCG; i++) {
			RatAgent rat = new RatAgent();
			rat.AgentId = GameConstants.RatAgentId;
			rat.Alive = 1;
			rat.MovingDirection = (short)random.Next (0,4);
			
			bool ratPosFound = false;
			while (!ratPosFound) {
				short xCoord = (short)random.Next (0,GameConstants.MapWidthPixels);
				short yCoord = (short)random.Next (0,GameConstants.MapHeightPixels);

				if(GameMap.GameMapArray[xCoord,yCoord]==GameConstants.MapHallwayFloorcell) {
					rat.LocationX = xCoord;
					rat.LocationY = yCoord;
					ratPosFound = true;
				}
			}
			
			rats.Add(rat);			
		}


		// add floor sensors
		short numberOfFloorSensorsPlace = 0;
		while (numberOfFloorSensorsPlace < NumberOfFloorSensors_PCG) {
			short xCoord = (short)random.Next (0,GameConstants.MapWidthPixels);
			short yCoord = (short)random.Next (0,GameConstants.MapHeightPixels);

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

	public static void setPlayerPosition(short locationX, short locationY) {
		player.LocationX = locationX;
		player.LocationY = locationY;
	}

	public static bool RunGame() {
		// update guards knowledge
		estimator.UpdateEstimator ();

		// update cellmate knowledge
		learner.UpdateLearner ();

		// update game enviornment
		bool result = UpdateGameEnvironment ();

		// return game over condition
		return result;
	}

	private static bool UpdateGameEnvironment () {
		// lowest distance Player is to Guard
		short lowestPlayeristanceToGuard = GameConstants.GuardStartDistanceFromPlayer + 10;

		// update cellmate (location updated in reinforcement leaner)
		short lowestCellmateDistanceToGuard = GameConstants.GuardStartDistanceFromPlayer + 10;


		// update guards
		// TODO: add guard speeds
		for (int i = 0; i<guards.Count; i++) {
			short[] newLocation = estimator.GetGuardTargetLocation(guards[i]);

			short newX = newLocation[0];
			short newY = newLocation[1];
			short oldX = guards[i].LocationX;
			short oldY = guards[i].LocationY;

			short distanceUp = (short)Math.Sqrt (((newX-oldX)*(newX-oldX))+((newY+1-oldY)*(newY+1-oldY)));
			short distanceDown = (short)Math.Sqrt (((newX-oldX)*(newX-oldX))+((newY-1-oldY)*(newY-1-oldY)));
			short distanceLeft = (short)Math.Sqrt (((newX-oldX-1)*(newX-oldX-1))+((newY-oldY)*(newY-oldY)));
			short distanceRight = (short)Math.Sqrt (((newX-oldX+1)*(newX-oldX+1))+((newY-oldY)*(newY-oldY)));


			bool canGoUp = GameMap.GameMapArray[guards[i].LocationX,guards[i].LocationY+1] == GameConstants.MapHallwayFloorcell;
			bool canGoDown = GameMap.GameMapArray[guards[i].LocationX,guards[i].LocationY-1] == GameConstants.MapHallwayFloorcell;
			bool canGoRight = GameMap.GameMapArray[guards[i].LocationX+1,guards[i].LocationY] == GameConstants.MapHallwayFloorcell;
			bool canGoLeft = GameMap.GameMapArray[guards[i].LocationX-1,guards[i].LocationY] == GameConstants.MapHallwayFloorcell;


			bool locationSet = false;
			if (distanceUp < distanceDown && distanceUp < distanceLeft && distanceUp < distanceRight) { // up
				if (canGoUp) {
					guards[i].LocationY += 1;
					locationSet = true;
				} else {
					distanceUp = (short)(32000);
				}

			} else if (distanceDown < distanceUp && distanceDown < distanceLeft && distanceDown < distanceRight) { // down
				if (canGoDown) {
					guards[i].LocationY -= 1;
					locationSet = true;
				} else {
					distanceDown = (short)(32000);
				}

			} else if (distanceLeft < distanceUp && distanceLeft < distanceDown && distanceLeft < distanceRight) { // left
				if (canGoLeft) {
					guards[i].LocationX -= 1;
					locationSet = true;
				} else {
					distanceLeft = (short)(32000);
				}

			} else if (distanceRight < distanceUp && distanceRight < distanceDown && distanceRight < distanceLeft) { // right
				if (canGoRight) {
					guards[i].LocationX += 1;
					locationSet = true;
				} else {
					distanceRight = (short)(32000);
				}

			}

			if (locationSet) {
				// nothing
			}


			// lowest cellmate to guard distance
			short gLocationX = guards[i].LocationX;
			short gLocationY = guards[i].LocationY;
			short cLocationX = cellmate.LocationX;
			short cLocationY = cellmate.LocationY;
			short cellmateToGuardDistance = (short)Math.Sqrt (((gLocationX-cLocationX)*(gLocationX-cLocationX))+((gLocationY-cLocationY)*(gLocationY-cLocationY)));;
			if (lowestCellmateDistanceToGuard > cellmateToGuardDistance) {
				lowestCellmateDistanceToGuard = cellmateToGuardDistance;
			}

			// lowest player to guard sitance
			short pLocationX = cellmate.LocationX;
			short pLocationY = cellmate.LocationY;
			short playerToGuardDistance = (short)Math.Sqrt (((gLocationX-pLocationX)*(gLocationX-pLocationX))+((gLocationY-pLocationY)*(gLocationY-pLocationY)));;
			if (lowestPlayeristanceToGuard > playerToGuardDistance) {
				lowestPlayeristanceToGuard = playerToGuardDistance;
			}


			// guard doesn't need direction and guards stay in same place if it doesn't find best direction
		}


		// update rats
		System.Random random = new System.Random ();
		for (int i = 0; i<rats.Count; i++) {
			short direction = (short)random.Next(0,4);

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
		}


		// check if cellmate is caught
		if (lowestCellmateDistanceToGuard < GameConstants.PlayerCaughtDistancePixels) {
			cellmate.Alive = 0;
		}
	

		// check if Player is on floor sensor
		if (estimator.IsFloorSensorAtLocation(player.LocationX, player.LocationY)) {
			estimator.CreateParticle(GameConstants.PlayerAgentId, player.LocationX, player.LocationY);
		}

		// check if Player is caught by Guard
		if (lowestPlayeristanceToGuard < GameConstants.PlayerCaughtDistancePixels) {
			player.Alive = 0;
			return false;
		}


		// TODO: check if Player won - is this correct?
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
		NumberOfOpenPrisonCells_PCG = (short)(NumberOfPrisonCells_PCG * (TotalTimesPlayerInPrisonCell/(NumberOfPrisonCells_PCG*NumberOfgamesPlayed)));
		if (NumberOfOpenPrisonCells_PCG > NumberOfPrisonCells_PCG) {
			NumberOfOpenPrisonCells_PCG = NumberOfPrisonCells_PCG;
		} else if (NumberOfOpenPrisonCells_PCG < 5) {
			NumberOfOpenPrisonCells_PCG = 5;
		}

		// set number of coins placed
		short numberOfCoinsCollected = 0;
		for (int i = 0; i<coins.Count; i++) {
			if (coins[i].Collected == true) {
				++numberOfCoinsCollected;
			}
		}

		AvgNumberOfCoinsCollected = (short)(((AvgNumberOfCoinsCollected * (NumberOfgamesPlayed)) + numberOfCoinsCollected)/(NumberOfgamesPlayed));
		AvgNumberOfCoinsPlaced = (short)(((AvgNumberOfCoinsCollected * (NumberOfgamesPlayed)) + coins.Count)/(NumberOfgamesPlayed));
		NumberOfCoins_PCG = (short)((AvgNumberOfCoinsCollected / AvgNumberOfCoinsPlaced) * GameConstants.NumberOfHallwayFloorTiles);
		NumberOfCoinsRequiredToWin_PCG = (short)(NumberOfCoins_PCG*.75);

		// TODO: use game time somehow
		// set distance from exit
		short dieWinDiff = (short)(NumberOfgamesPlayed-NumberOfPlayerDeaths);
		if (dieWinDiff < (short)0) {
			PlayerPixelStartDistanceFromExit_PCG = (short)(50-(10*dieWinDiff));
		} else {
			PlayerPixelStartDistanceFromExit_PCG = (short)(50+(10*dieWinDiff));
		}

		// TODO: how do we use Percentage of cellmate route explored?
		// set number of cellmate lives
		if (isGameWin) {
			NumberOfCellmateLives_PCG -= (short)1;
		} else {
			NumberOfCellmateLives_PCG += (short)1;
		}

		// set number of guards
		if (isGameWin) {
			NumberOfGuards_PCG -= (short)5;
		} else {
			NumberOfGuards_PCG += (short)5;
		}

		// set amount of speed boost
		if (isGameWin) {
			SpeedBoostSeconds_PCG += (short)3;
		} else {
			SpeedBoostSeconds_PCG -= (short)3;
		}

		// set number of floor sensors
		if (isGameWin) {
			NumberOfFloorSensors_PCG += (short)(NumberOfFloorSensors_PCG*.15);
		} else {
			NumberOfFloorSensors_PCG -= (short)(NumberOfFloorSensors_PCG*.15);
		}

		// set number of rats
		if (isGameWin) {
			NumberOfRats_PCG += (short)(NumberOfRats_PCG*.05);
		} else {
			NumberOfRats_PCG += (short)(NumberOfRats_PCG*.05);
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
