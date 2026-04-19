using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
    }

    void LateUpdate()
    {
        if (_cam == null) return;

        Quaternion targetRot = Quaternion.LookRotation(
            transform.position - _cam.transform.position
        );

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
    }
}