using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : PersistentUnitySingleton<GameManager> {
    // Sprite Storage
    private Sprite[] enemySprites;
    public Sprite[] EnemySprites { get { return enemySprites; } }
    private Sprite[] enemyBulletSprites;
    public Sprite[] EnemyBulletSprites { get { return enemyBulletSprites; } }
    private Sprite[] playerBulletSprites;
    public Sprite[] PlayerBulletSprites { get { return playerBulletSprites; } }

    // Prefabs
    private GameObject _enemyPrefab;
    private GameObject _playerPrefab;
    private GameObject _enemyBulletPrefab;
    private GameObject _playerBulletPrefab;

    // Object pooling
    private GameObjectPool enemyPool;
    private GameObjectPool enemyBulletPool;
    public GameObjectPool EnemyBulletPool { get { return enemyBulletPool; } }
    private GameObjectPool playerBulletPool;
    public GameObjectPool PlayerBulletPool { get { return playerBulletPool; } }

    // Timers
    private const float enemySpawnTimerDuration = 10f;
    private Timer enemySpawnTimer;

    // UI elements
    private Text playerScoreLabel;
    private GameObject pauseMenuContainer;

    // Player object
    private GameObject player;

    // values
    private float playerScore;

    // State variables
    private bool _loading;
    private bool _paused;

    /*---
    * Startup / Initialization
    ---*/
    
    // game scene will get loaded and this method will get called
    // this sets loading to be true until it's done with everything
	public void GameSceneLoaded () {
        _loading = true;

        // Populate sprite storage
        enemySprites = Resources.LoadAll<Sprite>("Sprites/enemy_ship");
        enemyBulletSprites = Resources.LoadAll<Sprite>("Sprites/enemy_bullet");
        playerBulletSprites = Resources.LoadAll<Sprite>("Sprites/player_bullet");

        // Populate prefabs
        _enemyPrefab = (GameObject)Resources.Load("Prefabs/Enemy", typeof(GameObject));
        _playerPrefab = (GameObject)Resources.Load("Prefabs/Player", typeof(GameObject));
        _enemyBulletPrefab = (GameObject)Resources.Load("Prefabs/EnemyBullet", typeof(GameObject));
        _playerBulletPrefab = (GameObject)Resources.Load("Prefabs/PlayerBullet", typeof(GameObject));

        // Deactivate all prefabs by default
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

        // UI elements
        playerScoreLabel = GameObject.Find("ScoreLabel").GetComponent<Text>();
        // Move the pause menu on screen (moved off in editor so it doesn't get in the way) and disable it
        pauseMenuContainer = GameObject.Find("PauseMenuContainer");
        pauseMenuContainer.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        pauseMenuContainer.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        pauseMenuContainer.SetActive(false);

        // Player object
        player = Instantiate(_playerPrefab, new Vector3(0f, -4.5f, 0f), Quaternion.identity) as GameObject;

        // values
        playerScore = 0;

        // Finished loading
        _loading = false;
        player.SetActive(true);
        // player.GetComponent<PlayerShoot>().Shooting = false;
        enemySpawnTimer.Start();
        enemySpawnTimer_onFinish();
	}

    private void CreateEnemyWave() {
        int waveSize = Random.Range(3, 10);
        int waveTypeIndex = Random.Range(0, EnemyWaves.WaveTypes.Length);

        GameObject[] enemies = enemyPool.Borrow(waveSize);

        for (int i = 0; i < waveSize; i++) {
            enemies[i].GetComponent<Enemy>().SetSpawnPosition(EnemyWaves.WaveTypes[waveTypeIndex].GetRandSpawnPoint());     // for now pick a random applicable spawn point from the wave type
            enemies[i].GetComponent<Enemy>().SetWaveType(waveTypeIndex);
        }

        StartCoroutine(SpawnEnemyWave(enemies, EnemyWaves.WaveTypes[waveTypeIndex]._delayBetweenSpawns));
    }

    private IEnumerator SpawnEnemyWave(GameObject[] enemies, float delayBetweenSpawns) {
        for (int i = 0; i < enemies.Length; i++) {
            enemies[i].GetComponent<Enemy>().Spawn();
            enemies[i].SetActive(true);
            yield return new WaitForSeconds(delayBetweenSpawns);
        }
    }


    /*---
    * Timer Tick / Finish event callbacks
    ---*/
    
    private void enemySpawnTimer_onFinish() {
        CreateEnemyWave();
    }

    /*---
    * Game Actions
    ---*/
    
    public void PlayerBomb() {
        enemyBulletPool.RestoreAll();
        enemyPool.RestoreAll();
        // show bomb animation/effect
        // fade background color?
    }

    public void SetPause(bool pause) {
        if (_paused == pause) return;
        _paused = pause;
        pauseMenuContainer.SetActive(pause);
        Time.timeScale = pause ? 0f : 1f;
    }

    /*---
    * Helper / Utility
    ---*/

    public void UpdateScore(int score) {
        playerScore += score;
        playerScoreLabel.text = playerScore.ToString();
    }

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
