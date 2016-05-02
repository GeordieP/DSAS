using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour {

    public void Start() {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GetRandomEnemySprite();
    }

    void Update() {
        transform.Translate(new Vector3(0f, -2 * Time.deltaTime, 0f));
        if (transform.position.y < -5) {
            GameManager.Instance.EnemyReturnToPool(gameObject);
        }
    }

    public void Spawn() {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GetRandomEnemySprite();
        transform.position = new Vector3(0f, 5.0f, 0f);
        print("new pos: " + transform.position);
    }

    public void Despawn() {
        transform.position = new Vector3(-50f, -50f, 0f);
    }
}
