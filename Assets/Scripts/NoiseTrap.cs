using UnityEngine;

public class NoiseTrap : MonoBehaviour
{
    public GameObject noisePrefab;
    public float playerFreezeDuration = 1f;

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

            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null) pm.Freeze(playerFreezeDuration);

            Destroy(gameObject);
        }
    }
}
