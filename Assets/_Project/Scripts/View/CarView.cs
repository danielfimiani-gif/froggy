using UnityEngine;

class CarView : MonoBehaviour {
    private CarModel model;
    private float zPosition;

    public void Init(CarModel model, float zPosition) {
        this.model = model;
        this.zPosition = zPosition;
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