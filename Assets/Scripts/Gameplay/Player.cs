﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CircleCollider2D))]
public class Player : MonoBehaviour, IDamageable {

    // IDamageable properties
    public Color originalColor { get; set; }
    public float initialHealth { get; set; }
    
    // hold active powerup duation timers and post-effect values to restore
    private Dictionary<string, Timer> powerupEffectDurations;
    private Dictionary<string, PowerupEffect> powerupEffectRestoreValues;
    // private List<Timer> powerupEffectDurations;
    // private List<PowerupEffect> powerupEffectRestoreValues;

    private float _health;
    public float health {
        get { return _health; }
        set {
            _health = value;
            GameManager.Instance.UpdateHealthBar(_health / initialHealth);
        }
    }

    public float BulletScaling {
        get { return PlayerBullet._bulletScaling; }
        set {
            PlayerBullet._bulletScaling = value;
        }
    }

    public float ShipScaling {
        get { return transform.localScale.x; }
        set {
            transform.localScale = Vector3.one * value;
        }
    }

    public Powerup.PlayerShootPatternDelegate ShootPattern {
        get { return GetComponent<PlayerShoot>().shootPattern; }
        set {
            GetComponent<PlayerShoot>().shootPattern = value;
        }
    }

	// Shoot Delay WaitForSeconds
	private static WaitForSeconds PLAYER_NEXT_SHOT_DELAY_WAITFORSECONDS = new WaitForSeconds(Balance.DAMAGED_PLAYER_NEXT_SHOT_DELAY);

	private void Start() {
        initialHealth = Balance.PLAYER_INITIAL_HEALTH;
        health = initialHealth;

        originalColor = GetComponent<SpriteRenderer>().color;
        GameManager.Instance.UpdateHealthBar(health / initialHealth);

        powerupEffectDurations = new Dictionary<string, Timer>();
        powerupEffectRestoreValues = new Dictionary<string, PowerupEffect>();

        BulletScaling = 0.5f;       // set default bullet scaling
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
    // Set up post-effect restore values (if applicable) and start restore timer
    public void GotPowerup(PowerupEffect effect) {
        // initial value to the restore effect. Should get overwritten by another one if an effect is happening
            // otherwise just here as a fallback
        PowerupEffect restoreValuesEffect = new PowerupEffect {
            bulletScaling = BulletScaling
        };

        string effectType = "";

        if (effect.health != null) {
            health = (float)effect.health;
        }
    
        // if (effect.shield != null) shield = (float)effect.shield;        // TOOD: implement shield
    
        if (effect.bulletScaling != null) {
            restoreValuesEffect = new PowerupEffect {
                bulletScaling = BulletScaling
            };

            effectType = "BulletScaling";
            BulletScaling = (float)effect.bulletScaling;
        }

        if (effect.shipScaling != null) {
            restoreValuesEffect = new PowerupEffect {
                shipScaling = ShipScaling
            };

            effectType = "ShipScaling";
            ShipScaling = (float)effect.shipScaling;
        }

        if (effect.shootPattern != null) {
            restoreValuesEffect = new PowerupEffect {
                shootPattern = ShootPattern
            };

            effectType = "ShootPattern";
            ShootPattern = effect.shootPattern;
        }

        if (effect.duration != null) {
            Timer effectTimer = TimerManager.Instance.CreateTimerOneshot((float)effect.duration);

            effectTimer.onFinish += () => { 
                GotPowerup(restoreValuesEffect);      // apply the values from the restoreValuesEffect to return to a 'normal' state

                powerupEffectRestoreValues.Remove(effectType);
                powerupEffectDurations[effectType].Stop();
                powerupEffectDurations.Remove(effectType);
            };

            effectTimer.Start();

            // add or update the timer for this effect type
            if (powerupEffectDurations.ContainsKey(effectType)) {
                powerupEffectDurations[effectType].Stop();
                powerupEffectDurations[effectType] = effectTimer;
            } else {
                powerupEffectDurations.Add(effectType, effectTimer);
            }

            // add or update the restore value effect struct for this effect type
            if (powerupEffectRestoreValues.ContainsKey(effectType)) {
                powerupEffectRestoreValues[effectType] = restoreValuesEffect;
            } else {
                powerupEffectRestoreValues.Add(effectType, restoreValuesEffect);
            }
        }
    }
}
