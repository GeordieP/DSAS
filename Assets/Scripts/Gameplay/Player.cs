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
        StartCoroutine(ColorFlash());
        if (health <= 0) Dead();
    }

    private IEnumerator ColorFlash() {
        GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(Balance.DMG_FLASH_DURATION);
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.name == "EnemyBullet(Clone)" || other.transform.name == "BossBullet(Clone)") {
            Bullet bullet = other.GetComponent<Bullet>();
            UpdateHealth(health - bullet.Dmg_Value);
            GameManager.Instance.EnemyBulletReturnToPool(other.gameObject);
        }
    }
}
