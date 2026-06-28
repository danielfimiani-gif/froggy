using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CameraFollowTests {
    private static void SetPrivate(object o, string f, object v) {
        o.GetType()
            .GetField(f, BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(o, v);
    }

    [UnityTest]
    public IEnumerator CameraFollow_MovesTowardTarget_WhenTargetMoves() {
        //Arrange
        var camGO = new GameObject();
        camGO.transform.position = new Vector3(0, 10, -10);

        var follow = camGO.AddComponent<CameraFollow>();
        var target = new GameObject().transform;
        SetPrivate(follow, "target", target);

        yield return null;
        float startZ = camGO.transform.position.z;

        //Act
        target.position = new Vector3(0, 0, 5);
        for (int i = 0; i < 30; i++) yield return null;

        //Assert
        Assert.Greater(camGO.transform.position.z, startZ);

        //Cleanup
        Object.Destroy(camGO);
        Object.Destroy(target.gameObject);
    }
}
