using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class GameController : MonoBehaviour {
    [SerializeField] private PlayerView playerView;
    [SerializeField] private LaneView[] laneViews;
    [SerializeField] private LaneConfig[] laneConfigs;

    [SerializeField] private float cellSize = 1.0f;
    [SerializeField] private float playerHalfWidth = 0.4f;
    [SerializeField] private float laneMinX = -10;
    [SerializeField] private float laneMaxX = 10;

    [SerializeField] private PlayerAudioView playerAudioView;

    private PlayerModel playerModel;
    private GameStateModel gameState;
    private List<LaneModel> laneModels;

    void Awake() {
        int totalRows = laneViews.Length + 2;
        playerModel = new PlayerModel(totalRows);
        gameState = new GameStateModel();
        laneModels = new List<LaneModel>();

        for (int i = 0; i < laneViews.Length; i++) {
            var cfg = laneConfigs[i];
            int row = i + 1;
            var laneModel = new LaneModel(row, cfg.Speed, cfg.SpawnInterval, laneMinX, laneMaxX, cfg.CarWidth);
            laneModels.Add(laneModel);
            laneViews[i].Init(laneModel, row * cellSize);
        }

        playerView.Init(playerModel);
        if (playerAudioView != null) playerAudioView.Init(playerModel);

        playerModel.OnReachedGoal += HandlePlayerWon;

        gameState.SetPlaying();

        GetComponent<InputController>().Init(playerModel);
    }

    void Update() {
        if (gameState.State != EGameState.Playing) return;

        float dt = Time.deltaTime;
        foreach (var lane in laneModels)
            lane.Tick(dt);

        CheckCollisions();
    }

    void CheckCollisions() {
        foreach (var lane in laneModels) {
            if (lane.RowIndex != playerModel.CurrentRow) continue;
            foreach (var car in lane.Cars) {
                if (CollisionDetector.OverLaps(car.PositionX, car.Width, playerView.transform.position.x, playerHalfWidth)) {
                    playerModel.Die();
                    playerModel.Respawn();
                    return;
                }
            }
        }
    }

    void HandlePlayerWon() {
        gameState.SetWon();
        StartCoroutine(LoadWonAfterDelay());
    }

    IEnumerator LoadWonAfterDelay() {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("WonScene");
    }
}
