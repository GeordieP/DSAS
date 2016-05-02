using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Bullet : MonoBehaviour {
    protected float MOVE_SPEED = 40;

    public virtual void Spawn(Transform shooterTransform) {
        transform.position = shooterTransform.position;
    }

    public virtual void Despawn() {
        transform.position = new Vector3(-50f, -50f, -50f);
    }
}
