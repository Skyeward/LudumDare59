using UnityEngine;

public class LocalSpin : MonoBehaviour
{
    [Header("Main Rotation")]
    public float yRotationSpeed = 30f;

    private float baseY;
    private float timeOffset;

    void Start()
    {
        // Random starting Y rotation
        baseY = Random.Range(0f, 360f);
        yRotationSpeed *= Random.Range(0.6f, 1.4f);

        // Optional: desync wobble too (prevents identical motion)
        timeOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float t = Time.time + timeOffset;

        float y = baseY + t * yRotationSpeed;

        transform.localRotation = Quaternion.Euler(0, y, 0);
    }
}