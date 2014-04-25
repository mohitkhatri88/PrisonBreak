using UnityEngine;
using System.Collections;

/*
 * Game constants
 * 
 * TODO: update and tweak constants
 */
public class GameConstants {
	/* Left direction */
	public const short Left = 0;

	/* Down direction */
	public const short Down = 1;

	/* Right direction */
	public const short Right = 2;

	/* Up direction */
	public const short Up = 3;

	/* Id that uniquely identifies CellmateAgent */
	public const short CellmateAgentId = 4;

	/* Id that uniquely identifies PlayerAgent */
	public const short PlayerAgentId = 5;

	/* Id that uniquely identifies GuardAgent */
	public const short GuardAgentId = 6;

	/* Id that uniquely identifies RatAgent */
	public const short RatAgentId = 7;

	/* Distance that guard can move from their current location */
	public const short GuardSearchDistancePixels = 25;

	/* Distance that guard can catch PlayerAgent */
	public const short PlayerCaughtDistancePixels = 10;

	/* Number of pixels on 3D map for every one pixel in 2D GameMap */
	public const short MapScalePixels = 10;

	/* Size of 2D GameMap */
	public const short SizeOfMapArrayPixels = 10000;

	/* Width of 2D GameMap */
	public const short MapWidthPixels = 100;

	/* Height of 2D GameMap */
	public const short MapHeightPixels = 100;

	/* Width of 2D GameMap turning floor cell */
	public const short TurningFloorCellWidthPixels = 7;

	/* Height of 2D GameMap turning floor cell */
	public const short TurningFloorCellHeightPixels = 7;

	/* Initial number of time stemps for particle to live */
	public const short ParticleLifeSpan = 100;

	/* Particle value when RatAgent steps on floor sensor */
	public const short RatAgentParticleValue = 25;

	/* IParticle value when PlayerAgent steps on floor sensor */
	public const short PlayerAgentParticleValue = 50;

	/* Floor value from Map Array */
	public const short MapFloor = 0;
	/* Wall value from Map Array */
	public const short MapWall = 1;
	/* Exit value from Map Array */
	public const short MapExit = 2;
	/* Cell Entrance value from Map Array */
	public const short MapEntranceCell = 3;
	/* Turning cell value from Map Array */
	public const short TurningCell = 4;
	/* Cell Space value from Map Array */
	public const short MapCell = 5;

	public const short GuardStartDistanceFromPlayer = 20;
	public const short NumberOfHallwayFloorTiles = 4930;

	/* Number of game steps to skip before printing GameDebugger */
	public const short GameDebuggerGameSteps = 5;

	/* Turns GameDebugger on and off */
	public const bool GameDebuggerOn = false;
}
