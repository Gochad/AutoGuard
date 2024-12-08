using System;
using System.Windows.Forms;
using GTA;

namespace template
{
    public class Main : Script
    {
        private bool hasEnteredVehicle = false;

        public Main()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;

            WaypointManager.SetWaypoint(-1034.6f, -2733.6f);
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (!hasEnteredVehicle)
            {
                if (Game.Player.Character != null && Game.Player.Character.IsAlive)
                {
                    VehicleManager.EnterNearestVehicleAndDrive();
                    hasEnteredVehicle = true;
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F7)
            {
                WaypointManager.SetWaypoint(-1034.6f, -2733.6f);
            }

            if (e.KeyCode == Keys.F8)
            {
                ObstacleManager.AddObstacle();
            }

            if (e.KeyCode == Keys.F9)
            {
                VehicleManager.EnterNearestVehicleAndDrive();
            }
        }
    }
}