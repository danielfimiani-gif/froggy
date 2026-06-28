using System;

class GameStateModel {
    public EGameState State { get; private set; }

    public event Action<EGameState> OnStateChanged;

    public void SetPlaying() {
        if (State != EGameState.Playing) {
            State = EGameState.Playing;
            OnStateChanged?.Invoke(EGameState.Playing);
        }
    }

    public void SetWon() {
        State = EGameState.Won;
        OnStateChanged?.Invoke(EGameState.Won);
    }
}
