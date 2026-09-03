using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Referencias")]
    public Transform cameraPivot;
    public Transform player;

    [Header("Configuración")]
    public float sensitivity = 220f;
    public float minPitch = -30f;
    public float maxPitch = 65f;

    float yaw;
    float pitch = 15f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = player.eulerAngles.y;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        yaw += mouseX;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        player.rotation = Quaternion.Euler(0, yaw, 0);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0, 0);
    }
}