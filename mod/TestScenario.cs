using GTA.Math;

namespace template
{
    public class TestScenario
    {
        public string Name { get; set; }
        public Vector3 StartPosition { get; set; }
        public float StartHeading { get; set; }
        public Vector3 WaypointPosition { get; set; }
        public bool SpawnObstacle { get; set; }
        public Vector3 ObstaclePosition { get; set; }

        public TestScenario(string name, Vector3 start, float heading, Vector3 waypointPos, bool spawnObstacle, Vector3 obstaclePos)
        {
            Name = name;
            StartPosition = start;
            StartHeading = heading;
            WaypointPosition = waypointPos;
            SpawnObstacle = spawnObstacle;
            ObstaclePosition = obstaclePos;
        }
    }
}