using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/*
 * Controls global game state
 */
public static class GameEngine {
	// time game started
	static DateTime gameStartTime = new DateTime();

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
	public static short PlayerPixelDistanceFromExit_PCG { get; set; }
	public static short NumberOfCellmateLives_PCG { get; set; }
	public static short NumberOfGuards_PCG { get; set; }
	public static short SpeedBoostSeconds_PCG { get; set; }
	public static short NumberOfFloorSensors_PCG { get; set; }
	public static short NumberOfRats_PCG { get; set; }

	public static void InitGame() {
		// Initialize PCG Values from the stored values in global info 
		// Create map (map arrays)
		// Create agents

		// start game by calling RunGame()
	}

	public static void setPlayerPosition() {
		//player.LocationX;
		//player.LocationY;
	}
	public static void RunGame() {
		// update game enviornment by calling UpdateGameEnvironment()
		// show game visually by calling ShowGame()
		// call EndGame() when game is over
	}

	private static void UpdateGameEnvironment () {
		// Update agents locations
	    // Update time, coins, and distance traveled
		// Update numberOfGuardsAvoided
	    // Update numberOfPlayerDeaths
	}

	private static void EndGame() {
		// update PCG values by using end game state

		/*  end game state:
		        Distance travelled at time of death
			    Coins collected
				Number of guards avoided
				Percentage of cellmate route explored
				Percentage of speed boost used
				Number of times died (maybe give player three lives)
				Time spent playing
        */

		/*  calculated pcg values:
				Number of Guards placed
				Starting position distance from exit
				Reinforcement learning rate
				Number of open prison cells
				Amount of time Player can speed boost
				Number of rats and floor sensors
        */

	}
}
