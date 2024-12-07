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
        }

        private void OnTick(object sender, EventArgs e)
        {
            // TODO
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F8)
            {
                SetWaypoint(-1034.6f, -2733.6f);
            }

            if (e.KeyCode == Keys.F9) 
            {
                AddObstacle();
            }
        }

        private void SetWaypoint(float x, float y)
        {
            Function.Call(Hash.SET_NEW_WAYPOINT, x, y);
        }

        private void AddObstacle()
        {
            Vector3 playerPosition = Game.Player.Character.Position;

            Vector3 obstaclePosition = playerPosition + Game.Player.Character.ForwardVector * 5;

            Model obstacleModel = new Model("prop_container_01a");

            if (!obstacleModel.IsLoaded) obstacleModel.Request(1000);

            if (obstacleModel.IsLoaded)
            {
                World.CreateProp(obstacleModel, obstaclePosition, Game.Player.Character.Rotation, true, false);
            }
        }
    }
}