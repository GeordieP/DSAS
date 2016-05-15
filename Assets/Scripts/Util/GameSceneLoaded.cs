using UnityEngine;
public class GameSceneLoaded : MonoBehaviour {
	void Start () {
        print("Game Scene Loaded");
        GameManager.Instance.GameSceneLoaded();
	}
}
