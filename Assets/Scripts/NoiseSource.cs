using UnityEngine;
using TMPro;

public class NoiseSource : MonoBehaviour
{
    public float range = 5f;
    public float delay = 3f;
    public float activeDuration = 1f;

    [SerializeField] private TextMeshPro countdownText;

    private float _timeAlive = 0f;

    public bool IsActive => _timeAlive >= delay && _timeAlive < delay + activeDuration;

    private void Start()
    {
        if (countdownText == null)
        {
            countdownText = GetComponentInChildren<TextMeshPro>();
        }
        if (delay <= 0f && countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
        Destroy(gameObject, delay + activeDuration);
    }

    private void Update()
    {
        _timeAlive += Time.deltaTime;

        if (countdownText == null) return;

        float remaining = delay - _timeAlive;
        if (remaining > 0f)
        {
            countdownText.text = remaining.ToString("0.0") + "s";
        }
        else
        {
            countdownText.text = "!";
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
