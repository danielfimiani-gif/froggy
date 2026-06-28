using NUnit.Framework;

public class GameStateModelTests {
    private GameStateModel _gameState;

    [SetUp]
    public void Setup() {
        _gameState = new GameStateModel();
    }

    [Test]
    public void SetPlaying_ChageStateToPlaying_FireOnStateChanged() {
        //Arrange
        _gameState.SetWon();
        EGameState expectedState = EGameState.Playing;
        bool stateChanged = false;
        _gameState.OnStateChanged += (s) => stateChanged = true;

        //Act
        _gameState.SetPlaying();

        //Assert
        Assert.AreEqual(expectedState, _gameState.State);
        Assert.IsTrue(stateChanged);
    }

    [Test]
    public void SetWon_ChageStateToWon_FireOnStateChanged() {
        //Arrange
        EGameState expectedState = EGameState.Won;
        bool stateChanged = false;
        _gameState.OnStateChanged += (s) => stateChanged = true;

        //Act
        _gameState.SetWon();

        //Assert
        Assert.AreEqual(expectedState, _gameState.State);
        Assert.IsTrue(stateChanged);
    }
}
