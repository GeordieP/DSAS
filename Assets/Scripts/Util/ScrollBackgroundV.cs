using UnityEngine;
public class ScrollBackgroundV : MonoBehaviour {
    private Material mat;
    public float scrollSpeed = 1.0f;

	void Start () {
        mat = GetComponent<Renderer>().material;
	}
	
	void Update () {
        mat.mainTextureOffset = new Vector2(0f, Time.time * scrollSpeed);
	}
}
