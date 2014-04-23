using UnityEngine;
using System.Collections;

/*
 * Represents a partile used for the Particle Filter Estimator
 */
public class Particle {
	/* Direction particle is moving */
	public short MovingDirection { get; set; }

	/* x coordinate in GameMap GameMapArray */
	public short CurrentLocationX { get; set; }

	/* y coordinate in GameMap GameMapArray */
	public short CurrentLocationY { get; set; }

	/* Value given based on if PlayerAgent or RatAgent created particle */
	public short ParticleValue { get; set; }

	/* Number of game steps until partile dies */
	public short TimeToLive { get; set; }
}
