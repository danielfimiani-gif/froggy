using UnityEngine;

class CameraFollow : MonoBehaviour {
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothness = 5f;

    void LateUpdate() {
        if (target == null) return;

        Vector3 desired = target.position + offset;

        Vector3 smoothPosition = Vector3.Lerp(transform.position, desired, smoothness * Time.deltaTime);

        transform.position = smoothPosition;

        transform.LookAt(target);
    }
}