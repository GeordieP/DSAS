using UnityEngine;

public class PlayerBullet : Bullet {
	private void Start() {
        MOVE_SPEED = Balance.PLAYER_BULLET_INITIAL_SPEED;
        dmg_value = Balance.PLAYER_BULLET_BASE_DMG;
	}

    public void SetRotation(float rotation) {
        transform.Rotate(new Vector3(0f, 0f, 1f), 270 + rotation);
    }

    public override void SetType(int pickupTypeIndex) {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.PlayerBulletSprites[pickupTypeIndex];
    }
	
	private void FixedUpdate() {
        // transform.Translate(0f, MOVE_SPEED * Time.deltaTime, 0f, Space.World);
        transform.Translate(moveDirection * MOVE_SPEED * Time.deltaTime, Space.World);
        
        if (transform.position.y > Balance.DespawnBounds.top) {
            Despawn();
        }
    }
}
