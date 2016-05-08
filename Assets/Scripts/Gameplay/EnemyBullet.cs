using UnityEngine;


public class EnemyBullet : Bullet {
    void Start() {
        MOVE_SPEED = Balance.ENEMY_BULLET_INITIAL_SPEED;
        dmg_value = Balance.ENEMY_BULLET_BASE_DMG;
    }

    public void SetType(int enemyTypeIndex) {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.EnemyBulletSprites[enemyTypeIndex];        
    }

	void Update () {
	   transform.Translate(0f, MOVE_SPEED * Time.deltaTime, 0f);

        if (transform.position.y < Balance.ScreenBounds.bottom) {
            GameManager.Instance.EnemyBulletReturnToPool(gameObject);
        }        
	}
}
