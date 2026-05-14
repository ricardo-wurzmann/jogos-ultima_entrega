using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed;

    [Header("Sprint")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeedMultiplier = 1.7f;
    public float maxStamina = 100f;
    public float staminaDrainPerSecond = 35f;
    public float staminaRegenPerSecond = 20f;
    [Tooltip("Após zerar a stamina, é preciso recuperar até este valor antes de poder correr de novo.")]
    public float staminaUnlockThreshold = 25f;

    [Header("Noise")]
    public GameObject noisePrefab;
    public float noiseCooldown = 2f;
    public int maxNoises = 3;

    private Rigidbody2D rb;
    private float _lastNoiseTime = -Mathf.Infinity;
    private int _noisesUsed = 0;
    private float _freezeTimer = 0f;
    private float _stamina;
    private bool _sprintLocked;
    private bool _isSprinting;

    public int NoisesRemaining => Mathf.Max(0, maxNoises - _noisesUsed);
    public bool IsFrozen => _freezeTimer > 0f;
    public float NoiseCooldownRemaining => Mathf.Max(0f, noiseCooldown - (Time.time - _lastNoiseTime));

    public float Stamina => _stamina;
    public float StaminaRatio => maxStamina > 0f ? Mathf.Clamp01(_stamina / maxStamina) : 0f;
    public bool IsSprinting => _isSprinting;

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
        _stamina = maxStamina;
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
            _isSprinting = false;
            RegenStamina(Time.fixedDeltaTime);
            return;
        }

        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        bool isMoving = movement.sqrMagnitude > 0.001f;

        bool wantsSprint = Input.GetKey(sprintKey) && isMoving;

        if (_sprintLocked && _stamina >= staminaUnlockThreshold)
            _sprintLocked = false;

        _isSprinting = wantsSprint && !_sprintLocked && _stamina > 0f;

        if (_isSprinting)
        {
            _stamina = Mathf.Max(0f, _stamina - staminaDrainPerSecond * Time.fixedDeltaTime);
            if (_stamina <= 0f)
            {
                _sprintLocked = true;
                _isSprinting = false;
            }
        }
        else
        {
            RegenStamina(Time.fixedDeltaTime);
        }

        float currentSpeed = _isSprinting ? speed * sprintSpeedMultiplier : speed;
        rb.MovePosition(rb.position + movement.normalized * Time.fixedDeltaTime * currentSpeed);
    }

    private void RegenStamina(float deltaTime)
    {
        if (_stamina < maxStamina)
            _stamina = Mathf.Min(maxStamina, _stamina + staminaRegenPerSecond * deltaTime);
    }
}
