using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LevelExit : MonoBehaviour
{
    [Tooltip("Tag do jogador que pode usar a saída.")]
    public string playerTag = "Player";

    [Tooltip("Sprite mostrado quando a saída está bloqueada (objetivo ainda não coletado). Opcional.")]
    public SpriteRenderer lockedVisual;

    [Tooltip("Sprite mostrado quando a saída está liberada. Opcional.")]
    public SpriteRenderer unlockedVisual;

    [Tooltip("GameObject filho com a Light 2D de destaque. Fica ativo só quando todas as entregas foram feitas.")]
    [SerializeField] private GameObject selectedPointHighlight;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void Update()
    {
        bool unlocked = GameManager.instance != null && GameManager.instance.HasAllDeliveries;
        if (lockedVisual != null) lockedVisual.enabled = !unlocked;
        if (unlockedVisual != null) unlockedVisual.enabled = unlocked;

        if (selectedPointHighlight != null && selectedPointHighlight.activeSelf != unlocked)
            selectedPointHighlight.SetActive(unlocked);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (GameManager.instance == null) return;

        if (GameManager.instance.HasAllDeliveries)
        {
            VisualFeedback.LevelCompleted(transform.position);
        }

        GameManager.instance.TryFinishLevel();
    }
}
