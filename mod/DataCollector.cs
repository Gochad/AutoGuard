using System;
using System.IO;
using GTA;
using GTA.Math;
using GTA.Native;

namespace DataCollectorNamespace
{
    public class DataCollector
    {
        private StreamWriter dataWriter;

        public DataCollector(string filePath)
        {
            dataWriter = new StreamWriter(filePath, false);
            dataWriter.WriteLine("Time,PositionX,PositionY,PositionZ,Speed,Heading");
        }

        public void CollectData(Vehicle vehicle)
        {
            if (vehicle != null)
            {
                Vector3 position = vehicle.Position;
                float speed = vehicle.Speed;
                float heading = vehicle.Heading;

                string dataLine = $"{DateTime.Now},{position.X},{position.Y},{position.Z},{speed},{heading}";
                dataWriter.WriteLine(dataLine);
            }
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
    
    public class LidarCollector
    {
        private StreamWriter dataWriter;
        private int raysPer360 = 360;
        private float maxDistance = 100f;
        private Vector3 sensorOffset = new Vector3(0, 0, 1);

        public LidarCollector(string filePath)
        {
            dataWriter = new StreamWriter(filePath, false);
            dataWriter.WriteLine("Time,SensorPosX,SensorPosY,SensorPosZ,Angle,HitDistance,HitEntity");
        }

        public void CollectLidarData(Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists())
            {
                Vector3 sensorPosition = vehicle.Position + sensorOffset;
                DateTime now = DateTime.Now;

                for (int i = 0; i < raysPer360; i++)
                {
                    float angleDeg = i;
                    float angleRad = angleDeg * (float)(Math.PI / 180.0);
                    Vector3 direction = new Vector3((float)Math.Cos(angleRad), (float)Math.Sin(angleRad), 0f);

                    RaycastResult result = World.Raycast(sensorPosition, sensorPosition + direction * maxDistance, IntersectFlags.Everything);
                    
                    float hitDistance = maxDistance;
                    string hitEntity = "None";

                    if (result.DidHit)
                    {
                        hitDistance = sensorPosition.DistanceTo(result.HitPosition);

                        if (result.HitEntity != null)
                        {
                            if (result.HitEntity is Ped)
                                hitEntity = "Ped";
                            else if (result.HitEntity is Vehicle)
                                hitEntity = "Vehicle";
                            else
                                hitEntity = "Object";
                        }
                        else
                        {
                            hitEntity = "WorldCollision";
                        }
                    }

                    string line = $"{now},{sensorPosition.X},{sensorPosition.Y},{sensorPosition.Z},{angleDeg},{hitDistance},{hitEntity}";
                    dataWriter.WriteLine(line);
                }
            }
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
