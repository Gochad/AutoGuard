using System;
using System.Diagnostics;
using System.Windows.Forms;
using GTA;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using DataCollectorNamespace;

namespace drivingMod
{
    public class Main : Script
    {
        private Vehicle currentVehicle;
        private DrivingMetricsCollector dataCollector;
        private TestManager testManager;
        private DateTime lastObstacleSpawnTime = DateTime.Now;

        public Main()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;

            dataCollector = new DrivingMetricsCollector();
            testManager = new TestManager(dataCollector);

            var allScenarios = Scenarios.GetAllScenarios();
            
            var selectedScenarioNames = LoadScenarioSelectionFromFile();
            
            List<TestScenario> chosenScenarios;

            if (selectedScenarioNames.Count == 0) {
                chosenScenarios = allScenarios;
            } else {
                chosenScenarios = allScenarios
                    .Where(s => selectedScenarioNames.Contains(s.Name))
                    .ToList();
            }

            testManager.AddScenarios(chosenScenarios, defaultTimeLimit: 180);

            testManager.OnAllScenariosCompleted += HandleAllScenariosCompleted;

            testManager.StartNextScenario();
        }

        private void OnTick(object sender, EventArgs e)
        {
            var currentScenario = testManager.GetCurrentScenario();
            if (currentScenario == null) return;
            
            currentScenario.PrepareAndExecuteScenario(dataCollector);

            testManager.UpdateTimeLimit();

            if (currentScenario.IsNearWaypoint())
            {
                currentScenario.EndScenario(dataCollector, true);
                testManager.StartNextScenario();
            }
            
            if (testManager.TestInProgress)
            {
                double elapsed = (DateTime.Now - lastObstacleSpawnTime).TotalSeconds;
                switch (currentScenario.SpawnObstacle)
                {
                    case "barrier":
                        if (elapsed >= 10.0)
                        {
                            ObstacleManager.AddObstacle();
                            lastObstacleSpawnTime = DateTime.Now;

                            GTA.UI.Notification.Show("OBSTACLE WAS ADDED!");
                        }
                        break;
                    case "human":
                        if (elapsed >= 40.0)
                        {
                            ObstacleManager.AddPedObstacle();
                            lastObstacleSpawnTime = DateTime.Now;

                            GTA.UI.Notification.Show("PEDESTRIAN WAS ADDED!");
                        }
                        break;
                    case "random":
                        if (elapsed >= 20.0)
                        {
                            ObstacleManager.AddConesInFrontOfPlayer(5);
                            lastObstacleSpawnTime = DateTime.Now;

                            GTA.UI.Notification.Show("RANDOM OBSTACLE WAS ADDED!");
                        }
                        break;
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
                    LogPlayerPosition();
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

        private void LogPlayerPosition()
        {
            var player = Game.Player.Character;
            if (player != null && player.Exists())
            {
                var position = player.Position;
                string positionLog =
                    $"Player Position: X={position.X}, Y={position.Y}, Z={position.Z}";

                System.IO.File.AppendAllText(
                    System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "PlayerPositionLog.txt"
                    ),
                    positionLog + Environment.NewLine
                );
            }
        }
        private void HandleAllScenariosCompleted()
        {
            dataCollector.Close();

            GTA.UI.Notification.Show("Scenarios are done. Game is closing...");

            KillGameProcess();
        }

        private void KillGameProcess()
        {
            Process[] processes = Process.GetProcessesByName("GTA5");
            if (processes.Length > 0)
            {
                processes[0].Kill();
            }
        }
        
        private List<string> LoadScenarioSelectionFromFile()
        {
            var selectedNames = new List<string>();
            
            string filePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "selectedScenarios.txt"
            );
            
            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                        continue;

                    selectedNames.Add(trimmed);
                }
            }
            catch (Exception ex)
            {
                GTA.UI.Notification.Show($"Error reading scenario file: {ex.Message}");
            }

            return selectedNames;
        }
    }
}
