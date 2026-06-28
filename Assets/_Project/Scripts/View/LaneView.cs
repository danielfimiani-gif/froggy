using System.Collections.Generic;
using UnityEngine;

public class LaneView : MonoBehaviour {
    private LaneModel model;
    public float zPosition;
    public GameObject[] carPrefabs;
    private Dictionary<CarModel, GameObject> carGameObjects = new();

    public void Init(LaneModel model, float zPosition) {
        this.model = model;
        this.zPosition = zPosition;
        model.OnCarSpawned += HandleCarSpawned;
        model.OnCarDeSpawned += HandleCarDeSpawned;
    }
    private void HandleCarSpawned(CarModel car) {
        var prefab = carPrefabs[UnityEngine.Random.Range(0, carPrefabs.Length)];
        GameObject go = Instantiate(prefab, transform);
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
