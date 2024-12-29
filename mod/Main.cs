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

            currentScenario.SetWaypoint();
            currentScenario.SpawnVehicle();
            currentScenario.EnterVehicle();
            currentScenario.CollectMetrics(dataCollector);

            if (currentScenario.IsNearWaypoint())
            {
                currentScenario.EndScenario();
                testManager.StartNextScenario();
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
        }
    }
}
