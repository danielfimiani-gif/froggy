using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayeraAudioViewsTests {
    private static void SetPrivate(object obj, string field, object value) {
        obj.GetType()
            .GetField(field, BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(obj, value);
    }

    [UnityTest]
    public IEnumerator PlayerAudioView_ReactsToAllEvents_WithoutError() {
        //Arrange
        var go = new GameObject();
        var audiosource = go.AddComponent<AudioSource>();
        var view = go.AddComponent<PlayerAudioView>();

        var clip = AudioClip.Create("test", 44100, 1, 44100, false);
        SetPrivate(view, "source", audiosource);
        SetPrivate(view, "hopClip", clip);
        SetPrivate(view, "crashClip", clip);
        SetPrivate(view, "winClip", clip);

        var model = new PlayerModel(2);
        view.Init(model);

        //Act
        model.MoveForward();
        model.Die();
        yield return null;

        //Cleanup
        Object.Destroy(go);
        yield return null;

        //Assert
        Assert.Pass("Audio handlers rund without errors");
    }
}
