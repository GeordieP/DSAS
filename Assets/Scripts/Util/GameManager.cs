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
    private Sprite[] powerupSprites;
    public Sprite[] PowerupSprites { get { return powerupSprites; } }

    // Prefabs
    private GameObject _enemyPrefab;
    private GameObject _playerPrefab;
    private GameObject _enemyBulletPrefab;
    private GameObject _bossPrefab;
    private GameObject _bossBulletPrefab;
    private GameObject _playerBulletPrefab;
    private GameObject _explosionFragmentPrefab;
    private GameObject _powerupPrefab;

    // Object pooling
    private GameObjectPool enemyPool;
    private GameObjectPool enemyBulletPool;
    public GameObjectPool EnemyBulletPool { get { return enemyBulletPool; } }
    private GameObjectPool playerBulletPool;
    public GameObjectPool PlayerBulletPool { get { return playerBulletPool; } }
    private GameObjectPool explosionFragmentPool;
    public GameObjectPool ExplosionFragmentPool { get { return explosionFragmentPool; } }
    private GameObjectPool bossBulletPool;
    public GameObjectPool BossBulletPool { get { return bossBulletPool; } }
    private GameObjectPool powerupPool;
    public GameObjectPool PowerupPool { get { return powerupPool; } }

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

    // Current active boss (initially not assigned)
    private GameObject boss;

    // global values
    private float playerScore;
    private int stage_advancement_score;
    private int stage;
    public int Stage { get { return stage; } }

    // State variables
    private bool _loading;
    private bool _paused;
    public bool BossWaveActive { get; set; }

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
        powerupSprites = Resources.LoadAll<Sprite>("Sprites/powerups");

        // Populate prefabs
        _enemyPrefab = (GameObject)Resources.Load("Prefabs/Enemy", typeof(GameObject));
        _playerPrefab = (GameObject)Resources.Load("Prefabs/Player", typeof(GameObject));
        _enemyBulletPrefab = (GameObject)Resources.Load("Prefabs/EnemyBullet", typeof(GameObject));
        _bossPrefab = (GameObject)Resources.Load("Prefabs/Boss", typeof(GameObject));
        _bossBulletPrefab = (GameObject)Resources.Load("Prefabs/BossBullet", typeof(GameObject));
        _playerBulletPrefab = (GameObject)Resources.Load("Prefabs/PlayerBullet", typeof(GameObject));
        _explosionFragmentPrefab = (GameObject)Resources.Load("Prefabs/ExplosionFragment", typeof(GameObject));
        _powerupPrefab = (GameObject)Resources.Load("Prefabs/Powerup", typeof(GameObject));

        // Deactivate all prefabs by default
        _enemyPrefab.SetActive(false);
        _playerPrefab.SetActive(false);
        _enemyBulletPrefab.SetActive(false);
        _bossPrefab.SetActive(false);
        _playerBulletPrefab.SetActive(false);
        _explosionFragmentPrefab.SetActive(false);
        _powerupPrefab.SetActive(false);

        // Object pools
        enemyPool = new GameObjectPool(Balance.POOL_SIZE_ENEMY, _enemyPrefab);
        enemyBulletPool = new GameObjectPool(Balance.POOL_SIZE_ENEMY_BULLET, _enemyBulletPrefab);
        playerBulletPool = new GameObjectPool(Balance.POOL_SIZE_PLAYER_BULLET, _playerBulletPrefab);
        explosionFragmentPool = new GameObjectPool(Balance.POOL_SIZE_EXPLOSION_FRAGMENT, _explosionFragmentPrefab);
        bossBulletPool = new GameObjectPool(Balance.POOL_SIZE_BOSS_BULLET, _bossBulletPrefab);
        powerupPool = new GameObjectPool(Balance.POOL_SIZE_POWERUP, _powerupPrefab);

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

        // global values
        playerScore = 0;
        stage_advancement_score = Balance.INITIAL_STAGE_ADVANCEMENT_SCORE_CAP;
        stage = 1;

        // Finished loading
        _loading = false;
        player.SetActive(true);
        // player.GetComponent<PlayerShoot>().Shooting = false;

        NewPowerupSpawnScore();

        enemySpawnTimer.Start();
        enemySpawnTimer_onFinish();
    }

    private void CreateEnemyWave() {
        if (BossWaveActive) return;
        int waveSize = Random.Range(Balance.ENEMY_WAVE_MIN_SIZE, Balance.ENEMY_WAVE_MAX_SIZE);
        int waveTypeIndex = Random.Range(0, EnemyWaves.WaveTypes.Length);

        print("spawning wave");
        print("available pool size: " + enemyPool.Available.Count);
        print("inuse pool size: " + enemyPool.InUse.Count);

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
    * Timer Tick / Finish event callbacks / Updates
    ---*/
    
    private void enemySpawnTimer_onFinish() {
        CreateEnemyWave();
    }

    private float nextPowerupSpawnScore = 300;

    private float NewPowerupSpawnScore() {
        return Random.Range(playerScore + 700f, stage_advancement_score);
    }

    private void FixedUpdate() {
        if (playerScore >= nextPowerupSpawnScore) {
            // generate a new spawn score
            nextPowerupSpawnScore = NewPowerupSpawnScore();

            // spawn a powerup
            GameObject powerup = powerupPool.Borrow();
            powerup.GetComponent<Powerup>().Spawn();
            powerup.SetActive(true);
        }
    }

    /*---
    * Game Actions
    ---*/
    
    public void PlayerNuke() {

        enemySpawnTimer.Stop();

        GameObject[] toKill = enemyPool.GetInUse();

        for (int i = 0; i < toKill.Length; i++) {
            toKill[i].GetComponent<Enemy>().Dead();
        }

        enemySpawnTimer.Start();

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
        bossBulletPool.Clear();

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

    public void SetEnemySpawnEnabled(bool enabled) {
        if (enabled)
            enemySpawnTimer.StartInstant();
        else
            enemySpawnTimer.Stop();
    }

    public void AddScore(float score) {
        playerScore += score;
        playerScoreLabel.text = playerScore.ToString();

        // stage advancement based on score
        if (playerScore > stage_advancement_score) AdvanceStage();
    }

    private void AdvanceStage() {
        // TODO: maybe we don't need a whole method for this; scaling implementations might mostly exist outside of this method
        // TODO: some text popup to show us the stage has advanced
        stage++;

        // every 2 stages, spawn a boss 
        if (stage % 2 == 0) {
            print("spawning a boss");
            // stop the enemy spawner
            SetEnemySpawnEnabled(false);
            // wait for all the enemies to be dead before spawning the boss 


            // TODO: seems nukes actually arent returning everything to their pools


            enemyPool.onPoolFull += ScreenClear;
            BossWaveActive = true;
        }

        if (stage_advancement_score * Balance.STAGE_ADVANCEMENT_SCORE_MULTIPLIER < Balance.STAGE_ADVANCEMENT_SCORE_CAP) {
            stage_advancement_score *= Balance.STAGE_ADVANCEMENT_SCORE_MULTIPLIER;
        } else {
            stage_advancement_score += Balance.STAGE_ADVANCEMENT_SCORE_CAP;
        }

        // scale enemy health
            // done in Enemy.RandomizeType()
        // Bullet damage scaling
            // done in EnemyBullet.SetType()


        /* to scale:
        chance of an enemy being a shooter
        spawn patterns
        enemy counts
        movement patterns
        move speeds?
        frequency of spawns?
        */
    }

    // easy control for player's shooting boolean, as not everywhere has
    // access to the player object
    public void SetPlayerShoot(bool enabled) {
        player.GetComponent<PlayerShoot>().Shooting = enabled;
    }

    // called by Pool Full event in enemy pool
    // when there are no enemies left alive
    public void ScreenClear() {
        // stop player shooting
        SetPlayerShoot(false);

        // screen is free of enemies, spawn the boss
        boss = Instantiate(_bossPrefab, new Vector3(0f, Balance.BossSpawnBounds.top, 0f), Quaternion.identity) as GameObject;
        boss.GetComponent<Boss>().Spawn();
        boss.SetActive(true);
        enemyPool.onPoolFull -= ScreenClear;
    }
}
