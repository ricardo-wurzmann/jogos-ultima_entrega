using UnityEngine;

public class NoiseTrap : MonoBehaviour
{
    public GameObject noisePrefab;
    public float playerFreezeDuration = 1f;

    [Header("Audio")]
    public AudioClip triggerSound;
    [Range(0f, 1f)] public float triggerVolume = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (noisePrefab != null)
            {
                GameObject obj = Instantiate(noisePrefab, transform.position, Quaternion.identity);
                NoiseSource ns = obj.GetComponent<NoiseSource>();
                if (ns != null) ns.delay = 0f;
            }

            if (triggerSound != null)
                AudioSource.PlayClipAtPoint(triggerSound, transform.position, triggerVolume);

            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null) pm.Freeze(playerFreezeDuration);

            VisualFeedback.TrapTriggered(transform.position);
            Destroy(gameObject);
        }
    }
}
