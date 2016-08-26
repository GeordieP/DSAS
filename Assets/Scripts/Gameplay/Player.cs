using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class Player : MonoBehaviour, IDamageable {

    // IDamageable properties
    public Color originalColor { get; set; }
    public float initialHealth { get; set; }
    
    private float _health;
    public float health {
        get { return _health; }
        set {
            _health = value;
            GameManager.Instance.UpdateHealthBar(_health / initialHealth);
        }
    }

    private float _bulletScaling;
    public float BulletScaling {
        get { return _bulletScaling; }
        set {
            _bulletScaling = value;
            // TOOD: set bullet scaling in EnemyBullet
        }
    }

    private float _shipScaling;
    public float ShipScaling {
        get { return _shipScaling; }
        set {
            print("setting ship scale " + value);
            _shipScaling = value;
            transform.localScale = Vector3.one * _shipScaling;
        }
    }

    private Powerup.PlayerShootPatternDelegate _shootPattern;
    public Powerup.PlayerShootPatternDelegate ShootPattern {
        get { return _shootPattern; }
        set {
            _shootPattern = value;
            GetComponent<PlayerShoot>().shootPattern = _shootPattern;
        }
    }

	// Shoot Delay WaitForSeconds
	private static WaitForSeconds PLAYER_NEXT_SHOT_DELAY_WAITFORSECONDS = new WaitForSeconds(Balance.DAMAGED_PLAYER_NEXT_SHOT_DELAY);

	private void Start() {
        initialHealth = Balance.PLAYER_INITIAL_HEALTH;
        health = initialHealth;

        originalColor = GetComponent<SpriteRenderer>().color;
        GameManager.Instance.UpdateHealthBar(health / initialHealth);
    }

    /*---
    * Implement IDamageable members
    ---*/

    public void CheckHealth() {
        if (health <= 0) Dead();
    }

    public void Dead() {
        GameManager.Instance.UpdateHealthBar(health / initialHealth);
        GetComponent<PlayerShoot>().Shooting = false;
        Destroy(gameObject);
        print("game over");     // TOOD: a real game over
    }

    public void Knockback() {
        StartCoroutine(KnockBackAndForth());
    }

    public IEnumerator ColorFlash() {
        // Set sprite color to white
        GetComponent<SpriteRenderer>().color = Color.white;

		// Wait for several frames
		yield return Balance.DMG_FLASH_WAITFORSECONDS;

        // Set color back to normal
        GetComponent<SpriteRenderer>().color = originalColor;
    }

	public IEnumerator ShootDelay() {
        GetComponent<PlayerShoot>().Shooting = false;
		yield return PLAYER_NEXT_SHOT_DELAY_WAITFORSECONDS;
		GetComponent<PlayerShoot>().Shooting = true;
    }

    /*---
    * Player specific methods
    ---*/
    
    private IEnumerator KnockBackAndForth() {
        // Move back by knockback distance
        transform.Translate(new Vector3(0f, -Balance.ENEMY_BULLET_KNOCKBACK_DISTANCE, 0f));

        // Wait for several frames
        yield return Balance.DMG_FLASH_WAITFORSECONDS;

        // Move forward to regular position
        transform.Translate(new Vector3(0f, Balance.ENEMY_BULLET_KNOCKBACK_DISTANCE, 0f));
    }

    public void HitByBullet(Bullet bullet) {
        if (bullet.tag == "PlayerBullet") return;
        
        health -= bullet.Dmg_Value;
        bullet.Despawn();

        StartCoroutine(ShootDelay());
        StartCoroutine(ColorFlash());
        Knockback();
        CheckHealth();
    }

    // Perform null checks and set values if effect's is not null
    public void GotPowerup(PowerupEffect effect) {
        if (effect.health != null) {
            print("got health effect");
            health = (float)effect.health;
        }
    
        // if (effect.shield != null) shield = (float)effect.shield;        // TOOD: implement shield
    
        if (effect.bulletScaling != null) {
            BulletScaling = (float)effect.bulletScaling;
        }
        if (effect.shipScaling != null) {
            ShipScaling = (float)effect.shipScaling;
        }
        if (effect.shootPattern != null) {
            ShootPattern = effect.shootPattern;
        }
    }
}
