using UnityEngine;

public class Bullet : MonoBehaviour {
    private const float MOVE_SPEED = 40;
    private Vector2 move_direction = Vector2.up;
	
	void Update () {
        transform.Translate(0f, MOVE_SPEED * Time.deltaTime, 0f);
        if (transform.position.y > 10) {
            Destroy(gameObject);
        }
	}
}
