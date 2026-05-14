using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed;

    [Header("Noise")]
    public GameObject noisePrefab;
    public float noiseCooldown = 2f;
    public int maxNoises = 3;

    private Rigidbody2D rb;
    private float _lastNoiseTime = -Mathf.Infinity;
    private int _noisesUsed = 0;
    private float _freezeTimer = 0f;

    public int NoisesRemaining => Mathf.Max(0, maxNoises - _noisesUsed);
    public bool IsFrozen => _freezeTimer > 0f;
    public float NoiseCooldownRemaining => Mathf.Max(0f, noiseCooldown - (Time.time - _lastNoiseTime));

    public string CarriedObjectiveId  { get; private set; } = "";
    public string CarriedDisplayName  { get; private set; } = "";
    public bool   IsCarrying => !string.IsNullOrEmpty(CarriedObjectiveId);

    public void PickUpObjective(string id, string label)
    {
        CarriedObjectiveId = id;
        CarriedDisplayName = label;
    }

    public bool TryDeliver(string requiredId)
    {
        if (CarriedObjectiveId != requiredId) return false;
        CarriedObjectiveId = "";
        CarriedDisplayName = "";
        return true;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
        rb.angularVelocity = 0f;
    }

    public void Freeze(float duration)
    {
        if (duration > _freezeTimer) _freezeTimer = duration;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryThrowNoise();
        }
    }

    void TryThrowNoise()
    {
        if (IsFrozen) return;
        if (noisePrefab == null) return;
        if (_noisesUsed >= maxNoises) return;
        if (Time.time - _lastNoiseTime < noiseCooldown) return;

        Instantiate(noisePrefab, transform.position, Quaternion.identity);
        _lastNoiseTime = Time.time;
        _noisesUsed++;
    }

    void FixedUpdate()
    {
        if (_freezeTimer > 0f)
        {
            _freezeTimer -= Time.fixedDeltaTime;
            return;
        }

        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        rb.MovePosition(rb.position + movement.normalized * Time.fixedDeltaTime * speed);
    }
}
