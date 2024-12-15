using System;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using DataCollectorNamespace;
using GTA.Math;

namespace template
{
    public class Main : Script
    {
        private bool hasEnteredVehicle = false;
        private Vehicle currentVehicle;
        private DataCollector dataCollector;
        private LidarCollector lidarCollector;

        public Main()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;
            
            WaypointManager.SetWaypoint(-1034.6f, -2733.6f);
            
            string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GTA_Data.csv");
            dataCollector = new DataCollector(filePath);
            
            string lidarFilePath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                "GTA_LidarData.csv");
            lidarCollector = new LidarCollector(lidarFilePath);
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (!hasEnteredVehicle)
            {
                if (Game.Player.Character != null && Game.Player.Character.IsAlive)
                {
                    VehicleManager.EnterNearestVehicleAndDrive();
                    currentVehicle = Game.Player.Character.CurrentVehicle;
                    hasEnteredVehicle = true;
                }
            }

            if (hasEnteredVehicle && currentVehicle != null)
            {
                dataCollector.CollectData(currentVehicle);
                lidarCollector.CollectLidarData(currentVehicle);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F7:
                    WaypointManager.SetWaypoint(-1034.6f, -2733.6f);
                    break;

                case Keys.F8:
                    ObstacleManager.AddObstacle();
                    break;

                case Keys.F9:
                    VehicleManager.EnterNearestVehicleAndDrive();
                    break;

                case Keys.F10:
                    dataCollector.Close();
                    lidarCollector.Close();
                    break;
            }
        }
    }
}
