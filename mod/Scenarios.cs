using System.Collections.Generic;
using GTA.Math;

namespace drivingMod
{
    public static class Scenarios
    {
        public static List<TestScenario> GetAllScenarios()
        {
            var list = new List<TestScenario>();

            list.Add(new TestScenario(
                name: "Test1_easy_drive",
                start: new Vector3(-1034.6f, -2733.6f, 20.0f),
                heading: 240.0f,
                waypointPos: new Vector3(-1034.6f, -2733.6f, 20.0f),
                spawnObstacle: false
            ));

            list.Add(new TestScenario(
                name: "Test2_uphill",
                start: new Vector3(430f, 320f, 103.3f),
                heading: 0.0f,
                waypointPos: new Vector3(535.3f, 1069.7f, 225.1f),
                spawnObstacle: false
            ));

            list.Add(new TestScenario(
                name: "Test3_train_crossing",
                start: new Vector3(367.3f, -581.4f, 28.7f),
                heading: 90.0f,
                waypointPos: new Vector3(799.2f, -1485.7f, 27),
                spawnObstacle: false
            ));

            list.Add(new TestScenario(
                name: "Test4_mountain_road",
                start: new Vector3(1387, -1615.1f, 55.6f),
                heading: 45.0f,
                waypointPos: new Vector3(1729.1f, -1582.9f, 111.8f),
                spawnObstacle: false
            ));

            list.Add(new TestScenario(
                name: "Test5_highway_speed",
                start: new Vector3(857.6f, 111.4f, 70),
                heading: 270.0f,
                waypointPos: new Vector3(2063.8f, 1435.2f, 75.1f),
                spawnObstacle: false
            ));
            
            return list;
        }
    }
}
