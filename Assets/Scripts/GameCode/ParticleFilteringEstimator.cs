using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/*
 * Represents all GuardAgents knowledge
 */
public class ParticleFilteringEstimator {
	/* Particle */
	public List<Particle> Particles { get; set; }

	/* Probability of PlayerAgent being at a GameMap xy location (noise caused by RatAgents) */
	public byte[] FloorCellProbabilities { get; set; }

	/*
	 * Construtor
	 */
	public ParticleFilteringEstimator () {
		Particles = new List<Particle>();
		FloorCellProbabilities = new byte[GameConstants.SizeOfMapArrayPixels];
	}

	/*
	 * Updates Agents knowledge of the environment
	 */	
	public void UpdateEstimator() {

	}
}
