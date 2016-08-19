using UnityEngine;
using System.Collections;

public abstract class Boss : MonoBehaviour, IDamageable {
    protected byte currentPhase;

    // Intro
    protected bool introMoveComplete, introRotationComplete;
    public const float IntroDuration = 4f;        // seconds
    protected float introVelocity;
    protected Vector3 postIntroPosition = new Vector3(0f, 3f, 0f);

    // Object Pooling
    protected GameObjectPool bossBulletPool;

    // Timers
    protected Timer shootTimer;

    // Shooting
    protected const float timeBetweenShots = 0.2f;

    public Color originalColor { get; set; }
    public Color flashColor;
    public float initialHealth { get; set; }
    public float health { get; set; }

    // TODO: remove awake, it's only to call Spawn for testing right now
    void Awake() {
        Spawn();
    }

    protected virtual void Spawn() {
        initialHealth = Balance.BOSS_BASE_HEALTH;
        health = initialHealth;
        originalColor = GetComponent<SpriteRenderer>().color;
        flashColor = new Color(originalColor.r + 0.2f, originalColor.g + 0.2f, originalColor.b + 0.2f);

        // initial phase - intro - moving to initial position before doing anything
        currentPhase = 0;

        // move body to starting position
        transform.position = new Vector3(0f, Balance.BossSpawnBounds.top, 0f);

        // velocity is based on the duration of the intro and how far above the screen the boss is set to spawn
        introVelocity = (postIntroPosition.y - Balance.BossSpawnBounds.top) / IntroDuration;

    }

    protected abstract void shootTimer_onFinish();
    protected abstract void Shoot();

    public virtual void CheckHealth() {
        print(health);
        if (health <= 0f) {
            Dead();
            return;
        }

        switch (currentPhase) {
            case 0:
                break;
            case 1:
                if (health <= initialHealth * 0.6f) {
                    AdvancePhase();
                }
                break;
            case 2:
                if (health <= initialHealth * 0.3f) {
                    AdvancePhase();
                }
                break;
        }
    }

    protected virtual void AdvancePhase() {
        currentPhase++;
    }

    public virtual void Dead() {
        shootTimer.Stop();
    }

    public virtual void Knockback() {
        StartCoroutine(KnockBackAndForth());
    }

    private IEnumerator KnockBackAndForth() {
        // Move back by knockback distance
        transform.Translate(new Vector3(0f, -Balance.ENEMY_BULLET_KNOCKBACK_DISTANCE, 0f));

        // Wait for several frames
        yield return new WaitForSeconds(Balance.DMG_FLASH_DURATION);

        // Move forward to regular position
        transform.Translate(new Vector3(0f, Balance.ENEMY_BULLET_KNOCKBACK_DISTANCE, 0f));
    }

    public virtual IEnumerator ColorFlash() {
        // GetComponent<SpriteRenderer>().color = Color.white;
        GetComponent<SpriteRenderer>().color = flashColor;
        yield return new WaitForSeconds(Balance.DMG_FLASH_DURATION);
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    // bosses shouldn't really have a shoot delay so this shouldn't really get called
    public virtual IEnumerator ShootDelay() {
        shootTimer.Stop();
        yield return new WaitForSeconds(Balance.DAMAGED_ENEMY_NEXT_SHOT_DELAY);
        shootTimer.Start();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (currentPhase == 0) return;
        if (other.transform.name == "PlayerBullet(Clone)")   {
            Bullet bullet = other.GetComponent<Bullet>();
            health -= bullet.Dmg_Value;
            GameManager.Instance.PlayerBulletReturnToPool(other.gameObject);

            CheckHealth();
            Knockback();
            StartCoroutine(ColorFlash());
        }
    }
}
