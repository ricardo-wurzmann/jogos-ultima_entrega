using UnityEngine;

public class PatrolAI : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] waypoints;
    public float speed = 3f;
    public float chaseSpeed = 5f;
    public float rotationSpeed = 5f;
    public float waitTime = 1f;

    [Header("Hearing")]
    public LayerMask wallMask;

    private int _currentWaypointIndex = 0;
    private bool _isWaiting = false;
    private bool _isChasing = false;
    private bool _isInvestigating = false;
    private Vector2 _investigationTarget;
    private Rigidbody2D rb;
    private FieldOfView _fov;
    private Transform _player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _fov = GetComponent<FieldOfView>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = Vector2.zero;

        bool seesPlayer = _fov != null && _fov.canSeePlayer && _player != null;

        if (seesPlayer)
        {
            if (!_isChasing)
            {
                _isChasing = true;
                _isWaiting = false;
                _isInvestigating = false;
                StopAllCoroutines();
            }
            MoveToTarget(_player.position, chaseSpeed);
            return;
        }

        if (_isChasing)
        {
            _isChasing = false;
            if (_player != null)
            {
                _isInvestigating = true;
                _investigationTarget = _player.position;
            }
        }

        CheckForNoise();

        if (_isInvestigating)
        {
            MoveToTarget(_investigationTarget, speed);
            if (Vector2.Distance(rb.position, _investigationTarget) < 0.5f)
            {
                _isInvestigating = false;
            }
            return;
        }

        if (waypoints.Length == 0 || _isWaiting) return;
        MoveToWaypoint();
    }

    void CheckForNoise()
    {
        if (_isChasing) return;

        NoiseSource[] sources = Object.FindObjectsByType<NoiseSource>(FindObjectsSortMode.None);
        foreach (var source in sources)
        {
            if (!source.IsActive) continue;

            Vector2 to = (Vector2)source.transform.position - (Vector2)transform.position;
            float dist = to.magnitude;
            if (dist > source.range) continue;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, to.normalized, dist, wallMask);
            if (hit.collider != null) continue;

            _isInvestigating = true;
            _investigationTarget = source.transform.position;
            _isWaiting = false;
            StopAllCoroutines();
            return;
        }
    }

    void MoveToWaypoint()
    {
        Transform target = waypoints[_currentWaypointIndex];
        MoveToTarget(target.position, speed);

        if (Vector2.Distance(rb.position, target.position) < 0.1f)
        {
            StartCoroutine(WaitAndSwitch());
        }
    }

    void MoveToTarget(Vector2 targetPosition, float currentSpeed)
    {
        Vector2 direction = (targetPosition - rb.position).normalized;

        if (direction != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float smoothedAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothedAngle);
        }

        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, currentSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
    }

    System.Collections.IEnumerator WaitAndSwitch()
    {
        _isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
        _isWaiting = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.GameOver();
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        for (int i = 0; i < waypoints.Length; i++)
        {
            Transform a = waypoints[i];
            Transform b = waypoints[(i + 1) % waypoints.Length];
            if (a == null || b == null) continue;

            Vector2 from = a.position;
            Vector2 to = b.position;
            Vector2 delta = to - from;
            float dist = delta.magnitude;
            if (dist < Mathf.Epsilon) continue;

            RaycastHit2D hit = Physics2D.Raycast(from, delta.normalized, dist, wallMask);
            Gizmos.color = hit.collider != null ? Color.red : Color.green;
            Gizmos.DrawLine(from, to);
        }
    }
}
