using NUnit.Framework;

public class CarModelTests {
    [TestCase(4.0f, 10.0f, 5f)]
    [TestCase(5.0f, -10.0f, 6f)]
    [TestCase(0.0f, 5.0f, 15f)]
    [TestCase(4.0f, -5.0f, -5f)]
    public void NewCarModel_GivenValues_ShouldReturnExactValue(float posX, float speed, float width) {
        //Act
        var carModel = new CarModel {
            PositionX = posX,
            Speed = speed,
            Width = width,
        };

        //Assert
        Assert.AreEqual(posX, carModel.PositionX);
        Assert.AreEqual(speed, carModel.Speed);
        Assert.AreEqual(width, carModel.Width);
    }

    [TestCase(1.0f, ExpectedResult = 12.0f)]
    [TestCase(-1.0f, ExpectedResult = 8.0f)]
    public float CarModel_WhenTick_ShouldIncreaseOrDecreaseXPosition(float speed) {
        //Arrange
        float delta = 2.0f;
        var carModel = new CarModel {
            PositionX = 10.0f,
            Speed = speed,
            Width = 5.0f,
        };

        //Act
        carModel.Tick(delta);

        //Assert
        return carModel.PositionX;
    }
}
