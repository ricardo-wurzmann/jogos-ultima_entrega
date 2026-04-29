using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius = 3f;          // Distância máxima da visão
    [Range(0, 360)]
    public float angle = 90f;         // Abertura do cone
    public LayerMask targetMask;      // Camada do Jogador
    public LayerMask obstructionMask; // Camada das Paredes/Obstáculos

    public bool canSeePlayer;
    private GameObject playerRef;

    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        // Inicia uma rotina de checagem (melhor para performance que o Update)
        InvokeRepeating(nameof(CheckFOV), 0.2f, 0.2f);
    }

    private void CheckFOV()
    {
        Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector2 directionToTarget = (target.position - transform.position).normalized;

            // Checa se o alvo está dentro do ângulo do cone (usando o vetor 'up' ou 'right' dependendo do seu sprite)
            if (Vector2.Angle(transform.up, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.position);

                // Lança um raio para ver se há paredes no caminho
                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        // Desenha o círculo de alcance
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, radius);

        // Desenha as linhas do limite do ângulo
        Vector3 angle01 = DirectionFromAngle(-angle / 2);
        Vector3 angle02 = DirectionFromAngle(angle / 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + angle01 * radius);
        Gizmos.DrawLine(transform.position, transform.position + angle02 * radius);

        if (canSeePlayer && playerRef != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, playerRef.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float angleInDegrees)
    {
        // No Unity 2D, a rotação no eixo Z para a esquerda (anti-horário) é positiva,
        // mas as contas trigonométricas aqui são horárias, então subtraímos a rotação.
        angleInDegrees -= transform.eulerAngles.z;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }
}