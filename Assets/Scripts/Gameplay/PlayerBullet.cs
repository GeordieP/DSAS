using UnityEngine;

public class PlayerBullet : Bullet {
	void Start () {
        MOVE_SPEED = 40f;
	}

    public void SetType(int pickupTypeIndex) {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.PlayerBulletSprites[pickupTypeIndex];
    }
	
	void Update () {
        transform.Translate(0f, MOVE_SPEED * Time.deltaTime, 0f);
        if (transform.position.y > 10) {
            GameManager.Instance.PlayerBulletReturnToPool(gameObject);
        }
	}
}
