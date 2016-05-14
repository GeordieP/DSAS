using UnityEngine;

/*
* Cleanly handle UI actions and call the methods to do the actual work
* these are often found in GameManager
*/
public class GameSceneActions : MonoBehaviour {

	public void Pause() {
        GameManager.Instance.SetPause(true);
    }

    public void Unpause() {
        GameManager.Instance.SetPause(false);
    }
}
