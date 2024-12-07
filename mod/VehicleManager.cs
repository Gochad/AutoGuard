using GTA;
using GTA.Math;
using GTA.Native;

namespace template
{
    public static class VehicleManager
    {
        public static void EnterNearestVehicleAndDrive()
        {
            Vehicle nearestVehicle = GetNearestCar(Game.Player.Character.Position, 50.0f);

            if (nearestVehicle != null)
            {
                Function.Call(Hash.TASK_GO_TO_ENTITY, Game.Player.Character.Handle, nearestVehicle.Handle, -1, 5.0f, 4.0f, 0, 0);

                Script.Wait(5000);
                
                Function.Call(Hash.TASK_ENTER_VEHICLE, Game.Player.Character.Handle, nearestVehicle.Handle, -1, -1, 2.0f, 1, 0);
                
                Game.Player.Character.Task.EnterVehicle(nearestVehicle, VehicleSeat.Driver);
                Script.Wait(5000);
                
                DriveForward(nearestVehicle);
            }
        }

        private static void DriveForward(Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Driver == Game.Player.Character)
            {
                float speedInMetersPerSecond = 15.0f / 3.6f;

                Function.Call(Hash.TASK_VEHICLE_DRIVE_WANDER, Game.Player.Character.Handle, vehicle.Handle, speedInMetersPerSecond, 786603);

                Script.Wait(5000); 
            }
        }

        private static Vehicle GetNearestCar(Vector3 position, float radius)
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

        private static bool IsMotorcycle(Vehicle vehicle)
        {
            return Function.Call<bool>(Hash.IS_THIS_MODEL_A_BIKE, vehicle.Model.Hash);
        }
    }
}
