using System;

public static class CollisionDetector {
  public static bool OverLaps(float carX, float carWidth, float playerX, float playerHalfWidth) {
    return Math.Abs(carX - playerX) < (carWidth / 2f) + playerHalfWidth;
  }
}
