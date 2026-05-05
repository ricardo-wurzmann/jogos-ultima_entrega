using UnityEngine;

public class CameraJuice : MonoBehaviour
{
    private static CameraJuice _instance;

    private Vector3 _baseLocalPosition;
    private float _shakeTime;
    private float _shakeDuration;
    private float _shakeStrength;

    private void Awake()
    {
        _instance = this;
        _baseLocalPosition = transform.localPosition;
    }

    public static void Shake(float strength = 0.12f, float duration = 0.18f)
    {
        if (_instance == null) return;

        _instance._shakeStrength = Mathf.Max(_instance._shakeStrength, strength);
        _instance._shakeDuration = Mathf.Max(0.01f, duration);
        _instance._shakeTime = _instance._shakeDuration;
    }

    private void LateUpdate()
    {
        if (_shakeTime <= 0f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _baseLocalPosition, Time.deltaTime * 12f);
            return;
        }

        _shakeTime -= Time.deltaTime;
        float progress = Mathf.Clamp01(_shakeTime / _shakeDuration);
        Vector2 offset = Random.insideUnitCircle * (_shakeStrength * progress);
        transform.localPosition = _baseLocalPosition + new Vector3(offset.x, offset.y, 0f);
    }
}
