﻿using System;
using System.IO;
using System.Windows.Forms;
using GTA;
using DataCollectorNamespace;

namespace drivingMod
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

            dataCollector = new DrivingMetricsCollector();

            testManager = new TestManager();

            var allScenarios = Scenarios.GetAllScenarios();

            testManager.AddScenarios(allScenarios);

            testManager.StartNextScenario();

            UpdateDataCollectorFile();
        }

        private void OnTick(object sender, EventArgs e)
        {
            var currentScenario = testManager.GetCurrentScenario();

            if (currentScenario == null)
            {
                return;
            }

            currentScenario.PrepareAndExecuteScenario(dataCollector);

            if (currentScenario.IsNearWaypoint())
            {
                currentScenario.EndScenario();
                testManager.StartNextScenario();
                UpdateDataCollectorFile();
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
                        UpdateDataCollectorFile();
                    }
                    break;

                case Keys.F8:
                    LogPlayerPosition();
                    //ObstacleManager.AddObstacle();
                    break;

                case Keys.F9:
                    VehicleManager.EnterNearestVehicleAndDrive();
                    break;

                case Keys.F10:
                    dataCollector.Close();
                    break;
            }
        }
        
        private void LogPlayerPosition()
        {
            var player = Game.Player.Character;

            if (player != null && player.Exists())
            {
                var position = player.Position;
                string positionLog = $"Player Position: X={position.X}, Y={position.Y}, Z={position.Z}";

                string logFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "PlayerPositionLog.txt");

                File.AppendAllText(logFilePath, positionLog + Environment.NewLine);
            }
        }

        private void UpdateDataCollectorFile()
        {
            var currentScenario = testManager.GetCurrentScenario();

            if (currentScenario != null)
            {
                string scenarioName = currentScenario.Name;
                string scenarioFilePath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    $"GTA_Data_{scenarioName}.csv");

                dataCollector.SetOutputFile(scenarioFilePath);
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
