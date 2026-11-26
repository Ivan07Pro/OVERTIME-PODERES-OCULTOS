using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Cámara en tercera persona con "pivote": el ratón gira al jugador (izquierda/derecha)
/// y al pivote de la cámara (arriba/abajo). La cámara debe ser hija de cameraPivot.
/// </summary>
public class PlayerLook : MonoBehaviour
{
    [Header("Referencias")]
    public Transform cameraPivot;
    public Transform player;
    [Tooltip("Si se deja vacío se busca automáticamente.")]
    public PlayerInputHandler input;

    [Header("Configuración")]
    public float minPitch = -30f;
    public float maxPitch = 65f;

    float yaw;
    float pitch = 15f;
    bool initialized;

    void Start()
    {
        if (player == null || cameraPivot == null)
        {
            Debug.LogWarning(
                $"PlayerLook en '{name}': falta asignar 'Player' o 'Camera Pivot' en el Inspector. " +
                "Este componente se desactiva.", this);
            enabled = false;
            return;
        }

        if (input == null)
            input = PlayerInputHandler.FindFor(player);

        yaw = player.eulerAngles.y;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        LockCursor(true);
        initialized = true;
    }

    void Update()
    {
        // Escape libera el ratón; clic izquierdo lo vuelve a bloquear.
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            LockCursor(false);
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame
                 && Cursor.lockState != CursorLockMode.Locked)
            LockCursor(true);

        Vector2 look = input.Look; // ya viene en grados para este frame

        yaw += look.x;
        pitch -= look.y;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        player.rotation = Quaternion.Euler(0f, yaw, 0f);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void OnDisable()
    {
        // Solo soltamos el ratón si este componente fue el que lo bloqueó.
        if (initialized)
            LockCursor(false);
    }

    static void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
