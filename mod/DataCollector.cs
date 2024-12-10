using System;
using System.IO;
using GTA;
using GTA.Math;

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
}
