using UnityEngine;
using System.Collections;

public abstract class Boss : MonoBehaviour, IDamageable {
    protected byte currentPhase;

    // Intro
    protected bool introMoveComplete, introRotationComplete;
    public const float IntroDuration = 4f;        // seconds
    protected float introVelocity;
    protected Vector3 postIntroPosition = new Vector3(0f, 3f, 0f);

    // Score
    protected int scoreValue;

    // Object Pooling
    protected GameObjectPool bossBulletPool;

    // Timers
    protected Timer shootTimer;

    // Shooting
    protected const float timeBetweenShots = 0.2f;

    private bool alive;

    // IDamageable properties as well as related flashColor 
    public Color originalColor { get; set; }
    public Color flashColor;
    public float initialHealth { get; set; }
    public float health { get; set; }

    public virtual void Spawn() {
        originalColor = GetComponent<SpriteRenderer>().color;
        flashColor = new Color(originalColor.r + 0.2f, originalColor.g + 0.2f, originalColor.b + 0.2f);

        // initial phase - intro - moving to initial position before doing anything
        currentPhase = 0;

        // move body to starting position
        transform.position = new Vector3(0f, Balance.BossSpawnBounds.top, 0f);

        // velocity is based on the duration of the intro and how far above the screen the boss is set to spawn
        introVelocity = (postIntroPosition.y - Balance.BossSpawnBounds.top) / IntroDuration;

        // set up the shoot timer, attach it to shoot method
        bossBulletPool = GameManager.Instance.BossBulletPool;

        alive = true;
    }

    // Shoot implementation will be different in every boss type
    protected abstract void Shoot();
    protected virtual void shootTimer_onFinish() {
        Shoot();
    }

    /*---
    * Implement IDamageable members
    ---*/    

    public virtual void CheckHealth() {
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

    public virtual void Dead() {
        alive = false;
        shootTimer.Stop();
        GameManager.Instance.AddScore(scoreValue);
        transform.position = new Vector3(0f, Balance.BossSpawnBounds.top, 0f);
        gameObject.SetActive(false);

        GameManager.Instance.SetEnemySpawnEnabled(true);
    }

    public virtual void Knockback() {
        StartCoroutine(KnockBackAndForth());
    }

    public virtual IEnumerator ColorFlash() {
        GetComponent<SpriteRenderer>().color = flashColor;
        yield return new WaitForSeconds(Balance.DMG_FLASH_DURATION);
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    // bosses shouldn't really have a shoot delay so this shouldn't really get called
    // but we need to implement it as is required by the IDamageable interface
    public virtual IEnumerator ShootDelay() {
        yield return new WaitForEndOfFrame();
    }

    /*---
    * Generic Boss related methods
    ---*/
    
    protected virtual void AdvancePhase() {
        if (currentPhase == 0) GameManager.Instance.SetPlayerShoot(true);   // start the player shooting again once the intro is complete
        currentPhase++;
    }

    // Enemies get knocked back, but bosses should snap back to their 
    private IEnumerator KnockBackAndForth() {
        // Move back by knockback distance
        transform.Translate(new Vector3(0f, -Balance.ENEMY_BULLET_KNOCKBACK_DISTANCE, 0f), Space.World);

        // Wait for several frames
        yield return new WaitForSeconds(Balance.DMG_FLASH_DURATION);

        // Move forward to regular position
        transform.Translate(new Vector3(0f, Balance.ENEMY_BULLET_KNOCKBACK_DISTANCE, 0f), Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // don't take damage in intro phase
        if (currentPhase == 0) return;

        // hit by player bullets
        if (other.transform.name == "PlayerBullet(Clone)")   {
            if (!alive) return;

            Bullet bullet = other.GetComponent<Bullet>();
            health -= bullet.Dmg_Value;
            bullet.Despawn();

            CheckHealth();
            Knockback();
            StartCoroutine(ColorFlash());
        }
    }
}
