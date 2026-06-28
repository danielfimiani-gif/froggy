using NUnit.Framework;

public class PlayerModelTests {
  [TestCase(5, 3, ExpectedResult = 3)]
  [TestCase(5, 10, ExpectedResult = 4)]
  [TestCase(2, 5, ExpectedResult = 1)]
  public int MoveForward_ClampsAtUpperBound(int totalRows, int steps) {
    // Arragne
    var player = new PlayerModel(totalRows);

    // Act
    for (int i = 0; i < steps; i++) player.MoveForward();

    // Assert
    return player.CurrentRow;
  }

  [TestCase(5, 3, ExpectedResult = 1)]
  [TestCase(5, 10, ExpectedResult = 0)]
  [TestCase(5, 1, ExpectedResult = 3)]
  public int MoveBackWards_ClampsAtLowerBoud(int totalRows, int steps) {
    //Arragne
    var player = new PlayerModel(totalRows);

    //Act
    for (int i = 0; i < totalRows; i++) player.MoveForward();
    for (int i = 0; i < steps; i++) player.MoveBackwards();

    //Assert
    return player.CurrentRow;
  }

  [Test]
  public void MoveForward_WhenDeadShouldNotMove() {
    //Arrange
    int totalRows = 2;
    int expectedCurrentRow = 0;
    int currentRow = int.MinValue;
    bool moves = false;
    bool isDead = false;
    var player = new PlayerModel(totalRows);
    player.OnDied += () => isDead = true;
    player.OnMoved += (r) => {
      moves = true;
      currentRow = r;
    };

    //Act
    player.Die();
    player.MoveForward();

    //Assert
    Assert.IsTrue(isDead);
    Assert.IsFalse(moves);
    Assert.AreEqual(int.MinValue, currentRow);
    Assert.AreEqual(expectedCurrentRow, player.CurrentRow);
  }

  [Test]
  public void MoveForward_FiresCurrentRow() {
    //Arragne
    int totalRows = 4;
    var player = new PlayerModel(totalRows);
    int expectedMovedRow = 1;
    int currentRow = int.MinValue;
    bool reached = false;
    player.OnReachedGoal += () => reached = true;
    player.OnMoved += (r) => currentRow = r;

    //Act
    player.MoveForward();

    //Assert
    Assert.IsFalse(reached);
    Assert.AreEqual(expectedMovedRow, currentRow);
  }

  [Test]
  public void MoveBackwards_WhenDeadShouldNotMove() {
    //Arrange
    int totalRows = 2;
    int expectedCurrentRow = 0;
    int currentRow = int.MinValue;
    bool moves = false;
    bool isDead = false;
    var player = new PlayerModel(totalRows);
    player.OnDied += () => isDead = true;
    player.OnMoved += (r) => {
      moves = true;
      currentRow = r;
    };

    //Act
    player.Die();
    player.MoveForward();

    //Assert
    Assert.IsTrue(isDead);
    Assert.IsFalse(moves);
    Assert.AreEqual(int.MinValue, currentRow);
    Assert.AreEqual(expectedCurrentRow, player.CurrentRow);
  }

  [Test]
  public void MoveBackwards_FiresCurrentRow() {
    //Arragne
    int totalRows = 4;
    var player = new PlayerModel(totalRows);
    int expectedMovedRow = 1;
    int currentRow = int.MinValue;
    bool reached = false;
    player.OnReachedGoal += () => reached = true;
    player.OnMoved += (r) => currentRow = r;
    for (int i = 0; i < 2; i++) player.MoveForward();

    //Act
    player.MoveBackwards();

    //Assert
    Assert.IsFalse(reached);
    Assert.AreEqual(expectedMovedRow, currentRow);
  }

  [Test]
  public void MoveForward_ToLastRow_FiresOnReachedGoal() {
    //Arrange
    int totalRows = 2;
    var player = new PlayerModel(totalRows);
    bool reached = false;
    player.OnReachedGoal += () => reached = true;

    //Act
    player.MoveForward();

    //Assert
    Assert.IsTrue(reached);
  }

  [Test]
  public void Die_FiresOnDied() {
    //Arrange
    bool isDied = false;
    int totalRows = 2;
    var player = new PlayerModel(totalRows);
    player.OnDied += () => isDied = true;

    //Act
    player.Die();

    //Assert
    Assert.IsTrue(isDied);
  }

  [Test]
  public void OnRespawn_SetCurrentRowAndFiresOnRespasn() {
    //Arrange
    int totalRows = 2;
    var player = new PlayerModel(totalRows);
    bool isRespawned = false;
    player.OnRespawned += () => isRespawned = true;

    //Act
    player.Respawn();

    //Assert
    Assert.IsTrue(isRespawned);
    Assert.AreEqual(0, player.CurrentRow);
  }
}


