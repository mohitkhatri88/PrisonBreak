using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour {
	
	public GUIText gameOverText, instructionsText;
	
	// this seems pretty dodgy, although I guess if you know there is just one ... ugh
	private static GUIManager instance;
	private bool isGame;
	private List<GameObject> guardObjects = new List<GameObject>();
	private List<GameObject> ratObjects = new List<GameObject>();
	private List<Coin> coinObjects = new List<Coin>();
	private float startTime;
	private float endTime;
	GameObject cellmateO;
	
	void Start () {
		// perhaps should check here to make sure only one?
		/* Map Generation */
		generateMap ();

		/* Init of the Game Engine */
		GameEngine.InitGame();

		/* Game Event Maneger activate*/
		instance = this;
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
		GameEventManager.GameUpdate += GameUpdate;

		/* Text Of the Game*/ 
		gameOverText.enabled = false;
		instructionsText.enabled = true;

		/* Check Game is on or not*/
		isGame = false;

		/* Making of Guards and intialize their position from Game Engine */
		GameObject guardO = GameObject.Find ("Guard");
		GameObject miniO = GameObject.Find ("MiniGuard");
		for(int i = 0 ; i < GameEngine.NumberOfGuards_PCG; i++){
			GuardAgent tempG = GameEngine.guards[i];
			GameObject Guard = (GameObject) Instantiate (guardO, ConvertLocation.ConvertToReal(tempG.LocationY, 4.58f, tempG.LocationX), Quaternion.identity);
			guardObjects.Add (Guard);
			GameObject Mini = (GameObject) Instantiate (miniO,miniO.transform.localPosition, Quaternion.identity);
			Mini.GetComponent<MiniMapGuard>().target = Guard;
		}
		/*GameObject Guard = (GameObject) Instantiate (GameObject.Find ("Guard"), ConvertLocation.ConvertToReal(GameEngine.guards[0].LocationX, 4.58f, 
		                                                                                                      GameEngine.guards[0].LocationY), Quaternion.identity);
		guardObjects.Add (Guard);
		GameObject Mini = (GameObject) Instantiate (GameObject.Find("MiniGuard"),GameObject.Find("MiniGuard").transform.localPosition, Quaternion.identity);
		Mini.GetComponent<MiniMapGuard>().target = Guard;*/

		/* Making of Rats and intialize their position from Game Engine */
		/*GameObject ratO = GameObject.Find ("Rat");
		for(int i = 0; i < GameEngine.NumberOfRats_PCG; i++){
			RatAgent tempR = GameEngine.rats[i];
			GameObject Rat = (GameObject) Instantiate (ratO, ConvertLocation.ConvertToReal(tempR.LocationX, 0.25f, tempR.LocationY), Quaternion.identity);
			ratObjects.Add (Rat);
		}*/
		/* Making of Cell Mate */

		/* Making of Coins and intialize their position from Game Engine */
		startTime = Time.time;

		cellmateO = GameObject.Find("CellMate");
		cellmateO.transform.localPosition = new Vector3(GameEngine.cellmate.LocationX, cellmateO.transform.localPosition.y, GameEngine.cellmate.LocationY);
	}
	
	void Update () {
		endTime = Time.time;
		if(Input.GetButtonDown("Jump") && !isGame){
			gameOverText.enabled = false;
			instructionsText.enabled = false;
			StartCoroutine(FadeInstructions());
			isGame = true;
			GameEventManager.TriggerGameStart();
		}
		if(isGame){
			if(endTime - startTime > 1){
				GameEngine.RunGame();
				GameDebugger.PrintArray(0, "Particle Filtering check", ParticleFilteringEstimator.FloorCellProbabilities);
				for(int i = 0 ; i < GameEngine.NumberOfGuards_PCG; i++){
					GuardAgent tempG = GameEngine.guards[i];
					guardObjects[i].transform.localPosition = ConvertLocation.ConvertToReal(tempG.LocationY, 4.58f, tempG.LocationX);
				}
				/*for(int i = 0; i < GameEngine.NumberOfRats_PCG; i++){
					RatAgent tempR = GameEngine.rats[i];
					ratObjects[i].transform.localPosition = ConvertLocation.ConvertToReal(tempR.LocationX, 0.25f, tempR.LocationY);
				}*/
				startTime = Time.time;
				cellmateO.transform.localPosition = new Vector3(GameEngine.cellmate.LocationX, cellmateO.transform.localPosition.y, GameEngine.cellmate.LocationY);
			}
		}
	}
	
	IEnumerator FadeInstructions() {
		for (float f = 5f; f >= 0; f -= 0.05f) {
			Color c = instructionsText.color;
			c.a = f/5f;
			instructionsText.color = c;
			yield return new WaitForSeconds(.01f);
		}
		instructionsText.enabled = false;
	}
	private void generateMap(){
		GameMap.CreateMap();
		int[,] tempMap = GameMap.GameMapArray;

		GameObject wallO = GameObject.Find("Wall");
		for(int i = 0; i < 100 ; i++){
			for(int j = 0; j < 100; j++){
				if(tempMap[i,j] != 0 && tempMap[i,j] != 4 && tempMap[i,j] != 3 && tempMap[i,j] != 5){
					GameObject cube = (GameObject)Instantiate(wallO, ConvertLocation.ConvertToReal(i, 25f, j), Quaternion.identity);
					cube.transform.localScale = new Vector3(10, 50, 10);
				}else if(tempMap[i,j] == 3){
					tempMap = checkJail(tempMap, i, j);
				}
			}
		}
		
	}

	private int[,] checkJail(int[,] tempMap, int i, int j){
		if(tempMap[i + 1, j] == 3){
			for(int k = 1 ; k < 6; k++){
				tempMap[i + k, j] = 0;
			}
			GameObject jailO = GameObject.Find("JailHorizontal");
			GameObject jail = (GameObject)Instantiate(jailO, ConvertLocation.ConvertToReal(i, 25f, j), Quaternion.Euler(0, 90, 0));
			jail.SetActive(true);
		}
		if(tempMap[i, j + 1] == 3){
			for(int k = 1 ; k < 6; k++){
				tempMap[i, j + k] = 0;
			}
			GameObject jailO = GameObject.Find("JailHorizontal");
			GameObject jail = (GameObject)Instantiate(jailO, ConvertLocation.ConvertToReal(i, 25f, j), Quaternion.identity);
			jail.SetActive(true);
		}

		return tempMap;
	}
	private void GameStart(){
	}

	private void GameUpdate() {
	}
	private void GameOver () {
		gameOverText.enabled = true;
		instructionsText.enabled = true;
	}

}