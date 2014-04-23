using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {
	
	public GUIText gameOverText, instructionsText, jumpsText;
	public int[,] map;
	
	// this seems pretty dodgy, although I guess if you know there is just one ... ugh
	private static GUIManager instance;
	
	void Start () {
		// perhaps should check here to make sure only one?
		generateMap ();
		instance = this;
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
		gameOverText.enabled = false;
		instructionsText.enabled = true;
		jumpsText.enabled = false;
	}
	
	void Update () {
		if(Input.GetButtonDown("Jump")){
			GameEventManager.TriggerGameStart();
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
		GameMap gameMap = new GameMap();
		gameMap.CreateMap();
		map = gameMap.GameMapArray;
		
		for(int i = 0; i < 100 ; i++){
			for(int j = 0; j < 100; j++){
				if(map[i,j] != 0 && map[i,j] != 4){
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					cube.transform.localPosition = new Vector3(((j - 50f) + 0.5f) * 10, 25f, ((49f-i) + 0.5f) * 10);
					//cube.transform.localPosition = new Vector3(((j - 50) + 0.5f) * 10f, 25f, ((49 - i) + 0.5f) * 10f);
					cube.transform.localScale = new Vector3(10, 50, 10);
				}
			}
		}
		
	}
	private void GameStart () {
		gameOverText.enabled = false;
		//instructionsText.enabled = false;
		StartCoroutine(FadeInstructions());
		jumpsText.enabled = true;
		enabled = false;
	}
	
	private void GameOver () {
		gameOverText.enabled = true;
		instructionsText.enabled = true;
		enabled = true;
	}
	
	public static void SetJumps(int jumps){
		instance.jumpsText.text = "Accelerate: " + jumps.ToString();
	}
}