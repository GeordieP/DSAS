using UnityEngine;

public class BossBullet : Bullet {
    private void Start() {
        MOVE_SPEED = Balance.BOSS_BULLET_INITIAL_SPEED;
        dmg_value = Balance.BOSS_BULLET_BASE_DMG;
    }

    public override void SetType(int pickupTypeIndex) {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.EnemyBulletSprites[pickupTypeIndex];
    }

    public void SetRotation(float rotation) {
        transform.Rotate(new Vector3(0f, 0f, 1f), 270 + rotation);
    }
    
    private void FixedUpdate () {
        transform.Translate(moveDirection * MOVE_SPEED * Time.deltaTime, Space.World);

        if (
            transform.position.y > Balance.ScreenBounds.top ||
            transform.position.y < Balance.ScreenBounds.bottom ||
            transform.position.x < Balance.ScreenBounds.left ||
            transform.position.x > Balance.ScreenBounds.right
            ) {
            Despawn();
        }       
    }
}
