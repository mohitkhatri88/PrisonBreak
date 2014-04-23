using UnityEngine;
using System.Collections;

/*
 * Represents GameAgent interface (PlayerAgent, CellmateAgent, GuardAgent, and RatAgent)
 */
public class GameAgent {
	/* x coordinate in GameMap GameMapArray */
	public short LocationX { get; set; }

	/* y coordinate in GameMap GameMapArray */
	public short LocationY { get; set; }

	/* Unique value that identifies Agent */
	public short AgentId { get; set; }

	/* Direction Agent is moving */
	public short MovingDirection { get; set; }

	/* Checks whether Agent is active in the game */
	public short Alive { get; set; }

	/*
	 * Updates Agent's state in the world
	 */
	public void DoTurn() {

	}

	/*
	 * Gets xy position in GameMap GameMapArray
	 */
	public short[] GetLocation() {
		return null;
	}
}
