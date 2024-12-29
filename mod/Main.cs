using System;
using System.Windows.Forms;
using GTA;
using DataCollectorNamespace;
using GTA.Math;
using GTA.Native;

namespace template
{
    public class Main : Script
    {
        private Vehicle currentVehicle;
        private DrivingMetricsCollector dataCollector;

        private TestManager testManager;

        private bool hasEnteredVehicleForThisScenario = false;
        private bool isVehicleSpawned = false;
        public Main()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;

            string lidarFilePath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "GTA_LidarData.csv");

            dataCollector = new DrivingMetricsCollector(lidarFilePath);

            testManager = new TestManager();

            var allScenarios = Scenarios.GetAllScenarios();

            testManager.AddScenarios(allScenarios);

            testManager.StartNextScenario();
        }

        private void OnTick(object sender, EventArgs e)
        {
            var currentScenario = testManager.GetCurrentScenario();

            if (currentScenario == null)
            {
                return;
            }
            
            WaypointManager.SetWaypoint(currentScenario.WaypointPosition.X, currentScenario.WaypointPosition.Y);

            if (!isVehicleSpawned)
            {
                Vector3 vehicleSpawnOffset = new Vector3(5f, 0f, 0f);
                Vector3 vehicleSpawnPosition = currentScenario.StartPosition + vehicleSpawnOffset;
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
                        currentVehicle.Heading = currentScenario.StartHeading;
                        currentVehicle.IsPersistent = true;
                        isVehicleSpawned = true;
                    }
                }
            }
            
            if (!hasEnteredVehicleForThisScenario || currentVehicle == null || !currentVehicle.Exists())
            {
                if (Game.Player.Character != null && Game.Player.Character.IsAlive)
                {
                    VehicleManager.EnterNearestVehicleAndDrive();
                    currentVehicle = Game.Player.Character.CurrentVehicle;
                    if (currentVehicle != null)
                    {
                        hasEnteredVehicleForThisScenario = true;
                    }
                }
            }
            else
            {
                dataCollector.CollectMetrics(currentVehicle);

                float distanceToWaypoint = Vector3.Distance(
                    currentVehicle.Position,
                    currentScenario.WaypointPosition
                );

                if (distanceToWaypoint < 30.0f)
                {
                    EndCurrentScenario();
                    testManager.StartNextScenario();
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F7:
                    if (testManager.GetCurrentScenario() != null)
                    {
                        EndCurrentScenario();
                        testManager.StartNextScenario();
                    }
                    break;

                case Keys.F8:
                    ObstacleManager.AddObstacle();
                    break;

                case Keys.F9:
                    VehicleManager.EnterNearestVehicleAndDrive();
                    break;

                case Keys.F10:
                    dataCollector.Close();
                    break;
            }
        }

        private void EndCurrentScenario()
        {
            if (currentVehicle != null && currentVehicle.Exists())
            {
                currentVehicle.Delete();
                currentVehicle = null;
            }

            hasEnteredVehicleForThisScenario = false;
            isVehicleSpawned = false;
        }
    }
}
