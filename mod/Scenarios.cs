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
                name: "Test2_with_obstacle",
                start: new Vector3(217.0f, -810.0f, 30.0f),
                heading: 90.0f,
                waypointPos: new Vector3(375.0f, -750.0f, 29.0f),
                spawnObstacle: true
            ));
            
            return list;
        }
    }
}