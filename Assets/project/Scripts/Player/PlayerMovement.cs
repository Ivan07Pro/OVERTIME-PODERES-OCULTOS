using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    [Tooltip("Estamina por segundo que gasta correr (si hay PlayerStats).")]
    public float sprintStaminaCost = 20f;

    [Header("Salto y gravedad")]
    public float gravity = -20f;
    public float jumpHeight = 2f;

    [Header("Referencias (se buscan solas si están vacías)")]
    public Transform cameraTransform;
    public Animator animator;
    public PlayerInputHandler input;
    public GroundChecker groundChecker;
    public PlayerStats stats;

    CharacterController controller;
    float verticalVelocity;

    static readonly int SpeedHash = Animator.StringToHash("Speed");
    static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    static readonly int VerticalVelocityHash = Animator.StringToHash("VerticalVelocity");
    static readonly int JumpHash = Animator.StringToHash("Jump");

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (input == null)
            input = PlayerInputHandler.FindFor(this);

        if (groundChecker == null)
            groundChecker = GetComponentInChildren<GroundChecker>();

        if (stats == null)
            stats = GetComponentInParent<PlayerStats>();
    }

    void Update()
    {
        if (cameraTransform == null)
        {
            // Puede que la cámara aparezca después (por ejemplo al cargar escena).
            if (Camera.main == null) return;
            cameraTransform = Camera.main.transform;
        }

        Move();
    }

    void Move()
    {
        Vector2 moveInput = input.Move;

        // Dirección relativa a la cámara, sin componente vertical.
        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
        Vector3 move = camForward * moveInput.y + camRight * moveInput.x;
        move = Vector3.ClampMagnitude(move, 1f);

        // Correr solo si nos movemos y (si hay stats) queda estamina.
        bool sprinting = input.SprintHeld && move.sqrMagnitude > 0.01f;
        if (sprinting && stats != null)
            sprinting = stats.UseStamina(sprintStaminaCost * Time.deltaTime);

        float speed = sprinting ? sprintSpeed : walkSpeed;

        bool grounded = controller.isGrounded
                        || (groundChecker != null && groundChecker.IsGrounded && verticalVelocity <= 0f);

        if (grounded && verticalVelocity < 0f)
            verticalVelocity = -2f; // mantiene al jugador pegado al suelo

        if (grounded && input.JumpPressed)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator != null) animator.SetTrigger(JumpHash);
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 finalMove = move * speed;
        finalMove.y = verticalVelocity;
        controller.Move(finalMove * Time.deltaTime);

        // -------- ANIMACIONES --------
        // Speed: 0 = quieto, 1 = caminando, ~1.75 = corriendo (sprintSpeed / walkSpeed).
        if (animator != null)
        {
            float animationSpeed = move.magnitude * (speed / walkSpeed);
            animator.SetFloat(SpeedHash, animationSpeed, 0.1f, Time.deltaTime);
            animator.SetBool(IsGroundedHash, grounded);
            animator.SetFloat(VerticalVelocityHash, verticalVelocity);
        }
    }
}
