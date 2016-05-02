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
    private GameObjectPool enemyPool;

    // Timers
    private const float enemySpawnTimerDuration = 5f;
    private Timer enemySpawnTimer;

    // Player object
    private GameObject player;

    // Keep track of loading
    public bool _loading;


    // eventually call Init() from start menu but for now just call it on start
    public void Start() {
        Init();
    }

    // game scene will get loaded and this method will get called
    // this sets loading to be true until it's done with everything
	public void Init () {
        _loading = true;

        // Populate sprite storage
        enemySprites = Resources.LoadAll<Sprite>("Sprites/enemy_ship");

        // Prefabs
        _enemyPrefab.SetActive(false);
        _playerPrefab.SetActive(false);

        // Object pooling
        enemyPool = new GameObjectPool(10, _enemyPrefab);

        // Timers
        enemySpawnTimer = TimerManager.Instance.CreateTimerRepeat(enemySpawnTimerDuration);
        enemySpawnTimer.onFinish += enemySpawnTimer_onFinish;

        // Player object
        player = Instantiate(_playerPrefab, new Vector3(0f, -3f, 0f), Quaternion.identity) as GameObject;

        // Finished loading
        _loading = false;
        player.SetActive(true);
        enemySpawnTimer.Start();
	}

    public void Update() {

    }

    /*---
    * Timer Tick / Finish event callbacks
    ---*/
    
    void enemySpawnTimer_onFinish() {
        GameObject enemy = enemyPool.Borrow();
        enemy.SetActive(true);
        enemy.GetComponent<Enemy>().Spawn();
    }


    /*---
    * Helper / Utility
    ---*/
    public void EnemyReturnToPool(GameObject enemy) {
        enemy.SetActive(false);
        enemy.GetComponent<Enemy>().Despawn();
        enemyPool.Restore(enemy);
    }

    public Sprite GetRandomEnemySprite() {
        return enemySprites[Random.Range(0, enemySprites.Length)];
    }

    public Sprite GetRandomBulletSprite() {
        return bulletSprites[Random.Range(0, bulletSprites.Length)];
    }
}


public class GameObjectPool {
    private List<GameObject> _inUse, _available;
    private GameObject _initialStateItem;

    public GameObjectPool(int length, GameObject initialStateItem) {
        _initialStateItem = initialStateItem;

        _inUse = new List<GameObject>();
        _available = new List<GameObject>();
        
        for (int i = 0; i < length; i++) {
            _available.Add(MonoBehaviour.Instantiate(initialStateItem, Vector3.zero, Quaternion.identity) as GameObject);
        }
    }

    // retrieve an object from the pool
    public GameObject Borrow() {
        GameObject requestedObj;

        lock (_available) {
            if (_available.Count > 0) {
                requestedObj = _available[0];
                _inUse.Add(requestedObj);
                _available.RemoveAt(0);
            } else {
                requestedObj = MonoBehaviour.Instantiate(_initialStateItem, Vector3.zero, Quaternion.identity) as GameObject;
                _inUse.Add(requestedObj);
            }
        }
        
        return requestedObj;
    }

    // return an object to the pool
    public void Restore(GameObject retObj) {
        lock (_available) {
            _available.Add(retObj);
            _inUse.Remove(retObj);
        }
    }
}
