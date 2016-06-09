using UnityEngine;


public class EnemyBullet : Bullet {
    private void Start() {
        MOVE_SPEED = Balance.ENEMY_BULLET_INITIAL_SPEED;
        dmg_value = Balance.ENEMY_BULLET_BASE_DMG;
    }

    public void SetType(int enemyTypeIndex) {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.EnemyBulletSprites[enemyTypeIndex];        
    }

    public void SetRotation(float rotation) {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        if (rotation != 0)
            transform.Rotate(new Vector3(0f, 0f, 1f), 270 + rotation);
    }

	private void FixedUpdate () {
        transform.Translate(moveDirection * MOVE_SPEED * Time.deltaTime, Space.World);

        if (transform.position.y < Balance.ScreenBounds.bottom || transform.position.x < Balance.ScreenBounds.left || transform.position.x > Balance.ScreenBounds.right) {
            GameManager.Instance.EnemyBulletReturnToPool(gameObject);
        }       
	}
}
