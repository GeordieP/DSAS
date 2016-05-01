using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour {
    public void Start() {
        Activate();
    }

    public void Activate() {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GetRandomEnemySprite();
    }
}
