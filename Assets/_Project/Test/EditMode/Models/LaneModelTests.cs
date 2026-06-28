using System.Linq;
using NUnit.Framework;

public class LaneModelTests {
    [Test]
    public void NewLaneModel_ShouldCreateNewInstance() {
        //Arrange
        bool carSpawned = false;
        bool carDeSpawned = false;

        //Act
        var lane = new LaneModel(row: 1, speed: 5f, spawnInterval: 1f, min: -10f, max: 10f, carWidth: 1f);
        lane.OnCarSpawned += (c) => carSpawned = true;
        lane.OnCarDeSpawned += (c) => carDeSpawned = true;

        //Assert
        Assert.NotNull(lane);
        Assert.IsEmpty(lane.Cars);
        Assert.IsFalse(carSpawned);
        Assert.IsFalse(carDeSpawned);
    }

    [Test]
    public void Tick_WhenSpawnIntervalReached_ShouldSpawnCarInstance() {
        //Arrange
        float deltaTime = 1.0f;
        bool carSpawned = false;
        int expectedCarLengt = 1;
        CarModel spawnedCar = null;
        var lane = new LaneModel(row: 1, speed: 5f, spawnInterval: 1f, min: -10f, max: 10f, carWidth: 1f);
        lane.OnCarSpawned += car => {
            carSpawned = true;
            spawnedCar = car;
        };

        //Act
        lane.Tick(deltaTime);

        //Assert
        Assert.IsTrue(carSpawned);
        Assert.NotNull(spawnedCar);
        Assert.AreEqual(expectedCarLengt, lane.Cars.Count);
    }

    [Test]
    public void Tick_WhenCardReachLaneLimt_ShouldDesPawnCar() {
        //Arrange
        bool carSpawned = false;
        bool carDeSpawned = false;
        int expectedCarLengt = 0;
        CarModel spawnedCar = null;
        var lane = new LaneModel(row: 1, speed: 5f, spawnInterval: 1f, min: -2f, max: 2f, carWidth: 1f);
        lane.OnCarSpawned += car => {
            carSpawned = true;
            spawnedCar = car;
        };
        lane.OnCarDeSpawned += car => carDeSpawned = true;

        //Act
        lane.Tick(1f);
        lane.Tick(0.9f);

        //Assert
        Assert.IsTrue(carSpawned);
        Assert.IsTrue(carDeSpawned);
        Assert.NotNull(spawnedCar);
        Assert.AreEqual(expectedCarLengt, lane.Cars.Count);
        Assert.IsFalse(lane.Cars.Contains(spawnedCar));
    }
}
