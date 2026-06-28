using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CarViewTests {
    [UnityTest]
    public IEnumerator CarView_FollowsModelPositionX_AfterUpdate() {
        //Arrange
        var go = new GameObject();
        float expectedPositionX = 5f;
        var view = go.AddComponent<CarView>();
        var model = new CarModel {
            PositionX = 5f,
            Speed = 3f,
            Width = 1f,
        };

        view.Init(model, zPosition: 2f);

        //Act
        yield return null;

        //Assert
        Assert.AreEqual(expectedPositionX, view.transform.position.x);

        //Cleanup
        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator CarView_KeepsFollowingModel_WhenPositionChanges() {
        //Arrange
        var go = new GameObject();
        float expectedPositionX1 = 5f;
        float expectedPositionX2 = -3f;
        var view = go.AddComponent<CarView>();
        var model = new CarModel {
            PositionX = 5f,
            Speed = 3f,
            Width = 1f,
        };
        view.Init(model, 2f);

        //Act + Assert 1
        yield return null;
        Assert.AreEqual(expectedPositionX1, view.transform.position.x, 0.001f);

        //Act + Assert 2
        model.PositionX = -3f;
        yield return null;
        Assert.AreEqual(expectedPositionX2, view.transform.position.x, 0.0001f);

        //Cleanup
        Object.Destroy(go);
    }
}
