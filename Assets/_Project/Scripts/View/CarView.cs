using UnityEngine;

class CarView : MonoBehaviour {
    private CarModel model;
    private float zPosition;

    public void Init(CarModel model, float zPosition) {
        this.model = model;
        this.zPosition = zPosition;

        float yRotation = model.Speed > 0 ? 90f : -90f;
        transform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }

    void Update() {
        if (model == null) return;
        transform.position = new Vector3(
            model.PositionX,
            transform.position.y,
            zPosition
        );
    }
}