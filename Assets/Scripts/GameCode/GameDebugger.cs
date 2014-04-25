using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Helps debug game code
 */
public static class GameDebugger {

	/*
	 * Prints array data (mainly used for Particle Filtering probability array data)
	 */
	public static void PrintArray(long gameStepNumber, string arrayDescription, short[,] data){
		string html = "<br />"+arrayDescription+", Game Step #"+gameStepNumber+":";
		html += "<table style=\"font-size:10pt\">";
		for (int i = 0; i<data.GetLength(0); i++) {
			html += "<tr>";
			for (int j = 0; j<data.GetLength(1); j++) {
				html += "<td>"+(data[i,j])+"</td>";
			}
			html += "</tr>";
		}
		html += "</table><br /><br />";

		Debug.Log (html);
	}
}