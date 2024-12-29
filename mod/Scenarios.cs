using System.Collections.Generic;
using GTA.Math;

namespace template
{
    public static class Scenarios
    {
        public static List<TestScenario> GetAllScenarios()
        {
            var list = new List<TestScenario>();

            list.Add(new TestScenario(
                name: "Test1 - easy drive",
                start: new Vector3(-1034.6f, -2733.6f, 20.0f),
                heading: 240.0f,
                waypointPos: new Vector3(-1267.0f, -3386.0f, 13.9f),
                spawnObstacle: false,
                obstaclePos: Vector3.Zero
            ));

            list.Add(new TestScenario(
                name: "Test2 - with obstacle",
                start: new Vector3(217.0f, -810.0f, 30.0f),
                heading: 90.0f,
                waypointPos: new Vector3(375.0f, -750.0f, 29.0f),
                spawnObstacle: true,
                obstaclePos: new Vector3(300.0f, -780.0f, 28.5f)
            ));

            list.Add(new TestScenario(
                name: "Test3 - long trip",
                start: new Vector3(-500.0f, 500.0f, 70.0f),
                heading: 0.0f,
                waypointPos: new Vector3(1700.0f, 3800.0f, 34.0f),
                spawnObstacle: false,
                obstaclePos: Vector3.Zero
            ));


            return list;
        }
    }
}