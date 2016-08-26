﻿using UnityEngine;
using System.Collections;

public class PowerupEffect {
    public float? health, shield, bulletScaling, shipScaling;
    public Powerup.PlayerShootPatternDelegate shootPattern;
}

public class Powerup : PooledEntity {
    public delegate void PlayerShootPatternDelegate(Vector3 origin, GameObjectPool pool);
    private float MOVE_SPEED = Balance.POWERUP_BASE_MOVE_SPEED;

    private int powerupType;
    private PowerupEffect effect;

/*need a timer to un-do these effects when they should wear off
  effects should probably have their duration bundled with them
*/

    public void Spawn() {
        transform.position = new Vector3(Random.Range(Balance.SpawnBounds.left, Balance.SpawnBounds.right), Balance.DespawnBounds.top, 0f);
        RandomizeType();
    }

    public void RandomizeType() {
        powerupType = Random.Range(0, PowerupEffects.EffectTypes.Length);

        /* need powerup sprites.... for now use enemy sprites*/
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.EnemySprites[powerupType];
        effect = PowerupEffects.EffectTypes[powerupType];

    }

	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector3(0f, -MOVE_SPEED * Time.deltaTime, 0f));
	}

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            other.GetComponent<Player>().GotPowerup(effect);
            Despawn();
        }
    }
}

public static class PowerupEffects {
    private static PowerupEffect[] _effectTypes = {

        // health effect
        new PowerupEffect {
            health = Balance.PLAYER_INITIAL_HEALTH,
            shield = null,
            bulletScaling = null,
            shipScaling = null,
            shootPattern = null
        },

        // shield effect
        new PowerupEffect {
            health = null,
            shield = 200f,
            bulletScaling = null,
            shipScaling = null,
            shootPattern = null
        },

        // 2x bullet scale effect
        new PowerupEffect {
            health = null,
            shield = null,
            bulletScaling = 2f,
            shipScaling = null,
            shootPattern = null
        },

        // half size ship effect
        new PowerupEffect {
            health = null,
            shield = null,
            bulletScaling = null,
            shipScaling = 0.25f,
            shootPattern = null
        },

        // different shoot pattern effect
        new PowerupEffect {
            health = null,
            shield = null,
            bulletScaling = null,
            shipScaling = null,
            shootPattern = TriFanUp_Shoot
        }
    };

    public static void TriFanUp_Shoot(Vector3 origin, GameObjectPool bulletPool) {
        GameObject[] bullets = bulletPool.Borrow(3);
        PlayerBullet currentBullet;

        for (int i = 0; i < 3; i++) {
            currentBullet = bullets[i].GetComponent<PlayerBullet>();
            float angle = 75 + 15 * i;
            currentBullet.SetRotation(-angle);

            Vector3 moveDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
            currentBullet.Spawn(origin, moveDirection);
            bullets[i].SetActive(true);
        }
    }

    public static PowerupEffect[] EffectTypes {
        get { return _effectTypes; }
    }
}