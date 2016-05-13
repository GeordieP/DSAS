using UnityEngine;
using System.Collections;

public class GameSceneActions : MonoBehaviour {

	public void PauseGame() {
        GameManager.Instance.PauseGame();
    }
}
