using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class Player : MonoBehaviour {
    private float health = Balance.PLAYER_INITIAL_HEALTH;
    private const float INITIAL_HEALTH = Balance.PLAYER_INITIAL_HEALTH;
    private Color originalColor;

    private void Start() {
        originalColor = GetComponent<SpriteRenderer>().color;
        GameManager.Instance.UpdateHealthBar(health / INITIAL_HEALTH);
    }

    private void Dead() {
        Destroy(this);
        print("game over");
    }

    private void UpdateHealth(float newHealth) {
        health = newHealth;
        GameManager.Instance.UpdateHealthBar(health / INITIAL_HEALTH);
        StartCoroutine(HitByBullet());
        StartCoroutine(ShootDelay());
        if (health <= 0) Dead();
    }

    private IEnumerator ShootDelay() {
        GetComponent<PlayerShoot>().Shooting = false;
        yield return new WaitForSeconds(Balance.DAMAGED_PLAYER_NEXT_SHOT_DELAY);
        GetComponent<PlayerShoot>().Shooting = true;
    }

    private IEnumerator HitByBullet() {
        // Set sprite color to white
        GetComponent<SpriteRenderer>().color = Color.white;
        // Move back by knockback distance
        transform.Translate(new Vector3(0f, -Balance.ENEMY_BULLET_KNOCKBACK_DISTANCE, 0f));

        // Wait for several frames
        yield return new WaitForSeconds(Balance.DMG_FLASH_DURATION);

        // Move forward to regular position
        transform.Translate(new Vector3(0f, Balance.ENEMY_BULLET_KNOCKBACK_DISTANCE, 0f));
        // Set color back to normal
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.name == "EnemyBullet(Clone)") {
            Bullet bullet = other.GetComponent<Bullet>();
            UpdateHealth(health - bullet.Dmg_Value);
            GameManager.Instance.EnemyBulletReturnToPool(other.gameObject);
        } else if (other.transform.name == "BossBullet(Clone)") {
            Bullet bullet = other.GetComponent<Bullet>();
            UpdateHealth(health - bullet.Dmg_Value);
            GameManager.Instance.BossBulletReturnToPool(other.gameObject);
        }
    }
}
