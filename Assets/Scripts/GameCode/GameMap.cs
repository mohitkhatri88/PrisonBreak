using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/*
 * Represents 2D game map
 */
[System.Serializable]
public class GameMap {
	/* Contains 2D floor cells with type values (0, 2, 3, ????) */
	public static int[,] gameMapArray;

	/* Contains distance each floor cell is to the exit (key: hash value of floor cell, value: distance to exit) */
	public Dictionary<int, int> FloorCellToExitDistanceMap;

	/*
	 * Constructor
	 */
	public GameMap () {
		gameMapArray = new int[GameConstants.MapWidthPixels, GameConstants.MapHeightPixels];
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
		int index = 0;
		try{
			TextAsset textFile = Resources.Load("text/result5") as TextAsset;
			StringReader sr = new StringReader(textFile.text);
			string source = sr.ReadLine();
			Debug.Log (source.Length);
			while(source != null){
				for(int j = 0; j < source.Length; j++){
					gameMapArray[index,j] = (int)char.GetNumericValue(source[j]);
				}
				index++;
				source = sr.ReadLine();
			}
		}catch(System.Exception ee){
			Debug.Log("Exception");
			return;
		}
	}
	public int[,] GameMapArray{
		get{
			return gameMapArray;
		}
		set{
			gameMapArray = value;
		}
	}

	/*
	 * Creates unique value for floor cell
	 * 
	 * TODO: does this need to return a long?
	 */
	public static ulong HashFloorCell(short locationX, short locationY) {
		//Getting the hash value for the given location. 
		return (locationX*37 + locationY*37*37);
	}

}
