using UnityEngine;

/// <summary>
/// Comprueba si el jugador toca el suelo con una esfera pequeña.
/// Ponlo en el objeto "GroundCheck", situado en los pies del personaje.
/// </summary>
public class GroundChecker : MonoBehaviour
{
    [Tooltip("Radio de la esfera que detecta el suelo.")]
    public float radius = 0.25f;
    [Tooltip("Capas que cuentan como suelo.")]
    public LayerMask groundLayers = ~0;

    public bool IsGrounded { get; private set; }

    readonly Collider[] hits = new Collider[8];

    void FixedUpdate()
    {
        IsGrounded = Check();
    }

    public bool Check()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position, radius, hits, groundLayers, QueryTriggerInteraction.Ignore);

        Transform myRoot = transform.root;
        for (int i = 0; i < count; i++)
        {
            // Ignoramos los colliders del propio jugador.
            if (hits[i].transform.root != myRoot)
                return true;
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
