using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Camera cameraToFollow;
    public float zOffset = 8f;

    private void LateUpdate()
    {
        if (cameraToFollow == null) return;

        Vector3 position = cameraToFollow.transform.position;
        transform.position = new Vector3(position.x, position.y, position.z + zOffset);
    }
}
