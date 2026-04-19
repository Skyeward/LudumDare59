using UnityEngine;

public class DashScroll : MonoBehaviour
{
    public float speed = 2f;

    private LineRenderer lr;
    private Material mat;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();

        lr.textureMode = LineTextureMode.Tile;

        mat = Instantiate(lr.material);
        lr.material = mat;

        mat.mainTextureScale = new Vector2(10f, 1f);
    }

    void Update()
    {
        mat.mainTextureScale = new Vector2(10f, 1f);

        float offset = (Time.time * speed) % 1f;
        mat.mainTextureOffset = new Vector2(offset, 0f);
    }
}