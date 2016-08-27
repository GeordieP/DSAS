using UnityEngine;

public class PlayerBullet : Bullet {
	private void Start() {
        moveDirection = Vector3.up;
        MOVE_SPEED = Balance.PLAYER_BULLET_INITIAL_SPEED;
        dmg_value = Balance.PLAYER_BULLET_BASE_DMG;
	}

    public void SetRotation(float rotation) {
        transform.Rotate(new Vector3(0f, 0f, 1f), 270 + rotation);
    }

    public override void SetType(int typeIndex) {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.PlayerBulletSprites[typeIndex];
    }

    public override void Despawn() {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        base.Despawn();
    }
	
	private void FixedUpdate() {
        transform.Translate(moveDirection * MOVE_SPEED * Time.deltaTime, Space.World);
        
        if (transform.position.y > Balance.DespawnBounds.top) {
            Despawn();
        }
    }
}
