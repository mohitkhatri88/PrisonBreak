using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Represents floor cells in prison that agents can turn on
 */
class TurningFloorCell {
	/* probability of agent taking left corner */
	public short LeftCornerProbability { get; set; }

	/* probability of agent taking down corner */
	public short DownCornerProbability { get; set; }

	/* probability of agent taking right corner */
	public short RightCornerProbability { get; set; }

	/* probability of agent taking up corner */
	public short UpCornerProbability { get; set; }

	/* x coordinate in GameMap GameMapArray */
	public short LocationX { get; set; }

	/* y coordinate in GameMap GameMapArray */
	public short LocationY { get; set; }

	/* unique value that identifies floor cell */
	public short FloorCellIndex { get; set; }

	/* unique hash value that can be used to index List<> and Ditionary<> */
	public int HashValue { get; set; }
}

