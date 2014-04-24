using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {
	
	public GUIText gameOverText, instructionsText;
	
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
		GameMap.CreateMap();
		short[,] tempMap = GameMap.GameMapArray;
		
		for(int i = 0; i < 100 ; i++){
			for(int j = 0; j < 100; j++){
				if(tempMap[i,j] != 0 && tempMap[i,j] != 4 && tempMap[i,j] != 3){
					GameObject cube = (GameObject)Instantiate(GameObject.Find("Wall"), new Vector3(((j - 50f) + 0.5f) * 10, 25f, ((49f-i) + 0.5f) * 10), Quaternion.identity);
					//cube.transform.localPosition = new Vector3(((j - 50f) + 0.5f) * 10, 25f, ((49f-i) + 0.5f) * 10);
					//cube.transform.localPosition = new Vector3(((j - 50) + 0.5f) * 10f, 25f, ((49 - i) + 0.5f) * 10f);
					cube.transform.localScale = new Vector3(10, 50, 10);
				}else if(tempMap[i,j] == 3){
					Debug.Log ("asdasdasd");
					tempMap = checkJail(tempMap, i, j);
				}
			}
		}
		
	}

	private short[,] checkJail(short[,] tempMap, int i, int j){
		if(tempMap[i + 1, j] == 3){
			for(int k = 1 ; k < 6; k++){
				tempMap[i + k, j] = 0;
			}
			GameObject jail = (GameObject)Instantiate(GameObject.Find("JailHorizontal"), new Vector3(((j - 50f) + 0.5f) * 10, 25f, ((49f-i) + 0.5f) * 10), Quaternion.Euler(0, 90, 0));
			jail.SetActive(true);
		}
		if(tempMap[i, j + 1] == 3){
			for(int k = 1 ; k < 6; k++){
				tempMap[i, j + k] = 0;
			}
			GameObject jail = (GameObject)Instantiate(GameObject.Find("JailHorizontal"), new Vector3(((j - 50f) + 0.5f) * 10, 25f, ((49f-i) + 0.5f) * 10), Quaternion.identity);
			jail.SetActive(true);
		}

		return tempMap;
	}
	private void GameStart () {
		gameOverText.enabled = false;
		//instructionsText.enabled = false;
		StartCoroutine(FadeInstructions());
		enabled = false;
	}
	
	private void GameOver () {
		gameOverText.enabled = true;
		instructionsText.enabled = true;
		enabled = true;
	}

}