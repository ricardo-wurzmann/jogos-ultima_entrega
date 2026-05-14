using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Objective : MonoBehaviour
{
    [Tooltip("ID que identifica este item. Deve ser igual ao objectiveId do DeliveryPoint correspondente.")]
    public string objectiveId = "item";

    [Tooltip("Nome exibido no HUD enquanto o jogador carrega. Deixe vazio para usar o objectiveId.")]
    public string displayName = "";

    [Tooltip("Tag do jogador que pode coletar o objetivo.")]
    public string playerTag = "Player";

    [Tooltip("Som opcional tocado ao coletar.")]
    public AudioClip pickupSound;

    private bool _pickedUp;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_pickedUp) return;
        if (!other.CompareTag(playerTag)) return;

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm == null || pm.IsCarrying) return;

        _pickedUp = true;
        pm.PickUpObjective(objectiveId, string.IsNullOrEmpty(displayName) ? objectiveId : displayName);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        VisualFeedback.ObjectiveCollected(transform.position);
        Destroy(gameObject);
    }
}
