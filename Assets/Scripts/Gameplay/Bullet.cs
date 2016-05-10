using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Bullet : MonoBehaviour {
    protected float MOVE_SPEED = Balance.BULLET_INITIAL_SPEED;
    protected float dmg_value = Balance.BULLET_BASE_DMG;
    public float Dmg_Value { get { return dmg_value; } }
    protected Vector3 moveDirection = Vector3.down;

    public virtual void Spawn(Transform shooterTransform) {
        transform.position = shooterTransform.position;
    }

    public virtual void Spawn(Transform shooterTransform, Vector3 moveDirection) {
        transform.position = shooterTransform.position;
        this.moveDirection = moveDirection;
    }

    public virtual void Despawn() {
        transform.position = new Vector3(-50f, -50f, -50f);
    }
}
