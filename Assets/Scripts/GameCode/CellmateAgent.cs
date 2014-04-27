using UnityEngine;
using System.Collections;
using System;

/*
 * Represents Guard Agent. Guards chase Player Agent.
 */
public class CellmateAgent : GameAgent {

	public CellmateAgent() {
		this.LocationX = 4;
		this.LocationY = 4;
	}

	public void updateLocation() {
		int nextDirection = GameEngine.learner.UpdateLearner ();
		//this.MovingDirection = nextDirection;
		if (nextDirection == GameConstants.Left) {
			this.LocationY--;
			GameDebugger.PrintMessage("Turning Left.");
		} else if (nextDirection == GameConstants.Right) {
			this.LocationY++;
			GameDebugger.PrintMessage("Turning Right.");
		} else if (nextDirection == GameConstants.Up) {
			this.LocationX--;
			GameDebugger.PrintMessage("Turning Up.");
		} else {
			this.LocationX++;
			GameDebugger.PrintMessage("Turning Down.");
		}
	}

	public bool Respawned { get; set; }
}

