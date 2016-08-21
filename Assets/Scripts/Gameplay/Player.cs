using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class Player : MonoBehaviour, IDamageable {

    // IDamageable properties
    public Color originalColor { get; set; }
    public float health { get; set; }
    public float initialHealth { get; set; }

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
        print(health);
        GetComponent<PlayerShoot>().Stop();
        if (health <= 0) Dead();
    }

    public void Dead() {
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
        yield return new WaitForSeconds(Balance.DMG_FLASH_DURATION);

        // Set color back to normal
        GetComponent<SpriteRenderer>().color = originalColor;
    }
    
    public IEnumerator ShootDelay() {
        GetComponent<PlayerShoot>().Shooting = false;
        yield return new WaitForSeconds(Balance.DAMAGED_PLAYER_NEXT_SHOT_DELAY);
        GetComponent<PlayerShoot>().Shooting = true;
    }

    /*---
    * Player specific methods
    ---*/
    
    private IEnumerator KnockBackAndForth() {
        // Move back by knockback distance
        transform.Translate(new Vector3(0f, -Balance.ENEMY_BULLET_KNOCKBACK_DISTANCE, 0f));

        // Wait for several frames
        yield return new WaitForSeconds(Balance.DMG_FLASH_DURATION);

        // Move forward to regular position
        transform.Translate(new Vector3(0f, Balance.ENEMY_BULLET_KNOCKBACK_DISTANCE, 0f));
    }

    private void HitByBullet(float bullet_dmg) {
        health -= bullet_dmg;
        GameManager.Instance.UpdateHealthBar(health / initialHealth);
        CheckHealth();
        Knockback();
        StartCoroutine(ShootDelay());
        StartCoroutine(ColorFlash());
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.name == "EnemyBullet(Clone)") {
            Bullet bullet = other.GetComponent<Bullet>();
            HitByBullet(bullet.Dmg_Value);
            GameManager.Instance.EnemyBulletReturnToPool(other.gameObject);
        } else if (other.transform.name == "BossBullet(Clone)") {
            Bullet bullet = other.GetComponent<Bullet>();            
            HitByBullet(bullet.Dmg_Value);            
            GameManager.Instance.BossBulletReturnToPool(other.gameObject);
        }
    }
}
