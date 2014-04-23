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

	/*
	 * ReinforcementLearner constructor 
	 */
	public ReinforcementLearner () {
		TurningFloorCellMap = new Dictionary<int, TurningFloorCell>();
		CellmateExploredMap = new byte[GameConstants.SizeOfMapArrayPixels];
	}

	/*
	 * Updates Agents knowledge of the environment
	 */
	public void UpdateLearner() {
		// Update turning floor cell probabilities (dead ends, distances, loops, coins) 
		// Last floor cell and direction taken
		// Store explored map (cellmate known map)
	}
}
