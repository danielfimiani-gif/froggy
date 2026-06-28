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
}
