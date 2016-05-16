using UnityEngine;
public class MainMenuSceneLoaded : MonoBehaviour {
    private void Awake() {
        // Create GameManager -- This should persist through every scene and control things
        GameManager.Instance.Create();
    }
}
