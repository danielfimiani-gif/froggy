using UnityEngine;

public class PlayerView : MonoBehaviour {
    private PlayerModel model;
    public float CellSize = 1.0f;

    public void Init(PlayerModel model) {
        this.model = model;
        model.OnMoved += HandleMoved;
        model.OnRespawned += HandleRespawned;
    }

    private void HandleMoved(int newRow) {
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            newRow * CellSize
        );
    }

    private void HandleRespawned() {
        HandleMoved(0);
    }

    void OnDestroy() {
        if (model != null) {
            model.OnMoved -= HandleMoved;
            model.OnRespawned -= HandleRespawned;
        }
    }
}
