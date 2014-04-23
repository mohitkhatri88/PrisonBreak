using UnityEngine;
using System.Collections;

/*
 * Game constants
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
	public const short GuardSearchDistance = -1;

	/* Distance that guard can catch PlayerAgent */
	public const short PlayerCaughtDistance = -1;

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

	// TODO: add constants for floor cell types (e.g. 0 = floor, 1 = call)
}
