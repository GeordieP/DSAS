using UnityEngine;

public class GameManager : PersistentUnitySingleton<GameManager> {

    // Sprite Storage
    private Sprite[] enemySprites;
    public Sprite[] EnemySprites { get { return enemySprites; } }
    private Sprite[] enemyBulletSprites;
    public Sprite[] EnemyBulletSprites { get { return enemyBulletSprites; } }
    private Sprite[] playerBulletSprites;
    public Sprite[] PlayerBulletSprites { get { return playerBulletSprites; } }

    // Prefabs
    public GameObject _enemyPrefab;
    public GameObject _playerPrefab;
    public GameObject _enemyBulletPrefab;
    public GameObject _playerBulletPrefab;

    // Object pooling
    private GameObjectPool enemyPool;
    private GameObjectPool enemyBulletPool;
    public GameObjectPool EnemyBulletPool { get { return enemyBulletPool; } }
    private GameObjectPool playerBulletPool;
    public GameObjectPool PlayerBulletPool { get { return playerBulletPool; } }

    // Timers
    private const float enemySpawnTimerDuration = 1f;
    private Timer enemySpawnTimer;

    // Player object
    private GameObject player;

    // Keep track of loading
    public bool _loading;

    /*---
    * Startup / Initialization
    ---*/    

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
        enemyBulletSprites = Resources.LoadAll<Sprite>("Sprites/enemy_bullet");
        playerBulletSprites = Resources.LoadAll<Sprite>("Sprites/player_bullet");

        // Prefabs
        _enemyPrefab.SetActive(false);
        _playerPrefab.SetActive(false);
        _enemyBulletPrefab.SetActive(false);
        _playerBulletPrefab.SetActive(false);

        // Object pools
        enemyPool = new GameObjectPool(10, _enemyPrefab);
        enemyBulletPool = new GameObjectPool(100, _enemyBulletPrefab);
        playerBulletPool = new GameObjectPool(50, _playerBulletPrefab);

        // Timers
        enemySpawnTimer = TimerManager.Instance.CreateTimerRepeat(enemySpawnTimerDuration);
        enemySpawnTimer.onFinish += enemySpawnTimer_onFinish;

        // Player object
        player = Instantiate(_playerPrefab, new Vector3(0f, -4.5f, 0f), Quaternion.identity) as GameObject;

        // Finished loading
        _loading = false;
        player.SetActive(true);
        // player.GetComponent<PlayerShoot>().Shooting = false;
        enemySpawnTimer.Start();
        enemySpawnTimer_onFinish();
	}

    /*---
    * Timer Tick / Finish event callbacks
    ---*/
    
    private void enemySpawnTimer_onFinish() {
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

    public void EnemyBulletReturnToPool(GameObject bullet) {
        bullet.SetActive(false);
        bullet.GetComponent<EnemyBullet>().Despawn();
        enemyBulletPool.Restore(bullet);
    }

    public void PlayerBulletReturnToPool(GameObject bullet) {
        bullet.SetActive(false);
        bullet.GetComponent<PlayerBullet>().Despawn();
        playerBulletPool.Restore(bullet);
    }
}
