using UnityEngine;

public class DashScroll : MonoBehaviour
{
    public float speed = 2f;
    private Material mat;

    void Start()
    {
        mat = GetComponent<LineRenderer>().material;
    }

    void Update()
    {
        float offset = Time.time * speed;
        mat.mainTextureOffset = new Vector2(offset, 0);
    }
}