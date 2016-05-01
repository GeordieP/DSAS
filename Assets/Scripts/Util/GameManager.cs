using UnityEngine;
using System.Collections.Generic;

public class GameManager : PersistentUnitySingleton<GameManager> {

    // Sprite Storage
    private Sprite[] enemySprites;
    private Sprite[] bulletSprites;

    // Prefabs
    public GameObject _enemyPrefab;
    public GameObject _playerPrefab;

    // Object pooling
    private GameObjectPool<GameObject> enemyPool;

    // Keep track of loading
    private bool _loading;


    // game scene will get loaded and this method will get called
    // this sets loading to be true until it's done with everything
	void Init () {
        _loading = true;

        // Populate sprite storage
        enemySprites = Resources.LoadAll<Sprite>("Sprites/enemy_ship");

        // Prefabs
        _enemyPrefab.SetActive(false);

        // Object pooling
        enemyPool = new GameObjectPool<GameObject>(10, _enemyPrefab);


        // Finished loading
        _loading = false;
	}

    /*---
    * Helper / Utility
    ---*/
    
    public Sprite GetRandomEnemySprite() {
        return enemySprites[Random.Range(0, enemySprites.Length)];
    }

    public Sprite GetRandomBulletSprite() {
        // return bulletSprites[Random.Range(0, bulletSprites.Length)];
    }
}

public class GameObjectPool<T> : MonoBehaviour where T : Transform {
    private List<T> _inUse, _available;
    private T _initialStateItem;

    public GameObjectPool(int length, T initialStateItem) {
        _initialStateItem = initialStateItem;
        for (int i = 0; i < length; i++) {
            _available.Add(Instantiate(initialStateItem, Vector3.zero, Quaternion.identity) as T);
        }
    }

    public T RequestObject() {
        T requestedObj;

        lock (_available) {
            if (_available.Count > 0) {
                requestedObj = _available[0];
                _inUse.Add(requestedObj);
                _available.RemoveAt(0);
            } else {
                requestedObj = Instantiate(_initialStateItem, Vector3.zero, Quaternion.identity) as T;
                _inUse.Add(requestedObj);
            }
        }
        
        return requestedObj;
    }

    public void ReturnObject(T retObj) {
        lock (_available) {
            _available.Add(retObj);
            _inUse.Remove(retObj);
        }
    }
}
