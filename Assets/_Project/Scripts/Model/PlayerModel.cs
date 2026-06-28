using System;

public class PlayerModel {
  public int CurrentRow { get; private set; }
  private bool isAlive = true;

  private int totalRows;

  public event Action<int> OnMoved;
  public event Action OnDied;
  public event Action OnReachedGoal;
  public event Action OnRespawned;

  public PlayerModel(int totalRows) {
    this.totalRows = totalRows;
    CurrentRow = 0;
  }

  public void MoveForward() {
    if (!isAlive) return;

    if (CurrentRow >= totalRows - 1) return;

    CurrentRow++;
    OnMoved?.Invoke(CurrentRow);

    if (CurrentRow == totalRows - 1) {
      OnReachedGoal?.Invoke();
    }
  }

  public void MoveBackwards() {
    if (!isAlive) return;

    if (CurrentRow > 0) {
      CurrentRow--;
      OnMoved?.Invoke(CurrentRow);
    }
  }

  public void Die() {
    isAlive = false;
    OnDied?.Invoke();
  }

  public void Respawn() {
    CurrentRow = 0;
    isAlive = true;
    OnRespawned?.Invoke();
  }
}
