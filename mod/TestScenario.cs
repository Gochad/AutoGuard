using GTA;
using GTA.Math;
using DataCollectorNamespace;
using System;

namespace drivingMod
{
    public class TestScenario
    {
        public string Name { get; set; }
        public Vector3 StartPosition { get; set; }
        public float StartHeading { get; set; }
        public Vector3 WaypointPosition { get; set; }
        public bool SpawnObstacle { get; set; }
        private Vehicle currentVehicle;
        private bool hasEnteredVehicle = false;
        private bool isVehicleSpawned = false;

        public int TimeLimit { get; set; }
        private DateTime? scenarioStartTime;

        public TestScenario(string name, Vector3 start, float heading, Vector3 waypointPos, bool spawnObstacle, int timeLimit)
        {
            Name = name;
            StartPosition = start;
            StartHeading = heading;
            WaypointPosition = waypointPos;
            SpawnObstacle = spawnObstacle;
            TimeLimit = timeLimit;
        }

        public void PrepareAndExecuteScenario(DrivingMetricsCollector dataCollector)
        {
            GTA.UI.Screen.ShowSubtitle("exectution", 5000);
            scenarioStartTime = DateTime.Now;
            SetWaypoint();
            SpawnVehicle();
            EnterVehicle();
            
            CollectMetrics(dataCollector);

            // while (!IsNearWaypoint())
            // {
            //     
            //     CheckTimeLimit(dataCollector);
            //     Script.Wait(100);
            // }

            // EndScenario(dataCollector);
        }

        public void SetWaypoint()
        {
            WaypointManager.SetWaypoint(WaypointPosition.X, WaypointPosition.Y);
        }

        public void SpawnVehicle()
        {
            if (!isVehicleSpawned)
            {
                Vector3 vehicleSpawnOffset = new Vector3(5f, 0f, 0f);
                Vector3 vehicleSpawnPosition = StartPosition + vehicleSpawnOffset;
                Model vehicleModel = new Model(VehicleHash.SultanRS);

                if (!vehicleModel.IsLoaded)
                {
                    vehicleModel.Request();
                }

                if (vehicleModel.IsLoaded)
                {
                    currentVehicle = World.CreateVehicle(vehicleModel, vehicleSpawnPosition);
                    if (currentVehicle != null && currentVehicle.Exists())
                    {
                        currentVehicle.Heading = StartHeading;
                        currentVehicle.IsPersistent = true;
                        isVehicleSpawned = true;
                    }
                }
            }
        }

        public void EnterVehicle()
        {
            if (!hasEnteredVehicle || currentVehicle == null || !currentVehicle.Exists())
            {
                if (Game.Player.Character != null && Game.Player.Character.IsAlive)
                {
                    VehicleManager.EnterNearestVehicleAndDrive();
                    currentVehicle = Game.Player.Character.CurrentVehicle;
                    if (currentVehicle != null)
                    {
                        hasEnteredVehicle = true;
                    }
                }
            }
        }

        public void CollectMetrics(DrivingMetricsCollector dataCollector)
        {
            if (currentVehicle != null && currentVehicle.Exists())
            {
                dataCollector.CollectMetrics(currentVehicle);
            }
        }

        public bool IsNearWaypoint()
        {
            if (currentVehicle == null || !currentVehicle.Exists())
            {
                return false;
            }

            float distanceToWaypoint = Vector3.Distance(currentVehicle.Position, WaypointPosition);
            return distanceToWaypoint < 30.0f;
        }

        public void CheckTimeLimit(DrivingMetricsCollector dataCollector)
        {
            if (scenarioStartTime.HasValue)
            {
                double elapsedTime = (DateTime.Now - scenarioStartTime.Value).TotalSeconds;

                if (elapsedTime > TimeLimit)
                {
                    EndScenario(dataCollector);
                }
            }
        }

        public void EndScenario(DrivingMetricsCollector dataCollector)
        {
            if (IsNearWaypoint())
            {
                dataCollector.MarkScenarioAsCompleted();
            }

            if (currentVehicle != null && currentVehicle.Exists())
            {
                currentVehicle.Delete();
                currentVehicle = null;
            }

            hasEnteredVehicle = false;
            isVehicleSpawned = false;

            dataCollector.Close();
        }
    }
}
