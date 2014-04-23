using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Represents 2D game map
 */
public class GameMap {
	/* Contains 2D floor cells with type values (0, 2, 3, ????) */
	public static byte[,] GameMapArray { get; set; }

	/* Contains distance each floor cell is to the exit (key: hash value of floor cell, value: distance to exit) */
	public Dictionary<int, int> FloorCellToExitDistanceMap;

	/*
	 * Constructor
	 */
	public GameMap () {
		GameMapArray = new byte[GameConstants.MapWidthPixels, GameConstants.MapHeightPixels];
		FloorCellToExitDistanceMap = new Dictionary<int, int>();
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
	 * 
	 * TODO: does this need to return a long?
	 */
	public static int HashFloorCell(short locationX, short locationY) {
		return 0;
	}

}
