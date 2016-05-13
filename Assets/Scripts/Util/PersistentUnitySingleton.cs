/*
By Michael Stevenson at http://redframe-game.com/blog/global-managers-with-generic-singletons/
*/

using UnityEngine;

public class PersistentUnitySingleton<T> : MonoBehaviour where T : Component {
	private static T instance;
	public static T Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<T>();
				if (instance == null) {
					GameObject obj = new GameObject();
					obj.name = "PersistentSingleton_" + typeof(T).FullName;
					// obj.hideFlags = HideFlags.HideAndDontSave;
					instance = obj.AddComponent<T>();
				}
			}
			return instance;
		}
	}

	// Helper to easily make sure an instance is created
	public virtual void Create() {
		print(gameObject.name + " created");
	}

	public virtual void Awake() {
		DontDestroyOnLoad(this.gameObject);
		if (instance == null) {
			instance = this as T;
		} else {
			Destroy(gameObject);
		}
	}
}
