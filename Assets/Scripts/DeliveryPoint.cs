using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeliveryPoint : MonoBehaviour
{
    [Tooltip("ID do objetivo aceito. Deve coincidir com o objectiveId do Objective correspondente.")]
    public string objectiveId = "item";

    [Header("Visual")]
    public Color corPendente = new Color(1f, 0.82f, 0f,   0.95f);
    public Color corColetado = new Color(0f,  0.8f, 1f,   0.95f);
    public Color corEntregue = new Color(0.3f, 1f,  0.3f, 0.95f);

    [Tooltip("GameObject filho com a Light 2D de destaque (ex.: 'selected_point'). Fica ativo só quando o jogador carrega o objetivo correspondente.")]
    [SerializeField] private GameObject selectedPointHighlight;

    [Header("Audio")]
    public AudioClip deliverySound;
    [Range(0f, 1f)] public float deliverySoundVolume = 1f;

    public bool IsDelivered { get; private set; }

    private SpriteRenderer _sr;
    private PlayerMovement _player;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnEnable()
    {
        GameManager.instance?.RegisterDeliveryPoint(this);
    }

    private void Start()
    {
        GameManager.instance?.RegisterDeliveryPoint(this);
        _player = FindAnyObjectByType<PlayerMovement>();
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        bool isTarget = !IsDelivered
                        && _player != null
                        && _player.CarriedObjectiveId == objectiveId;

        if (selectedPointHighlight != null && selectedPointHighlight.activeSelf != isTarget)
            selectedPointHighlight.SetActive(isTarget);

        if (_sr == null) return;

        if (IsDelivered)
            _sr.color = corEntregue;
        else if (isTarget)
            _sr.color = corColetado;
        else
            _sr.color = corPendente;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsDelivered) return;
        if (!other.CompareTag("Player")) return;

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm == null || !pm.TryDeliver(objectiveId)) return;

        IsDelivered = true;

        if (deliverySound != null)
            AudioSource.PlayClipAtPoint(deliverySound, transform.position, deliverySoundVolume);

        VisualFeedback.ObjectiveCollected(transform.position);
        GameManager.instance?.CompleteDelivery(this);
    }

}
