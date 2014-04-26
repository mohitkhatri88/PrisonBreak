using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

/*
 * Helps debug game code
 */
public static class GameDebugger {

	/*
	 * Prints array data (mainly used for Particle Filtering probability array data)
	 */
	public static void PrintArray(long gameStepNumber, string arrayDescription, int[,] data){
		if (GameConstants.GameDebuggerOn) {
			StringBuilder sb = new StringBuilder();
			sb.Append("<br />"+arrayDescription+", Game Step #"+gameStepNumber+":");
			sb.Append("<table style=\"font-size:10pt\"; border-width:\"1px\";>");
			for (int i = 0; i<data.GetLength(0); i++) {
				sb.Append("<tr>");
				for (int j = 0; j<data.GetLength(1); j++) {
					sb.Append("<td>"+(data[i,j])+"</td>");
				}
				sb.Append("</tr>");
			}
		    sb.Append("</table><br /><br />");

			Debug.Log (sb.ToString ());
		}
	}

	/*
	 * Prints debug message
	 */
	public static void PrintMessage(string message){
		if (GameConstants.GameDebuggerOn) {
			Debug.Log(message);
		}
	}
}