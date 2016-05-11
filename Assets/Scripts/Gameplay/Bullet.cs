using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Bullet : PooledEntity {
    protected float MOVE_SPEED = Balance.BULLET_INITIAL_SPEED;
    protected float dmg_value = Balance.BULLET_BASE_DMG;
    public float Dmg_Value { get { return dmg_value; } }
    protected Vector3 moveDirection = Vector3.down;

    public virtual void Spawn(Vector3 shooterPosition) {
        transform.position = shooterPosition;
    }

    public virtual void Spawn(Vector3 shooterPosition, Vector3 moveDirection) {
        transform.position = shooterPosition;
        this.moveDirection = moveDirection;
    }
}
