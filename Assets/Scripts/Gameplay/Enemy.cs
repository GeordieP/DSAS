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


    // delegates
    private delegate Vector3 MoveDelegate(Vector3 position);
    private MoveDelegate movePattern;

    Timer shootTimer;

    public void Init() {
        // move delegate
        // movePattern = MoveDelegates.Wavy;
        movePattern = MoveDelegates.Linear;

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
        transform.position = new Vector3(0f, Balance.ScreenBounds.top, 0f);
        shootTimer.Start();
    }

    public void Despawn() {
        RandomizeType();
        shootTimer.Stop();
        transform.position = new Vector3(-50f, -50f, 0f);
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    private void shootTimer_onFinish() {
        GameObject bullet = enemyBulletPool.Borrow();
        bullet.GetComponent<EnemyBullet>().SetType(enemyType);
        bullet.GetComponent<EnemyBullet>().Spawn(transform);
        bullet.SetActive(true);
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

    private IEnumerator ColorFlash() {
        GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(Balance.DMG_FLASH_DURATION);
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    void Update() {
        transform.Translate(movePattern(transform.position));
        if (transform.position.y < Balance.ScreenBounds.bottom) {

            GameManager.Instance.EnemyReturnToPool(gameObject);
        }
    }
}

public static class MoveDelegates {
    // move straight down
    public static Vector3 Linear(Vector3 position) {
        return new Vector3(0f, -2f * Time.deltaTime, 0f);
    }

    // wave back and forth
    public static Vector3 Wavy(Vector3 position) {
        return new Vector3(Mathf.Sin(Time.time * 2) * 0.02f, -2 * Time.deltaTime, 0f);
    }
}
