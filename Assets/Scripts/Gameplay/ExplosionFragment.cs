using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ExplosionFragment : PooledEntity {
    private Vector3 moveDirection;
    private float MOVE_SPEED;

	private void Start () {
        // moveDirection = Vector2.up;
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.ExplosionFragmentSprites[Random.Range(0, GameManager.Instance.ExplosionFragmentSprites.Length)];
	}

    public void Spawn(Vector3 origin) {
        transform.position = origin;
        MOVE_SPEED = Random.Range(3f, 15f);
        moveDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.4f, 1f), 0f);
    }

    public void Spawn(Vector3 origin, Vector3 directionInfluence) {
        throw new System.NotImplementedException();
    }

    public void Spawn(Vector3 origin, Vector3 directionInfluence, Vector3 moveDirection) {
        throw new System.NotImplementedException();
    }

    public override void Despawn() {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.ExplosionFragmentSprites[Random.Range(0, GameManager.Instance.ExplosionFragmentSprites.Length)];
    }

	private void FixedUpdate () {
        transform.Translate(moveDirection * MOVE_SPEED * Time.deltaTime);

        if (transform.position.y > Balance.ScreenBounds.top || transform.position.y < Balance.ScreenBounds.bottom || transform.position.x < Balance.ScreenBounds.left || transform.position.x > Balance.ScreenBounds.right) {
            GameManager.Instance.ExplosionFragmentReturnToPool(gameObject);
        }
	}
}
