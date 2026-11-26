using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Lee todos los controles del jugador con el Input System nuevo
/// (teclado + ratón y mando). Los demás scripts leen de aquí.
/// Se ejecuta antes que los demás para que lean el input de este mismo frame.
/// </summary>
[DefaultExecutionOrder(-100)]
public class PlayerInputHandler : MonoBehaviour
{
    [Header("Sensibilidad de cámara")]
    [Tooltip("Grados por píxel que se mueve el ratón.")]
    public float mouseSensitivity = 0.15f;
    [Tooltip("Grados por segundo con el stick derecho del mando.")]
    public float gamepadSensitivity = 180f;
    public bool invertY = false;

    // ---- Valores que leen los otros scripts ----
    public Vector2 Move { get; private set; }      // X = izquierda/derecha, Y = adelante/atrás
    public Vector2 Look { get; private set; }      // grados a girar este frame
    public bool SprintHeld { get; private set; }
    public bool JumpPressed { get; private set; }

    InputAction moveAction;
    InputAction lookMouseAction;
    InputAction lookStickAction;
    InputAction jumpAction;
    InputAction sprintAction;

    void Awake()
    {
        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
        moveAction.AddBinding("<Gamepad>/leftStick");

        lookMouseAction = new InputAction("LookMouse", InputActionType.Value, "<Mouse>/delta");
        lookStickAction = new InputAction("LookStick", InputActionType.Value, "<Gamepad>/rightStick");

        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");

        sprintAction = new InputAction("Sprint", InputActionType.Button);
        sprintAction.AddBinding("<Keyboard>/leftShift");
        sprintAction.AddBinding("<Gamepad>/leftStickPress");
    }

    void OnEnable()
    {
        moveAction.Enable();
        lookMouseAction.Enable();
        lookStickAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        lookMouseAction.Disable();
        lookStickAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();

        Move = Vector2.zero;
        Look = Vector2.zero;
        SprintHeld = false;
        JumpPressed = false;
    }

    void OnDestroy()
    {
        moveAction?.Dispose();
        lookMouseAction?.Dispose();
        lookStickAction?.Dispose();
        jumpAction?.Dispose();
        sprintAction?.Dispose();
    }

    void Update()
    {
        Move = Vector2.ClampMagnitude(moveAction.ReadValue<Vector2>(), 1f);

        // Con el cursor libre (menú, Escape) no giramos la cámara con el ratón.
        Vector2 mouse = Cursor.lockState == CursorLockMode.Locked
            ? lookMouseAction.ReadValue<Vector2>() * mouseSensitivity
            : Vector2.zero;
        Vector2 stick = lookStickAction.ReadValue<Vector2>() * gamepadSensitivity * Time.deltaTime;

        Vector2 look = mouse + stick;
        if (invertY) look.y = -look.y;
        Look = look;

        SprintHeld = sprintAction.IsPressed();
        JumpPressed = jumpAction.WasPressedThisFrame();
    }

    /// <summary>
    /// Busca el PlayerInputHandler del mismo personaje (en este objeto, sus padres o hermanos).
    /// Si no existe, lo crea en la raíz del personaje.
    /// </summary>
    public static PlayerInputHandler FindFor(Component c)
    {
        PlayerInputHandler handler = c.GetComponentInParent<PlayerInputHandler>();
        if (handler == null)
            handler = c.transform.root.GetComponentInChildren<PlayerInputHandler>();
        if (handler == null)
            handler = c.transform.root.gameObject.AddComponent<PlayerInputHandler>();
        return handler;
    }
}
