using System;
using UnityEngine;

public class PlayerAudioView : MonoBehaviour {
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip hopClip;
    [SerializeField] AudioClip crashClip;
    [SerializeField] AudioClip winClip;

    private PlayerModel model;

    public void Init(PlayerModel model) {
        this.model = model;
        model.OnMoved += HadleMoved;
        model.OnDied += HandleDied;
        model.OnReachedGoal += HandleReachedGoal;
    }

    private void HandleReachedGoal() {
        if (hopClip != null) source.PlayOneShot(winClip);
    }

    private void HandleDied() {
        if (crashClip != null) source.PlayOneShot(crashClip);
    }

    private void HadleMoved(int obj) {
        if (winClip != null) source.PlayOneShot(hopClip);
    }

    void OnDestroy() {
        if (model != null) {
            model.OnMoved -= HadleMoved;
            model.OnDied -= HandleDied;
            model.OnReachedGoal -= HandleReachedGoal;
        }
    }
}
