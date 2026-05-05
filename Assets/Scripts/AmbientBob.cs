using UnityEngine;

public class AmbientBob : MonoBehaviour
{
    public float amplitude = 0.05f;
    public float speed = 1.5f;

    private Vector3 _startPosition;
    private float _phase;

    private void Start()
    {
        _startPosition = transform.localPosition;
        _phase = Random.value * Mathf.PI * 2f;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * speed + _phase) * amplitude;
        transform.localPosition = _startPosition + new Vector3(0f, offset, 0f);
    }
}
