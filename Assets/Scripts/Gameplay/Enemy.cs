using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour {
    // Enemy type will determine what enemy sprite to use, and what bullet type to use when shooting
    private int enemyType;
    private float timeBetweenShots = 1f;
    // borrow the EnemyBulletPool from GameManager
    private GameObjectPool enemyBulletPool;
    private bool initialized;
    private Color originalColor;
    private float health = Balance.ENEMY_INITIAL_HEALTH;

    // set to Time.timeSinceLevelLoad on each Spawn() call to keep track of timing
    private float timeSpawned;

    // delegates
    private delegate Vector3 MoveDelegate(float timeSpawned, Vector3 position);
    private MoveDelegate movePattern;

    private delegate void ShootDelegate(GameObjectPool enemyBulletPool, int enemyType, Transform shooterTransform);
    private ShootDelegate shoot;

    Timer shootTimer;

    public void Init() {
        // default move delegate
        // movePattern = MovePatterns.Linear;
        if (movePattern == null) movePattern = MovePatterns.Linear;
        shoot = ShootPatterns.Circular;

        originalColor = GetComponent<SpriteRenderer>().color;
        initialized = true;
        enemyBulletPool = GameManager.Instance.EnemyBulletPool;
        RandomizeType();
        shootTimer = TimerManager.Instance.CreateTimerRepeat(timeBetweenShots);
        shootTimer.onFinish += shootTimer_onFinish;
        
    }

    public void RandomizeType() {
        enemyType = Random.Range(0, GameManager.Instance.EnemySprites.Length);
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.EnemySprites[enemyType];
    }

    public void Spawn() {
        if (!initialized) Init();
        timeSpawned = Time.timeSinceLevelLoad;
        shootTimer.Start();
    }

    public void Despawn() {
        RandomizeType();
        shootTimer.Stop();
        transform.position = new Vector3(-50f, -50f, 0f);
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    private void shootTimer_onFinish() {
        // GameObject bullet = enemyBulletPool.Borrow();
        // bullet.GetComponent<EnemyBullet>().SetType(enemyType);
        // bullet.GetComponent<EnemyBullet>().Spawn(transform);
        // bullet.SetActive(true);

        shoot(enemyBulletPool, enemyType, transform);
    }

    private void Dead() {
        GameManager.Instance.EnemyReturnToPool(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.name == "PlayerBullet(Clone)") {
            Bullet bullet = other.GetComponent<Bullet>();
            Knockback();
            health -= bullet.Dmg_Value;
            StartCoroutine(ColorFlash());
            if (health <= 0) Dead();
            GameManager.Instance.PlayerBulletReturnToPool(other.gameObject);
        }
    }

    void Knockback() {
        transform.Translate(new Vector3(0f, Balance.PLAYER_BULLET_KNOCKBACK_DISTANCE, 0f));
    }

    public void SetWaveType(int waveTypeIndex) {
        switch(waveTypeIndex) {
            case 0:
            case 1:
                movePattern = MovePatterns.WavyX;
                break;
            case 2:
                movePattern = MovePatterns.Circular;
                break;
            case 3:
                // if our initial spawn position is on the left of the screen, we move right and vice versa
                if (transform.position.x < 0) {
                    movePattern = MovePatterns.WavyYLeft;
                } else {
                    movePattern = MovePatterns.WavyYRight;
                }
                break;
        }
    }

    public void SetSpawnPosition(Vector3 spawnPos) {
        transform.position = spawnPos;
    }

    private IEnumerator ColorFlash() {
        GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(Balance.DMG_FLASH_DURATION);
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    void Update() {
        transform.Translate(movePattern(timeSpawned, transform.position));
        if (transform.position.y < Balance.ScreenBounds.bottom || transform.position.x < Balance.ScreenBounds.left || transform.position.x > Balance.ScreenBounds.right) {
            GameManager.Instance.EnemyReturnToPool(gameObject);
        }
    }
}

public static class MovePatterns {
    // move straight down
    public static Vector3 Linear(float timeSpawned, Vector3 position) {
        return new Vector3(0f, -2f * Time.deltaTime, 0f);
    }

    // wave back and forth
    public static Vector3 WavyX(float timeSpawned, Vector3 position) {
        return new Vector3(Mathf.Cos(Time.timeSinceLevelLoad - timeSpawned) * 0.02f, -2 * Time.deltaTime, 0f);
    }

    // wave up and down moving left
    public static Vector3 WavyYLeft(float timeSpawned, Vector3 position) {
        return new Vector3(1.5f * Time.deltaTime, Mathf.Cos(Time.timeSinceLevelLoad - timeSpawned) * 0.02f, 0f);
    }

    // wave up and down moving right
    public static Vector3 WavyYRight(float timeSpawned, Vector3 position) {
        return new Vector3(-1.5f * Time.deltaTime, Mathf.Cos(Time.timeSinceLevelLoad - timeSpawned) * 0.02f, 0f);
    }

    // move in a circular pattern downwards
    public static Vector3 Circular(float timeSpawned, Vector3 position) {
        return new Vector3(Mathf.Sin(Time.timeSinceLevelLoad * 3 - timeSpawned * 3) * 0.04f, -0.02f + (Mathf.Cos(Time.timeSinceLevelLoad * 3 - timeSpawned * 3) * 0.04f), 0f);
    }
}

public static class ShootPatterns {
    // implementations of Shoot()
    // takes playerPos

    // shoot single bullets at a time, straight down
    public static void SingleStraightDown(GameObjectPool enemyBulletPool, int enemyType, Transform shooterTransform) {
        GameObject bullet = enemyBulletPool.Borrow();
        bullet.GetComponent<EnemyBullet>().SetType(enemyType);
        bullet.GetComponent<EnemyBullet>().Spawn(shooterTransform, new Vector3(0f, 1f, 0));
        bullet.SetActive(true);
    }

    public static void TriFanDown(GameObjectPool enemyBulletPool, int enemyType, Transform shooterTransform) {
        GameObject[] bullets = enemyBulletPool.Borrow(3);
        EnemyBullet currentBullet;

        for (int i = 0; i < 3; i++) {
            currentBullet = bullets[i].GetComponent<EnemyBullet>();
            currentBullet.SetType(enemyType);
            // angle starts at 0 and -= 45 * i
            // moveangle should be 
            float angle = 85 + 5 * i;
            Vector3 meme = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
            currentBullet.Spawn(shooterTransform, meme);
            bullets[i].SetActive(true);
        }
    }

    // 3 bullets down at incrementing angles, but each shooting after a delay
    public static void TriFanDownDelayed(GameObjectPool enemyBulletPool, int enemyType, Transform shooterTransform) {
        throw new System.NotImplementedException();
    }

    // spray out in a circle
    public static void Circular(GameObjectPool enemyBulletPool, int enemyType, Transform shooterTransform) {
        int bulletCount = 5;
        GameObject[] bullets = enemyBulletPool.Borrow(bulletCount);
        EnemyBullet currentBullet;
        
        float angleIncrement = 360 / bulletCount;

        for (int i = 0; i < bulletCount; i++) {
            currentBullet = bullets[i].GetComponent<EnemyBullet>();
            currentBullet.SetType(enemyType);
            
            // angle increment will be 360 / bulletCount 
            // angle will be angleIncrement + angleincrement * i

            float angle = angleIncrement + angleIncrement * i;
            Vector3 meme = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
            currentBullet.Spawn(shooterTransform, meme);
            bullets[i].SetActive(true);
        }
    }

    // spray out in a circle, each bullet shooting delayed after one another
    public static void CircularDelayed(GameObjectPool enemyBulletPool, int enemyType, Transform shooterTransform) {
        throw new System.NotImplementedException();
    }
}
