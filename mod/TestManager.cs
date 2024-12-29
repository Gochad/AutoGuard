using System.Collections.Generic;
using GTA;
using GTA.Math;

namespace template
{
    public class TestManager
    {
        private Queue<TestScenario> scenariosQueue = new Queue<TestScenario>();
        private TestScenario currentScenario = null;

        public bool TestInProgress { get; private set; } = false;

        public void AddScenario(TestScenario scenario)
        {
            scenariosQueue.Enqueue(scenario);
        }

        public void AddScenarios(IEnumerable<TestScenario> scenarioList)
        {
            foreach (var scenario in scenarioList)
            {
                scenariosQueue.Enqueue(scenario);
            }
        }

        public void StartNextScenario()
        {
            if (scenariosQueue.Count > 0)
            {
                currentScenario = scenariosQueue.Dequeue();
                SetupScenario(currentScenario);
                TestInProgress = true;
            }
            else
            {
                currentScenario = null;
                TestInProgress = false;
            }
        }

        public TestScenario GetCurrentScenario()
        {
            return currentScenario;
        }

        private void SetupScenario(TestScenario scenario)
        {
            Game.Player.Character.Position = scenario.StartPosition;
            Game.Player.Character.Heading = scenario.StartHeading;

            WaypointManager.SetWaypoint(scenario.WaypointPosition.X, scenario.WaypointPosition.Y);

            if (scenario.SpawnObstacle)
            {
                ObstacleManager.AddObstacle();
            }
        }
    }
}