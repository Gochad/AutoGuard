using System;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.Math;

namespace template
{
    public class Main : Script
    {
        public Main()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;

            WaypointManager.SetWaypoint(-1034.6f, -2733.6f);
        }

        private void OnTick(object sender, EventArgs e)
        {
            // TODO
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