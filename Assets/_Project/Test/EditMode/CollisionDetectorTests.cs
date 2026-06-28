using NUnit.Framework;

public class CollissionDetectorTests {
  [TestCase(0f, 1f, 0f, 0.4f, ExpectedResult = true)]
  [TestCase(0.5f, 1f, 0f, 0.4f, ExpectedResult = true)]
  [TestCase(1.5f, 1f, 0f, 0.4f, ExpectedResult = false)]
  [TestCase(-1.5f, 1f, 0f, 0.4f, ExpectedResult = false)]
  public bool OverLaps_ReturnsExpected(float carX, float carWidth, float playerX, float playerHalfWidth) {
    return CollisionDetector.OverLaps(carX, carWidth, playerX, playerHalfWidth);
  }
}
