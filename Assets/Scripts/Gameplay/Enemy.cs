using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : PooledEntity, IDamageable {
    // Enemy type will determine what enemy sprite to use, and what bullet type to use when shooting
    private int enemyType;
    private const float timeBetweenShots = 3f;
    // borrow the EnemyBulletPool from GameManager
    private GameObjectPool enemyBulletPool;
    private bool initialized;

    // IDamageable properties
    public Color originalColor { get; set; }
    public float initialHealth { get; set; }
    public float health { get; set; }


	// Shoot delay WaitForSeconds
	private static WaitForSeconds ENEMY_NEXT_SHOT_DELAY_WAITFORSECONDS = new WaitForSeconds(Balance.DAMAGED_ENEMY_NEXT_SHOT_DELAY);

	private float scoreValue;
    private Vector3 mostRecentVelocity;

    // set to Time.timeSinceLevelLoad on each Spawn() call to keep track of timing
    private float timeSpawned;

    // delegates
    private delegate Vector3 MoveDelegate(float timeSpawned, Vector3 position);
    private MoveDelegate movePattern;

    private delegate void ShootDelegate(GameObjectPool enemyBulletPool, int enemyType, Vector3 shooterPosition);
    private ShootDelegate shoot;

    Timer shootTimer;
    public void Init() {
        initialHealth = Balance.ENEMY_INITIAL_HEALTH;
        health = initialHealth;
        originalColor = GetComponent<SpriteRenderer>().color;

        // default move delegate
        // movePattern = MovePatterns.Linear;
        if (movePattern == null) movePattern = MovePatterns.Linear;
        shoot = ShootPatterns.Circular;

        scoreValue = health;

        initialized = true;
        enemyBulletPool = GameManager.Instance.EnemyBulletPool;
        RandomizeType();
        shootTimer = TimerManager.Instance.CreateTimerRepeat(timeBetweenShots);
        shootTimer.onFinish += shootTimer_onFinish;
    }

    public void RandomizeType() {
        enemyType = Random.Range(0, GameManager.Instance.EnemySprites.Length);
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.EnemySprites[enemyType];

        if (Balance.ENEMY_TYPES_BASE_HEALTHS[enemyType] + 4 * GameManager.Instance.Stage < Balance.ENEMY_TYPES_MAX_HEALTHS[enemyType])
            health = Balance.ENEMY_TYPES_BASE_HEALTHS[enemyType] + 4 * GameManager.Instance.Stage;
        else
            health = Balance.ENEMY_TYPES_MAX_HEALTHS[enemyType];

        switch (enemyType) {
            case 0:
            case 3:
                shoot = ShootPatterns.Circular;
                break;
            case 2:
                shoot = ShootPatterns.TriFanDown;
                break;
            case 5:
                shoot = ShootPatterns.DualShot;
                break;
            default:
                shoot = ShootPatterns.SingleStraightDown;
                break;
        }
    }

    public void Spawn() {
        if (!initialized) Init();
        timeSpawned = Time.timeSinceLevelLoad;
        if (Random.Range(0, 10) < 7) {
            shootTimer.Start();
        } else {
            shootTimer.Stop();
        }
    }

    public override void Despawn() {
        if (shootTimer != null) shootTimer.Stop();
        GetComponent<SpriteRenderer>().color = originalColor;
        RandomizeType();
        base.Despawn();
    }

    private void shootTimer_onFinish() {
        shoot(enemyBulletPool, enemyType, transform.position);
    }

    /*---
    * Implementing IDamageable members
    ---*/
    
    public void CheckHealth() {
        if (health <= 0) Dead();
    }

    public void Dead() {
        Vector3 explosionSpawnPosition = transform.position;
        float explosionStartTime = Time.timeSinceLevelLoad;

        GameManager.Instance.AddScore(scoreValue);

        // call pooled entity methods 
        Despawn();

        // spawn explosion
        GameObject[] particles = GameManager.Instance.ExplosionFragmentPool.Borrow(5);

        for (int i = 0; i < particles.Length; i++) {
            particles[i].GetComponent<ExplosionFragment>().Spawn(explosionSpawnPosition, mostRecentVelocity, explosionStartTime);
            particles[i].SetActive(true);
        }        
    }

    public void Knockback() {
        transform.Translate(new Vector3(0f, Balance.PLAYER_BULLET_KNOCKBACK_DISTANCE, 0f));
    }

    public IEnumerator ColorFlash() {
        GetComponent<SpriteRenderer>().color = Color.white;
		yield return Balance.DMG_FLASH_WAITFORSECONDS;
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    public IEnumerator ShootDelay() {
        shootTimer.Stop();
		yield return ENEMY_NEXT_SHOT_DELAY_WAITFORSECONDS;
        shootTimer.Start();
    }

    /*---
    * Enemy specific methods
    ---*/

    public void HitByBullet(Bullet bullet) {
        if (bullet.tag == "EnemyBullet") return;    // don't get hit by our own bullets. That would suck.

        // if we're not visible on the screen yet, don't take damage
        if (
            transform.position.y > Balance.ScreenBounds.top ||
            transform.position.y < Balance.ScreenBounds.bottom ||
            transform.position.x < Balance.ScreenBounds.left ||
            transform.position.x > Balance.ScreenBounds.right ) {
            return;
        }       


        health -= bullet.Dmg_Value;
        bullet.Despawn();
        
        StartCoroutine(ColorFlash());
        StartCoroutine(ShootDelay());
        CheckHealth();
        Knockback();
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

    private void FixedUpdate() {
        mostRecentVelocity = movePattern(timeSpawned, transform.position);
        transform.Translate(mostRecentVelocity);

        if (transform.position.y < Balance.DespawnBounds.bottom || transform.position.x < Balance.DespawnBounds.left || transform.position.x > Balance.DespawnBounds.right) {
            Despawn();
        }
    }
}

public static class MovePatterns {
    private static readonly float MOVE_SPEED = Balance.ENEMY_BASE_MOVE_SPEED;    // simpler to type
    private static readonly float TIME_MULTIPLIER = MOVE_SPEED * 0.01f;          // reduce time

    // move straight down
    public static Vector3 Linear(float timeSpawned, Vector3 position) {
        return new Vector3(0f, -MOVE_SPEED * Time.deltaTime, 0f);
    }

    // wave back and forth
    public static Vector3 WavyX(float timeSpawned, Vector3 position) {
        return new Vector3(Mathf.Cos(Time.timeSinceLevelLoad - timeSpawned) * TIME_MULTIPLIER, -MOVE_SPEED * Time.deltaTime, 0f);
    }

    // wave up and down moving left
    public static Vector3 WavyYLeft(float timeSpawned, Vector3 position) {
        return new Vector3(MOVE_SPEED * Time.deltaTime, Mathf.Cos(Time.timeSinceLevelLoad - timeSpawned) * TIME_MULTIPLIER, 0f);
    }

    // wave up and down moving right
    public static Vector3 WavyYRight(float timeSpawned, Vector3 position) {
        return new Vector3(-MOVE_SPEED * Time.deltaTime, Mathf.Cos(Time.timeSinceLevelLoad - timeSpawned) * TIME_MULTIPLIER, 0f);
    }

    // move in a circular pattern downwards
    public static Vector3 Circular(float timeSpawned, Vector3 position) {
        return new Vector3(Mathf.Sin(Time.timeSinceLevelLoad * 3 - timeSpawned * 3) * TIME_MULTIPLIER * 2, -TIME_MULTIPLIER + (Mathf.Cos(Time.timeSinceLevelLoad * 3 - timeSpawned * 3) * 0.04f), 0f);
    }
}

public static class ShootPatterns {
    // shoot single bullets at a time, straight down
    public static void SingleStraightDown(GameObjectPool enemyBulletPool, int enemyType, Vector3 shooterPosition) {
        GameObject bullet = enemyBulletPool.Borrow();
        bullet.GetComponent<EnemyBullet>().SetType(enemyType);
        bullet.GetComponent<EnemyBullet>().Spawn(shooterPosition, new Vector3(0f, 1f, 0));

        bullet.SetActive(true);
    }

    public static void TriFanDown(GameObjectPool enemyBulletPool, int enemyType, Vector3 shooterPosition) {
        GameObject[] bullets = enemyBulletPool.Borrow(3);
        EnemyBullet currentBullet;

        for (int i = 0; i < 3; i++) {
            currentBullet = bullets[i].GetComponent<EnemyBullet>();
            currentBullet.SetType(enemyType);
            float angle = 65 + 25 * i;
            currentBullet.SetRotation(angle);

            Vector3 moveDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
            currentBullet.Spawn(shooterPosition, moveDirection);
            bullets[i].SetActive(true);
        }
    }

    // 3 bullets down at incrementing angles, but each shooting after a delay
    public static void TriFanDownDelayed(GameObjectPool enemyBulletPool, int enemyType, Vector3 shooterPosition) {
        throw new System.NotImplementedException();
    }

    // spray out in a circle
    public static void Circular(GameObjectPool enemyBulletPool, int enemyType, Vector3 shooterPosition) {
        int bulletCount = 5;
        GameObject[] bullets = enemyBulletPool.Borrow(bulletCount);
        EnemyBullet currentBullet;
        
        float angleIncrement = 360 / bulletCount;

        for (int i = 0; i < bulletCount; i++) {
            currentBullet = bullets[i].GetComponent<EnemyBullet>();
            currentBullet.SetType(enemyType);

            float angle = angleIncrement + angleIncrement * i;
            currentBullet.SetRotation(angle);
            Vector3 moveDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
            currentBullet.Spawn(shooterPosition, moveDirection);
            bullets[i].SetActive(true);
        }
    }

    // spray out in a circle, each bullet shooting delayed after one another
    public static void CircularDelayed(GameObjectPool enemyBulletPool, int enemyType, Vector3 shooterPosition) {
        throw new System.NotImplementedException();
    }

    public static void DualShot(GameObjectPool enemyBulletPool, int enemyType, Vector3 shooterPosition) {
        GameObject bullet = enemyBulletPool.Borrow();
        bullet.GetComponent<EnemyBullet>().SetType(enemyType);
        bullet.GetComponent<EnemyBullet>().Spawn(shooterPosition - new Vector3(-0.2f, 0f, 0f), new Vector3(0f, 1f, 0));
        bullet.SetActive(true);

        bullet = enemyBulletPool.Borrow();
        bullet.GetComponent<EnemyBullet>().SetType(enemyType);
        bullet.GetComponent<EnemyBullet>().Spawn(shooterPosition - new Vector3(0.2f, 0f, 0f), new Vector3(0f, 1f, 0));
        bullet.SetActive(true);        
    }
}
