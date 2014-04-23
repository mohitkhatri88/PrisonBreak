using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/*
 * Controls global game state
 */
public class GameEngine {
	// time game started
	DateTime gameStartTime = new DateTime();

	// agents
	PlayerAgent player = new PlayerAgent();
	CellmateAgent cellmate = new CellmateAgent();
	List<GuardAgent> guards = new List<GuardAgent>();
	List<RatAgent> rats = new List<RatAgent>();

	List<Coin> coins = new List<Coin>();
	public short RemainingCellmateLives { get; set; }
	public short RemainingSpeedBoostSeconds { get; set; }
	public short GuardsAvoided  { get; set; }
	public short NumberOfPlayerDeaths { get; set; }
	public short NumberOfgamesPlayed  { get; set; }

	// pcg values
	public short NumberOfPrisonCells_PCG { get; set; }
	public short NumberOfOpenPrisonCells_PCG { get; set; }
	public short NumberOfCoins_PCG { get; set; }
	public short NumberOfCoinsRequiredToWin_PCG { get; set; }
	public short PlayerPixelDistanceFromExit_PCG { get; set; }
	public short NumberOfCellmateLives_PCG { get; set; }
	public short NumberOfGuards_PCG { get; set; }
	public short SpeedBoostSeconds_PCG { get; set; }
	public short NumberOfFloorSensors_PCG { get; set; }
	public short NumberOfRats_PCG { get; set; }

	public void InitGame() {
		// Initialize PCG Values from the stored values in global info 
		// Create map (map arrays)
		// Create agents

		// start game by calling RunGame()
	}

	public void RunGame() {
		// update game enviornment by calling UpdateGameEnvironment()
		// show game visually by calling ShowGame()
		// call EndGame() when game is over
	}

	private void UpdateGameEnvironment () {
		// Update agents locations
	    // Update time, coins, and distance traveled
		// Update numberOfGuardsAvoided
	    // Update numberOfPlayerDeaths
	}

	private void EndGame() {
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
