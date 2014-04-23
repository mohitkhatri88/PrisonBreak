using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Represents 2D game map
 */
public class GameMap {
	/* Contains 2D floor cells with type values (0, 2, 3, ????) */
	public byte[] GameMapArray { get; set; }

	/* Contains distance each floor cell is to the exit (key: hash value of floor cell, value: distance to exit) */
	public Dictionary<int, int> FloorCellToExitDistanceMap;

	/* Contains GameMap positions that contain floor sensors (hash value of floor cell)   */
	List<int> FloorSensors;

	/*
	 * Constructor
	 */
	public GameMap () {
		GameMapArray = new byte[GameConstants.SizeOfMapArrayPixels];
		FloorCellToExitDistanceMap = new Dictionary<int, int>();
		FloorSensors = new List<int>();
	}

	/*
	 * Creates map info
	 */
	public void CreateMap() {
		// Map size (dimensions) - GameConstants class
		// Floor cell size (dimensions) - GameConstants class
		// Generate unity 3D map (coins, prison cells, floor, walls, start, and exit)
		// Initialize 2D map array (indexed position is a floor cell)
		// Initialize reinforcement learning turning floor cell positions (TurningFloorCell class)
		// Initialize each cell distance from exit
		// Initialize floor sensor locations (which floor cells will have sensors)
	}

	/*
	 * Creates unique value for floor cell
	 */
	public int HashFloorCell(short locationX, short locationY) {
		return 0;
	}

}
