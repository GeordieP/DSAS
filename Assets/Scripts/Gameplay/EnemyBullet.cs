using UnityEngine;


public class EnemyBullet : Bullet {
    void Start() {
        MOVE_SPEED = Balance.ENEMY_BULLET_INITIAL_SPEED;
    }

    public void SetType(int enemyTypeIndex) {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.EnemyBulletSprites[enemyTypeIndex];        
    }

	void Update () {
	   transform.Translate(0f, MOVE_SPEED * Time.deltaTime, 0f);

        if (transform.position.y < -5) {
            GameManager.Instance.EnemyBulletReturnToPool(gameObject);
        }        
	}
}
