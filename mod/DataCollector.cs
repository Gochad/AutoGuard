using System;
using System.IO;
using GTA;
using GTA.Math;
using GTA.Native;

namespace DataCollectorNamespace
{
    public class DrivingMetricsCollector
    {
        private StreamWriter dataWriter;
        private Vector3 lastPosition;
        private float lastSpeed;
        private DateTime lastUpdateTime;
        private int laneDepartureCount = 0;
        private DateTime lastLoggedTime = DateTime.MinValue;

        public void SetOutputFile(string filePath)
        {
            Close();
            dataWriter = new StreamWriter(filePath, false);
            dataWriter.WriteLine("Time;PositionX;PositionY;PositionZ;Speed;SpeedDeviation;Jerk;SteeringAngle;LaneOffset;LaneDepartures;TrafficViolations;CollisionDetected");
            lastPosition = Vector3.Zero;
            lastSpeed = 0f;
            lastUpdateTime = DateTime.Now;
        }

        public void CollectMetrics(Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists())
            {
                DateTime now = DateTime.Now;
                if ((now - lastLoggedTime).TotalSeconds < 1)
                {
                    return;
                }

                lastLoggedTime = now;

                Vector3 currentPosition = vehicle.Position;
                float currentSpeed = vehicle.Speed;
                float speedLimit;
                Vector3 vehiclePosition = vehicle.Position;
                string zoneName = Function.Call<string>(Hash.GET_NAME_OF_ZONE, vehiclePosition.X, vehiclePosition.Y, vehiclePosition.Z);

                if (zoneName == "HIGHWAY")
                    speedLimit = 120f / 3.6f;
                else if (zoneName == "CITY")
                    speedLimit = 50f / 3.6f;
                else
                    speedLimit = 30f / 3.6f;


                float laneOffset = CalculateLaneOffset(vehicle);
                bool laneDeparture = Math.Abs(laneOffset) > 1.5f;

                if (laneDeparture)
                    laneDepartureCount++;

                float speedDeviation = Math.Abs(currentSpeed - speedLimit);

                TimeSpan deltaTime = now - lastUpdateTime;
                float deltaSpeed = currentSpeed - lastSpeed;
                float jerk = deltaSpeed / (float)deltaTime.TotalSeconds;

                float steeringAngle = vehicle.SteeringAngle;

                int trafficViolations = CheckTrafficViolations(vehicle);

                bool collisionDetected = CheckCollisions(vehicle);

                string line = $"{now};{currentPosition.X};{currentPosition.Y};{currentPosition.Z};{currentSpeed};{speedDeviation};{jerk};{steeringAngle};{laneOffset};{laneDepartureCount};{trafficViolations};{collisionDetected}";
                dataWriter.WriteLine(line);

                lastPosition = currentPosition;
                lastSpeed = currentSpeed;
                lastUpdateTime = now;
            }
        }

        private float CalculateLaneOffset(Vehicle vehicle)
        {
            Vector3 vehiclePosition = vehicle.Position;
            Vector3 forwardVector = vehicle.ForwardVector;

            Vector3 rayStart = vehiclePosition + new Vector3(0, 0, 0.5f);
            Vector3 rayEnd = rayStart + forwardVector * 10f;

            RaycastResult rayResult = World.Raycast(rayStart, rayEnd, IntersectFlags.Everything);

            if (rayResult.DidHit)
            {
                Vector3 hitPosition = rayResult.HitPosition;

                return vehiclePosition.DistanceTo(hitPosition);
            }

            return 0f;
        }

        private int CheckTrafficViolations(Vehicle vehicle)
        {
            int violationCount = 0;

            if (IsAtTrafficLight(vehicle) && vehicle.Speed > 0.5f)
            {
                violationCount++;
            }

            if (IsAtStopSign(vehicle) && vehicle.Speed > 0.5f)
            {
                violationCount++;
            }

            return violationCount;
        }

        private bool IsAtTrafficLight(Vehicle vehicle)
        {
            Vector3 rayStart = vehicle.Position + new Vector3(0, 0, 1f);
            Vector3 rayEnd = rayStart + vehicle.ForwardVector * 10f;

            RaycastResult result = World.Raycast(rayStart, rayEnd, IntersectFlags.Everything);

            return result.DidHit && result.HitEntity != null;
        }

        private bool IsAtStopSign(Vehicle vehicle)
        {
            Vector3 rayStart = vehicle.Position + vehicle.ForwardVector * 5f;
            Vector3 rayEnd = rayStart - new Vector3(0, 0, 2f);

            RaycastResult result = World.Raycast(rayStart, rayEnd, IntersectFlags.Everything);

            return result.DidHit && result.HitEntity != null;
        }

        private bool CheckCollisions(Vehicle vehicle)
        {
            return Function.Call<bool>(Hash.HAS_ENTITY_COLLIDED_WITH_ANYTHING, vehicle);
        }

        public void Close()
        {
            if (dataWriter != null)
            {
                dataWriter.Close();
                dataWriter = null;
            }
        }
    }
}