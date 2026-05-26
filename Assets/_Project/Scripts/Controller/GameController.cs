using System;
using System.Collections.Generic;
using UnityEngine;

class GameController : MonoBehaviour {
    [SerializeField] private PlayerView playerView;
    [SerializeField] private LaneView[] laneViews;
    [SerializeField] private LaneConfig[] laneConfigs;

    [SerializeField] private float cellSize = 1.0f;
    [SerializeField] private float playerHalfWidth = 0.4f;
    [SerializeField] private float laneMinX = -10;
    [SerializeField] private float laneMaxX = 10;

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
                float dx = Math.Abs(car.PositionX - 0);
                if (dx < (car.Width + playerHalfWidth * 2) / 2) {
                    playerModel.Respawn();
                    return;
                }
            }
        }
    }

    void HandlePlayerWon() {
        gameState.SetWon();
    }
}