using System;
using System.Collections.Generic;
using UnityEngine;

class LaneView : MonoBehaviour {
    private LaneModel model;
    public float zPosition;
    public GameObject carPrefab;
    private Dictionary<CarModel, GameObject> carGameObjects = new();

    public void Init(LaneModel model, float zPosition) {
        this.model = model;
        this.zPosition = zPosition;
        model.OnCarSpawned += HandleCarSpawned;
        model.OnCarDeSpawned += HandleCarDeSpawned;
    }
    private void HandleCarSpawned(CarModel car) {
        GameObject go = Instantiate(carPrefab);
        CarView view = go.GetComponent<CarView>();
        view.Init(car, zPosition);
        carGameObjects[car] = go;
    }

    private void HandleCarDeSpawned(CarModel car) {
        if (carGameObjects.TryGetValue(car, out var go)) {
            Destroy(go);
            carGameObjects.Remove(car);
        }
    }

    void OnDestroy() {
        if (model != null) {
            model.OnCarSpawned -= HandleCarSpawned;
            model.OnCarDeSpawned -= HandleCarDeSpawned;
        }
    }
}