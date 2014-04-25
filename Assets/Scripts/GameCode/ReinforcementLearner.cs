using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Represents the knowledge of the Cellmate Agent. 
 */
public class ReinforcementLearner {
	/* Contains all floor cells that Agent can turn on. map key: unique hash value */
	Dictionary<int, TurningFloorCell> TurningFloorCellMap { get; set; }

	/* Previous turning cell that Agent has left */
	TurningFloorCell PrevTurningFloorCell { get; set; }

	/* Previous turn direction that Agent has taken */
	public short PrevDirectionTaken { get; set; }

	/* Ones represent floor cells Agent has walked on */
	public byte[] CellmateExploredMap { get; set; }

	/*A map sized turningfloor cell probabilities. */
	public TurningFloorCell[,] CellMap { get; set; }

	/*variable to keep the value for the epsilon.*/
	public double epsilon;

	/*Short value to set the learning rate.*/
	public double learningRate;
	/*
	 * ReinforcementLearner constructor 
	 */
	public ReinforcementLearner () {
		TurningFloorCellMap = new Dictionary<int, TurningFloorCell>();
		CellmateExploredMap = new byte[GameConstants.SizeOfMapArrayPixels];
		CellMap = new TurningFloorCell[GameConstants.MapWidthPixels, GameConstants.MapHeightPixels];
		epsilon = 0.9;
	}

	/*
	 * Updates Agents knowledge of the environment
	 */
	public short UpdateLearner() {
		//Get the best action from the available actions for the given state. 
		short nextDirection = GetNextDirection ();
		// Get the next state based on the direction. 

		// Last floor cell and direction taken
		// Store explored map (cellmate known map)
		return nextDirection;
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
