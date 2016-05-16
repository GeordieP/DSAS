using UnityEngine;


public class EnemyBullet : Bullet {
    private void Start() {
        MOVE_SPEED = Balance.ENEMY_BULLET_INITIAL_SPEED;
        dmg_value = Balance.ENEMY_BULLET_BASE_DMG;
    }

    public void SetType(int enemyTypeIndex) {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.EnemyBulletSprites[enemyTypeIndex];        
    }

	private void FixedUpdate () {
        // transform.Translate(0f, MOVE_SPEED * Time.deltaTime, 0f);
        transform.Translate(moveDirection * MOVE_SPEED * Time.deltaTime);

        if (transform.position.y < Balance.ScreenBounds.bottom || transform.position.x < Balance.ScreenBounds.left || transform.position.x > Balance.ScreenBounds.right) {
            GameManager.Instance.EnemyBulletReturnToPool(gameObject);
        }       
	}
}
