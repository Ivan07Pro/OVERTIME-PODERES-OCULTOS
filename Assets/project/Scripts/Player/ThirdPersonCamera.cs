using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;

    public float distance = 6f;
    public float height = 3f;

    public float mouseSensitivity = 3f;

    private float yaw = 0f;
    private float pitch = 15f;

    void LateUpdate()
    {
        if (target == null)
            return;

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -10f, 60f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 position =
            target.position
            - rotation * Vector3.forward * distance
            + Vector3.up * height;

        transform.position = position;

        transform.LookAt(target.position + Vector3.up * 1.2f);
    }
}