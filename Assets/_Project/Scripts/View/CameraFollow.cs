using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private float smoothness = 5f;

    private Vector3 offset;

    void Start() {
        if (target == null) return;
        offset = transform.position - target.position;
    }

    void LateUpdate() {
        if (target == null) return;

        Vector3 desired = new Vector3(
            transform.position.x,
            transform.position.y,
            target.position.z + offset.z
        );

        transform.position = Vector3.Lerp(transform.position, desired, smoothness * Time.deltaTime);
    }
}
