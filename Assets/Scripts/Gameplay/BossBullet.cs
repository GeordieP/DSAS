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
            transform.position.y > Balance.DespawnBounds.top ||
            transform.position.y < Balance.DespawnBounds.bottom ||
            transform.position.x < Balance.DespawnBounds.left ||
            transform.position.x > Balance.DespawnBounds.right
            ) {
            Despawn();
        }       
    }
}
