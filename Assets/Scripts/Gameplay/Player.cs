using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class Player : MonoBehaviour {
    private float health = Balance.PLAYER_INITIAL_HEALTH;

    private void Dead() {
        Destroy(this);
        print("game over");
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.name == "EnemyBullet(Clone)" || other.transform.name == "BossBullet(Clone)") {
            Bullet bullet = other.GetComponent<Bullet>();
            health -= bullet.Dmg_Value;
            if (health <= 0) Dead();
            GameManager.Instance.EnemyBulletReturnToPool(other.gameObject);
        }
    }
}
