using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Represents the knowledge of the Cellmate Agent. 
 */
public class ReinforcementLearner {
	/* Contains all floor cells that Agent can turn on. map key: unique hash value */
	Dictionary<ulong, TurningFloorCell> TurningFloorCellMap { get; set; }

	/* Previous turning cell that Agent has left */
	TurningFloorCell PrevTurningFloorCell { get; set; }

	/*Index of the previous turn cell.*/
	ulong PrevTurnFloorCellIndex{ get; set; }

	/* Previous turn direction that Agent has taken */
	public short PrevDirectionTaken { get; set; }

	/* Ones represent floor cells Agent has walked on */
	public byte[] CellmateExploredMap { get; set; }

	/*A map sized turningfloor cell probabilities. */
	//public TurningFloorCell[,] CellMap { get; set; }

	/*variable to keep the value for the epsilon.*/
	public double epsilon;

	/*Short value to set the learning rate.*/
	public double learningRate;
	/*
	 * ReinforcementLearner constructor 
	 */
	public ReinforcementLearner () {
		TurningFloorCellMap = new Dictionary<ulong, TurningFloorCell>();
		CellmateExploredMap = new byte[GameConstants.SizeOfMapArrayPixels];
		//CellMap = new TurningFloorCell[GameConstants.MapWidthPixels, GameConstants.MapHeightPixels];
		epsilon = 0.9;
	}

	/*
	 * Updates Agents knowledge of the environment
	 */
	public short UpdateLearner() {
		/*
		 * Keeping the next direction to be previous. 
		 * In case, we dont reach the middle, we just have to keep stepping like before
		 */
		short nextDirection = this.PrevDirectionTaken;
		//Check to see if we should change direction. 
		if (this.shouldChangeDirection(GameEngine.cellmate.LocationX, GameEngine.cellmate.LocationY)) {
			/*Check to see if this is a turn cell. 
			 * If it is, find the best direction, and update the previous direction. 
			 * Also, create a new turn cell if this one didnt exist. 
			 */
			if (this.isTurnCell(GameEngine.cellmate.LocationX, GameEngine.cellmate.LocationY)) {
				//Index for the current location. 
				ulong index = GameMap.HashFloorCell(GameEngine.cellmate.LocationX, GameEngine.cellmate.LocationY);

				//Add a turning cell if it doesnt exist. 
				if(!this.TurningFloorCellMap.ContainsKey(index)) {
					this.TurningFloorCellMap.Add(index, new TurningFloorCell(GameEngine.cellmate.LocationX, GameEngine.cellmate.LocationY));
				}

				//get the next direction that should be chosen. 
				nextDirection = this.getBestDirection(TurningFloorCellMap[index]);

				//Also, update the previous floor cell with the probabilities based on the current cell. 

				//Update the previous turn cell index. 
				this.PrevTurnFloorCellIndex = index;
			}
			//Now, decide the best direction from the available one's. 

			//Update the previous turn cell based on the current cell's distance from the exit position. 
		} 
		return this.PrevDirectionTaken;
	}

	public short getBestDirection(TurningFloorCell turnCell) {
		//Compare probabilities of all the directions to see which one is best. 
		short nextDirection = GameConstants.Left;
		double bestProbability = turnCell.LeftCornerProbability;
		if (turnCell.DownCornerProbability >= bestProbability) {
			nextDirection = GameConstants.Down;
			bestProbability = turnCell.DownCornerProbability;
		}
		if (turnCell.UpCornerProbability >= bestProbability) {
			nextDirection = GameConstants.Up;
			bestProbability = turnCell.UpCornerProbability;
		}
		if (turnCell.RightCornerProbability >= bestProbability) {
			nextDirection = GameConstants.Right;
		}

		return nextDirection;
	}

	/*
	 * Check to see if the current cell is a turning cell. 
	 * TODO: How to check if prison cell is open, and how to get into it. 
	 */
	public bool isTurnCell(short locationX, short locationY) {
		bool isTurnCell = false;
		//Number of Possible turns. 
		int nextCell = GameConstants.MapHeightPixels/2 + 1;
		int possibleTurns = 0;
		int right = locationX + nextCell;
		int left = locationX - nextCell;
		int up = locationY + nextCell;
		int down = locationY - nextCell;
		if (right==GameConstants.MapEntranceFloorcell || right==GameConstants.MapExitFloorcell || right==GameConstants.MapHallwayFloorcell) {
			possibleTurns++;
		}
		if (left==GameConstants.MapEntranceFloorcell || left==GameConstants.MapExitFloorcell || left==GameConstants.MapHallwayFloorcell) {
			possibleTurns++;
		}
		if (up==GameConstants.MapEntranceFloorcell || up==GameConstants.MapExitFloorcell || right==GameConstants.MapHallwayFloorcell) {
			possibleTurns++;
		}
		if (down==GameConstants.MapEntranceFloorcell || down==GameConstants.MapExitFloorcell || down==GameConstants.MapHallwayFloorcell) {
			possibleTurns++;
		}
		if (possibleTurns >= 2) {
			isTurnCell = true;
		}

		return isTurnCell;
	}
	/*
	 * You should always consider changing direction when reaching the middle of a cell. Keeps thing's consistent. 
	 */
	public bool shouldChangeDirection(short locationX, short locationY) {
		bool result = false;
		int cellCenter = GameConstants.MapHeightPixels / 2;
		int modX = (locationX - cellCenter) % GameConstants.TurningFloorCellWidthPixels;
		int modY = (locationY - cellCenter) % GameConstants.TurningFloorCellHeightPixels;
		if (modX == 0 || modY == 0) {
			result = true;
		}
		return result;
	}

	public short GetNextDirection() {
		//Compare the values for the current location of the cellmate. I assume this comes from the cellmate agent class. 
		return GameConstants.Left;
	}

	/*
	 * Gets new location and directionfor cellmate
	 */
	public short[] GetNextLocationWithDirection(short currentLocationX, short currentLocationY) {
		// TODO: change
		short[] location = new short[3];
		// location[0] is x
		// location[1] is y
		// location[2] is direction
		return location;
	}
}
