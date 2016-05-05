using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class Player : MonoBehaviour {
    private float health = Balance.PLAYER_INITIAL_HEALTH;
    private Color originalColor;

    void Start() {
        originalColor = GetComponent<SpriteRenderer>().color;
    }

    private void Dead() {
        Destroy(this);
        print("game over");
    }

    private IEnumerator ColorFlash() {
        GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(Balance.DMG_FLASH_DURATION);
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.name == "EnemyBullet(Clone)" || other.transform.name == "BossBullet(Clone)") {
            Bullet bullet = other.GetComponent<Bullet>();
            health -= bullet.Dmg_Value;
            StartCoroutine(ColorFlash());
            // if (health <= 0) Dead();
            GameManager.Instance.EnemyBulletReturnToPool(other.gameObject);
        }
    }
}
