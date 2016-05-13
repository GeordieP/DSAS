using UnityEngine;
public class MainMenuSceneLoaded : MonoBehaviour {
    void Awake() {
        // Create GameManager -- This should persist through every scene and control things
        GameManager.Instance.Create();
    }
}
