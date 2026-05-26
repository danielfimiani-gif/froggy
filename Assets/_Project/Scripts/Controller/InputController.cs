using UnityEngine;
using UnityEngine.InputSystem;

class InputController : MonoBehaviour {
    private PlayerModel model;

    public void Init(PlayerModel model) {
        this.model = model;
    }

    void Update() {
        if (model == null) return;

        if (Keyboard.current.wKey.wasPressedThisFrame)
            model.MoveForward();
        if (Keyboard.current.sKey.wasPressedThisFrame)
            model.MoveBackwards();
    }
}