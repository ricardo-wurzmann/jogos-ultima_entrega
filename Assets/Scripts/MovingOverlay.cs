using UnityEngine;

public class MovingOverlay : MonoBehaviour
{
    public Camera cameraToFollow;
    public Vector3 baseScale = Vector3.one;
    public Vector2 offset;
    public Vector2 speed = new Vector2(0.08f, 0.04f);
    public Vector2 driftAmount = new Vector2(1f, 0.35f);

    private void LateUpdate()
    {
        if (cameraToFollow == null) return;

        float height = cameraToFollow.orthographicSize * 2f;
        float width = height * cameraToFollow.aspect;
        transform.localScale = new Vector3(width / 16f * baseScale.x, height / 16f * baseScale.y, 1f);

        Vector3 drift = new Vector3(
            Mathf.Sin(Time.time * speed.x) * driftAmount.x,
            Mathf.Cos(Time.time * speed.y) * driftAmount.y,
            0f);

        transform.position = cameraToFollow.transform.position + new Vector3(offset.x, offset.y, 10f) + drift;
    }
}
