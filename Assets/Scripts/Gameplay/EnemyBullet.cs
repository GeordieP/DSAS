using UnityEngine;


public class EnemyBullet : Bullet {
    private void Start() {
        MOVE_SPEED = Balance.ENEMY_BULLET_INITIAL_SPEED;
        dmg_value = Balance.ENEMY_BULLET_BASE_DMG;
    }

    public override void SetType(int enemyTypeIndex) {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.EnemyBulletSprites[enemyTypeIndex];        
        if (Balance.ENEMY_TYPES_BASE_DMG[enemyTypeIndex] + Balance.ENEMY_DMG_SCALING_PER_STAGE * GameManager.Instance.Stage < Balance.ENEMY_MAX_SHOT_DMG)
            dmg_value = Balance.ENEMY_TYPES_BASE_DMG[enemyTypeIndex] + Balance.ENEMY_DMG_SCALING_PER_STAGE * GameManager.Instance.Stage;
    }

    public void SetRotation(float rotation) {
        transform.Rotate(new Vector3(0f, 0f, 1f), 270 + rotation);
    }

    public override void Despawn() {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        base.Despawn();
    }

	private void FixedUpdate () {
        transform.Translate(moveDirection * MOVE_SPEED * Time.deltaTime, Space.World);

        if (transform.position.y < Balance.ScreenBounds.bottom || transform.position.x < Balance.ScreenBounds.left || transform.position.x > Balance.ScreenBounds.right) {
            Despawn();
        }       
	}
}
