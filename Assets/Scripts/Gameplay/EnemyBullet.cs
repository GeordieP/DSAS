using UnityEngine;


public class EnemyBullet : Bullet {
    private int enemyType;

    void Start() {
        MOVE_SPEED = -30f;
    }

    public void SetType(int enemyTypeIndex) {
        enemyType = enemyTypeIndex;
        // GetComponent<SpriteRenderer>().sprite = GameManager.Instance.EnemyBulletSprites[enemyTypeIndex];        
    }

	void Update () {
	   transform.Translate(0f, MOVE_SPEED * Time.deltaTime, 0f);

        if (transform.position.y < -5) {
            GameManager.Instance.BulletReturnToPool(gameObject);
        }        
	}
}
