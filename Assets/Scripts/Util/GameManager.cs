using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : PersistentUnitySingleton<GameManager> {
    // Sprite Storage
    private Sprite[] enemySprites;
    public Sprite[] EnemySprites { get { return enemySprites; } }
    private Sprite[] enemyBulletSprites;
    public Sprite[] EnemyBulletSprites { get { return enemyBulletSprites; } }
    private Sprite[] playerBulletSprites;
    public Sprite[] PlayerBulletSprites { get { return playerBulletSprites; } }
    private Sprite[] explosionFragmentSprites;
    public Sprite[] ExplosionFragmentSprites { get { return explosionFragmentSprites; } }


    // Prefabs
    private GameObject _enemyPrefab;
    private GameObject _playerPrefab;
    private GameObject _enemyBulletPrefab;
    private GameObject _playerBulletPrefab;
    private GameObject _explosionFragmentPrefab;

    // Object pooling
    private GameObjectPool enemyPool;
    private GameObjectPool enemyBulletPool;
    public GameObjectPool EnemyBulletPool { get { return enemyBulletPool; } }
    private GameObjectPool playerBulletPool;
    public GameObjectPool PlayerBulletPool { get { return playerBulletPool; } }
    private GameObjectPool explosionFragmentPool;
    public GameObjectPool ExplosionFragmentPool { get { return explosionFragmentPool; } }

    // Timers
    private const float enemySpawnTimerDuration = Balance.ENEMY_WAVE_SPAWN_RATE;
    private Timer enemySpawnTimer;

    // UI elements
    private Text playerScoreLabel;
    private GameObject pauseMenuContainer;
    private RectTransform healthBarMaskFillTransform;
    private RectTransform nukeBarMaskFillTransform;

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
        _paused = false;

        // Populate sprite storage
        enemySprites = Resources.LoadAll<Sprite>("Sprites/enemy_ship");
        enemyBulletSprites = Resources.LoadAll<Sprite>("Sprites/enemy_bullet");
        playerBulletSprites = Resources.LoadAll<Sprite>("Sprites/player_bullet");
        explosionFragmentSprites = Resources.LoadAll<Sprite>("Sprites/explosion_fragments");

        // Populate prefabs
        _enemyPrefab = (GameObject)Resources.Load("Prefabs/Enemy", typeof(GameObject));
        _playerPrefab = (GameObject)Resources.Load("Prefabs/Player", typeof(GameObject));
        _enemyBulletPrefab = (GameObject)Resources.Load("Prefabs/EnemyBullet", typeof(GameObject));
        _playerBulletPrefab = (GameObject)Resources.Load("Prefabs/PlayerBullet", typeof(GameObject));
        _explosionFragmentPrefab = (GameObject)Resources.Load("Prefabs/ExplosionFragment", typeof(GameObject));

        // Deactivate all prefabs by default
        _enemyPrefab.SetActive(false);
        _playerPrefab.SetActive(false);
        _enemyBulletPrefab.SetActive(false);
        _playerBulletPrefab.SetActive(false);
        _explosionFragmentPrefab.SetActive(false);

        // Object pools
        enemyPool = new GameObjectPool(Balance.POOL_SIZE_ENEMY, _enemyPrefab);
        enemyBulletPool = new GameObjectPool(Balance.POOL_SIZE_ENEMY_BULLET, _enemyBulletPrefab);
        playerBulletPool = new GameObjectPool(Balance.POOL_SIZE_PLAYER_BULLET, _playerBulletPrefab);
        explosionFragmentPool = new GameObjectPool(Balance.POOL_SIZE_PLAYER_BULLET, _explosionFragmentPrefab);

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
        healthBarMaskFillTransform = GameObject.Find("HealthbarMaskFill").GetComponent<RectTransform>();
        nukeBarMaskFillTransform = GameObject.Find("NukebarMaskFill").GetComponent<RectTransform>();


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
        int waveSize = Random.Range(Balance.ENEMY_WAVE_MIN_SIZE, Balance.ENEMY_WAVE_MAX_SIZE);
        int waveTypeIndex = Random.Range(0, EnemyWaves.WaveTypes.Length);

        GameObject[] enemies = enemyPool.Borrow(waveSize);

        for (int i = 0; i < waveSize; i++) {
            enemies[i].GetComponent<Enemy>().SetSpawnPosition(EnemyWaves.WaveTypes[waveTypeIndex].GetSpawnPoint(i, waveSize));
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
    
    public void PlayerNuke() {

        for (int i = 0; i < enemyPool.InUse.Count; i++) {
            enemyPool.InUse[i].GetComponent<Enemy>().Dead();
        }

        enemyBulletPool.RestoreAll();
        enemyPool.RestoreAll();
        // show nuke animation/effect
        // fade background color?
    }

    public void SetPause(bool pause) {
        if (_paused == pause) return;
        _paused = pause;
        pauseMenuContainer.SetActive(pause);
        Time.timeScale = pause ? 0f : 1f;
    }

    public void ExitToMenu() {
        StopAllCoroutines();        // this could cause issues
        enemyPool.Clear();
        enemyBulletPool.Clear();
        playerBulletPool.Clear();

        player = null;
        Resources.UnloadUnusedAssets();

        // this may cause problems if we use timers for anything between calling this function
        // and transitioning the scene
        TimerManager.Instance.StopAndDeleteAll();
        
        // Transition scene to menu (should always be game scene index - 1)
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    /*---
    * Gameplay UI
    ---*/

    public void UpdateHealthBar(float percentage) {
        healthBarMaskFillTransform.localScale = new Vector3(percentage, 1f, 1f);
    }

    public void UpdateNukeBar(float percentage) {
        nukeBarMaskFillTransform.localScale = new Vector3(percentage, 1f, 1f);
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


    public void ExplosionFragmentReturnToPool(GameObject bullet) {
        bullet.SetActive(false);
        bullet.GetComponent<ExplosionFragment>().Despawn();
        explosionFragmentPool.Restore(bullet);
    }
}
