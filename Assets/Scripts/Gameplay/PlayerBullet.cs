using UnityEngine;

public class PlayerBullet : Bullet {
	void Start () {
        MOVE_SPEED = Balance.PLAYER_BULLET_INITIAL_SPEED;
        dmg_value = Balance.PLAYER_BULLET_BASE_DMG;
	}

    public void SetType(int pickupTypeIndex) {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.PlayerBulletSprites[pickupTypeIndex];
    }
	
	void Update () {
        transform.Translate(0f, MOVE_SPEED * Time.deltaTime, 0f);
        if (transform.position.y > Balance.ScreenBounds.top) {
            GameManager.Instance.PlayerBulletReturnToPool(gameObject);
        }
    }
}
