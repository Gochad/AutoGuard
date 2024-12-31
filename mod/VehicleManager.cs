using GTA;
using GTA.Math;
using GTA.Native;

namespace drivingMod
{
    public static class VehicleManager
    {
        public static void EnterNearestVehicleAndDrive()
        {
            Vehicle nearestVehicle = GetNearestCar(Game.Player.Character.Position, 50.0f);

            if (nearestVehicle != null)
            {
                Function.Call(Hash.SET_PED_PATH_CAN_USE_CLIMBOVERS, Game.Player.Character.Handle, true);
                Function.Call(Hash.SET_PED_PATH_CAN_USE_LADDERS, Game.Player.Character.Handle, true);
                Function.Call(Hash.SET_PED_PATH_CAN_DROP_FROM_HEIGHT, Game.Player.Character.Handle, true);
                Function.Call(Hash.SET_PED_PATH_AVOID_FIRE, Game.Player.Character.Handle, false);

                Vector3 driverDoorPosition = GetDriverDoorPosition(nearestVehicle);
                Function.Call(Hash.TASK_GO_STRAIGHT_TO_COORD, Game.Player.Character.Handle, driverDoorPosition.X, driverDoorPosition.Y, driverDoorPosition.Z, 2.0f, -1, nearestVehicle.Heading, 0);

                Script.Wait(3000);

                if (nearestVehicle.IsSeatFree(VehicleSeat.Driver))
                {
                    OpenNearestVehicleDoor(nearestVehicle);

                    Game.Player.Character.Task.EnterVehicle(nearestVehicle, VehicleSeat.Driver);

                    Script.Wait(5000);

                    Function.Call(Hash.SET_PED_PATH_CAN_USE_CLIMBOVERS, Game.Player.Character.Handle, false);
                    Function.Call(Hash.SET_PED_PATH_CAN_USE_LADDERS, Game.Player.Character.Handle, false);
                    Function.Call(Hash.SET_PED_PATH_CAN_DROP_FROM_HEIGHT, Game.Player.Character.Handle, false);

                    DriveForward(nearestVehicle);
                }
            }
        }

        private static void OpenNearestVehicleDoor(Vehicle vehicle)
        {
            if (vehicle != null)
            {
                Function.Call(Hash.TASK_OPEN_VEHICLE_DOOR, Game.Player.Character.Handle, vehicle.Handle, 0, false);
                Script.Wait(2000);
            }
        }
        
        private static Vector3 GetDriverDoorPosition(Vehicle vehicle)
        {
            Vector3 offset = vehicle.RightVector * -1.0f;
            Vector3 position = vehicle.Position + offset;
            position.Z += 0.5f;
            return position;
        }

        private static void DriveForward(Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Driver == Game.Player.Character)
            {
                float speedInMetersPerSecond = 20.0f;

                Function.Call(Hash.TASK_VEHICLE_DRIVE_WANDER, Game.Player.Character.Handle, vehicle.Handle, speedInMetersPerSecond, 262144);

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
