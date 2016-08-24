using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class Player : MonoBehaviour, IDamageable {

    // IDamageable properties
    public Color originalColor { get; set; }
    public float health { get; set; }
    public float initialHealth { get; set; }

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

        GameManager.Instance.UpdateHealthBar(health / initialHealth);
        StartCoroutine(ShootDelay());
        StartCoroutine(ColorFlash());
        Knockback();
        CheckHealth();
    }

    public void GotPowerup(Powerup powerup) {
        
    }
}
