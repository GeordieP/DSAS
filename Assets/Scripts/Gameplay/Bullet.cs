using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Bullet : PooledEntity {
    protected float MOVE_SPEED = Balance.BULLET_INITIAL_SPEED;
    protected float dmg_value = Balance.BULLET_BASE_DMG;
    public float Dmg_Value { get { return dmg_value; } }
    protected Vector3 moveDirection = Vector3.down;

    public virtual void Spawn(Vector3 shooterPosition) {
        transform.position = shooterPosition + new Vector3(0f, 0f, Balance.BULLET_Z_POSITION);
    }

    public virtual void Spawn(Vector3 shooterPosition, Vector3 moveDirection) {
        transform.position = shooterPosition + new Vector3(0f, 0f, Balance.BULLET_Z_POSITION);
        this.moveDirection = moveDirection;
    }
}
