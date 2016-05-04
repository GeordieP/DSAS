using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour {
    // Enemy type will determine what enemy sprite to use, and what bullet type to use when shooting
    private int enemyType;
    private float timeBetweenShots = 1f;
    // borrow the EnemyBulletPool from GameManager
    private GameObjectPool enemyBulletPool;
    private bool initialized;

    private float health = Balance.ENEMY_INITIAL_HEALTH;

    Timer shootTimer;

    public void Init() {
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
        transform.position = new Vector3(0f, 5.0f, 0f);
        shootTimer.Start();
    }

    public void Despawn() {
        RandomizeType();
        shootTimer.Stop();
        transform.position = new Vector3(-50f, -50f, 0f);
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
            health -= bullet.Dmg_Value;
            if (health <= 0) Dead();
            GameManager.Instance.PlayerBulletReturnToPool(other.gameObject);
        }
    }

    void Update() {
        transform.Translate(new Vector3(Mathf.Sin(Time.time * 2) * 0.02f, -2 * Time.deltaTime, 0f));
        if (transform.position.y < -5) {
            GameManager.Instance.EnemyReturnToPool(gameObject);
        }
    }
}
