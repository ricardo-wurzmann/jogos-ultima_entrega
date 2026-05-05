using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Objective : MonoBehaviour
{
    [Tooltip("Tag do jogador que pode coletar o objetivo.")]
    public string playerTag = "Player";

    [Tooltip("Som opcional tocado ao coletar.")]
    public AudioClip pickupSound;

    private bool _collected;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnEnable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterObjective(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected) return;
        if (!other.CompareTag(playerTag)) return;
        if (GameManager.instance == null) return;

        _collected = true;
        GameManager.instance.CollectObjective(this);

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        VisualFeedback.ObjectiveCollected(transform.position);
        Destroy(gameObject);
    }
}
