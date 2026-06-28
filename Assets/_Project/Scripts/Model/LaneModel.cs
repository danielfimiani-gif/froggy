using System;
using System.Collections.Generic;

public class LaneModel {
    public int RowIndex { get; private set; }
    private List<CarModel> cars = new();
    private float speed;
    private float spawnInterval;
    private float laneLengthMin;
    private float laneLengthMax;
    private float spawnTimer;
    private float carWidth;

    public event Action<CarModel> OnCarSpawned;
    public event Action<CarModel> OnCarDeSpawned;
    public IReadOnlyList<CarModel> Cars => cars;

    public LaneModel(int row, float speed, float spawnInterval,
        float min, float max, float carWidth) {
        this.RowIndex = row;
        this.speed = speed;
        this.spawnInterval = spawnInterval;
        this.laneLengthMin = min;
        this.laneLengthMax = max;
        this.carWidth = carWidth;
    }

    public void Tick(float deltaTime) {
        foreach (var car in Cars)
            car.Tick(deltaTime);

        for (int i = Cars.Count - 1; i >= 0; i--) {
            var car = Cars[i];
            if (car.PositionX > laneLengthMax || car.PositionX < laneLengthMin) {
                cars.RemoveAt(i);
                OnCarDeSpawned?.Invoke(car);
            }
        }

        spawnTimer += deltaTime;
        if (spawnTimer >= spawnInterval) {
            spawnTimer = 0;
            var car = new CarModel();
            car.PositionX = speed > 0 ? laneLengthMin : laneLengthMax;
            car.Speed = this.speed;
            car.Width = this.carWidth;
            cars.Add(car);
            OnCarSpawned?.Invoke(car);
        }
    }
}
