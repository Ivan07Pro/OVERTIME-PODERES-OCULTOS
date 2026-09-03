using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;

    [Header("Referencias")]
    public Transform cameraTransform;
    public Animator animator;

    private CharacterController controller;
    private float verticalVelocity;

    public float gravity = -20f;
    public float jumpHeight = 2f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * vertical + camRight * horizontal;

        bool sprint = Input.GetKey(KeyCode.LeftShift);
        float speed = sprint ? sprintSpeed : walkSpeed;

        if (controller.isGrounded)
        {
            verticalVelocity = -2f;

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 finalMove = move.normalized * speed;
        finalMove.y = verticalVelocity;

        controller.Move(finalMove * Time.deltaTime);

        // -------- ANIMACIONES --------

        float animationSpeed = move.magnitude;

        if (animator != null)
        {
            animator.SetFloat("Speed", animationSpeed);
            animator.SetBool("IsGrounded", controller.isGrounded);
            animator.SetFloat("VerticalVelocity", verticalVelocity);
        }

        // Descomenta esta línea si quieres comprobar el valor
        // Debug.Log("Speed = " + animationSpeed);
    }
}