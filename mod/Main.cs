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
            
            SetWaypoint(-1034.6f, -2733.6f);
        }

        private void OnTick(object sender, EventArgs e)
        {
            // TODO
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F7)
            {
                SetWaypoint(-1034.6f, -2733.6f);
            }

            if (e.KeyCode == Keys.F8)
            {
                AddObstacle();
            }

            if (e.KeyCode == Keys.F9)
            {
                EnterNearestVehicleAndDrive();
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
            Model obstacleModel = new Model("prop_barrier_work01a");

            if (!obstacleModel.IsLoaded) obstacleModel.Request(1000);

            if (obstacleModel.IsLoaded)
            {
                World.CreateProp(obstacleModel, obstaclePosition, Game.Player.Character.Rotation, true, false);
            }
        }

        private void EnterNearestVehicleAndDrive()
        {
            Vehicle nearestVehicle = GetNearestCar(Game.Player.Character.Position, 50.0f);

            if (nearestVehicle != null)
            {
                Function.Call(Hash.TASK_ENTER_VEHICLE, Game.Player.Character.Handle, nearestVehicle.Handle, -1, -1, 2.0f, 1, 0);
                
                Game.Player.Character.Task.EnterVehicle(nearestVehicle, VehicleSeat.Driver);
                Wait(5000);
                
                DriveForward(nearestVehicle);
            }
        }

        private void DriveForward(Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Driver == Game.Player.Character)
            {
                float speedInMetersPerSecond = 15.0f / 3.6f;

                Function.Call(Hash.TASK_VEHICLE_DRIVE_WANDER, Game.Player.Character.Handle, vehicle.Handle, speedInMetersPerSecond, 786603);

                Wait(5000); 
            }
        }

        private Vehicle GetNearestCar(Vector3 position, float radius)
        {
            Vehicle[] vehicles = World.GetNearbyVehicles(Game.Player.Character, radius);
            Vehicle nearestVehicle = null;
            float nearestDistance = float.MaxValue;

            foreach (Vehicle vehicle in vehicles)
            {
                if (vehicle != null && !IsMotorcycle(vehicle))
                {
                    float distance = vehicle.Position.DistanceTo(position);

                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestVehicle = vehicle;
                    }
                }
            }

            return nearestVehicle;
        }

        private bool IsMotorcycle(Vehicle vehicle)
        {
            return Function.Call<bool>(Hash.IS_THIS_MODEL_A_BIKE, vehicle.Model.Hash);
        }
    }
    
}