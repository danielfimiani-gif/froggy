using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerViewTest {
    [UnityTest]
    public IEnumerator PlayerView_MovesAlongZ_WhenModelMoves() {
        //Arrange
        var go = new GameObject();
        var view = go.AddComponent<PlayerView>();
        view.CellSize = 1f;
        var model = new PlayerModel(5);
        view.Init(model);

        //Act
        model.MoveForward();
        yield return null;

        //Assert
        Assert.AreEqual(1f, view.transform.position.z, 0.001f);

        //Cleanup
        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator PlayerView_ResetToStart_OnRespawn() {
        //Arrange
        var go = new GameObject();
        var view = go.AddComponent<PlayerView>();
        view.CellSize = 1f;
        var model = new PlayerModel(5);
        view.Init(model);

        //Act
        model.MoveForward();
        model.Die();
        model.Respawn();
        yield return null;

        //Assert
        Assert.AreEqual(0f, view.transform.position.z, 0.001f);

        //Cleanup
        Object.Destroy(go);
    }
}
