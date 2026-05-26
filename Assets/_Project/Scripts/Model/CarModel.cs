class CarModel {
    public float PositionX;
    public float Speed;
    public float Width;

    public void Tick(float deltaTime) {
        PositionX += Speed * deltaTime;
    }
}