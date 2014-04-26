using UnityEngine;
using System.Collections;
using System;

/*
 * Represents Guard Agent. Guards chase Player Agent.
 */
public class CellmateAgent : GameAgent {

	public CellmateAgent() {
		this.LocationX = 3;
		this.LocationY = 3;
	}

	public void updateLocation() {
		int nextDirection = GameEngine.learner.UpdateLearner ();
		if (nextDirection == GameConstants.Left) {
			this.LocationX--;
		} else if (nextDirection == GameConstants.Right) {
			this.LocationX++;
		} else if (nextDirection == GameConstants.Up) {
			this.LocationY++;
		} else {
			this.LocationY--;
		}
	}

	public bool Respawned { get; set; }
}

