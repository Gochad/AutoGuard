using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
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

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_CLOSE = 0x0010;

        public Main()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;

            dataCollector = new DrivingMetricsCollector();
            testManager = new TestManager();

            var allScenarios = Scenarios.GetAllScenarios();
            testManager.AddScenarios(allScenarios);

            testManager.OnAllScenariosCompleted += HandleAllScenariosCompleted;

            testManager.StartNextScenario();

            UpdateDataCollectorFile();
        }

        private void OnTick(object sender, EventArgs e)
        {
            var currentScenario = testManager.GetCurrentScenario();
            if (currentScenario == null) return;

            currentScenario.PrepareAndExecuteScenario(dataCollector);

            if (currentScenario.IsNearWaypoint())
            {
                currentScenario.EndScenario(dataCollector);
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

        private void UpdateDataCollectorFile()
        {
            var currentScenario = testManager.GetCurrentScenario();
            if (currentScenario != null)
            {
                string scenarioName = currentScenario.Name;
                string scenarioFilePath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    $"GTA_Data_{scenarioName}.csv"
                );

                dataCollector.SetOutputFile(scenarioFilePath);
            }
        }

        private void HandleAllScenariosCompleted()
        {
            dataCollector.Close();

            GTA.UI.Notification.Show("Scenarios are done. Game is closing...");

            CloseGameWindow();

            KillGameProcess();
        }

        private void CloseGameWindow()
        {
            Process[] processes = Process.GetProcessesByName("GTA5");
            if (processes.Length > 0)
            {
                Process gtaProcess = processes[0];
                IntPtr hWnd = gtaProcess.MainWindowHandle;
                PostMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }

        private void KillGameProcess()
        {
            Process[] processes = Process.GetProcessesByName("GTA5");
            if (processes.Length > 0)
            {
                processes[0].Kill();
            }
        }
    }
}