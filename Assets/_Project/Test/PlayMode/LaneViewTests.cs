using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LaneViewTests {
    [UnityTest]
    public IEnumerator LaneView_InstantiateCar_WhenModelSpawns() {
        //Arrange
        var prefab = new GameObject("carPrefab");
        prefab.AddComponent<CarView>();
        var go = new GameObject();
        var laneView = go.AddComponent<LaneView>();
        laneView.carPrefabs = new[] { prefab };
        var model = new LaneModel(1, speed: 5f, spawnInterval: 1f, min: -10f, max: 10f, carWidth: 1f);
        laneView.Init(model, zPosition: 0f);

        //Act
        model.Tick(1f);
        yield return null;

        //Assert
        Assert.Greater(laneView.transform.childCount, 0);

        //Cleanup
        Object.Destroy(go);
        Object.Destroy(prefab);
    }

    [UnityTest]
    public IEnumerator LaneView_DestoysCar_WhenModelDespawns() {
        //Arrange
        var prefab = new GameObject("carPrefab");
        prefab.AddComponent<CarView>();
        var go = new GameObject();
        var laneView = go.AddComponent<LaneView>();
        laneView.carPrefabs = new[] { prefab };
        var model = new LaneModel(1, speed: 5f, spawnInterval: 1f, min: -2f, max: 2f, carWidth: 1f);
        laneView.Init(model, zPosition: 0f);

        // Act 1 Assert
        model.Tick(1f);
        yield return null;
        Assert.AreEqual(1, laneView.transform.childCount);

        // Act 2 Assert
        model.Tick(0.9f);
        yield return null;
        Assert.AreEqual(0, laneView.transform.childCount);

        //Cleanup
        Object.Destroy(go);
        yield return null;
        Object.Destroy(prefab);
    }
}
