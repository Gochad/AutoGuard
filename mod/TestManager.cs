using System;
using System.Collections.Generic;
using DataCollectorNamespace;
using GTA;

namespace drivingMod
{
    public class TestManager
    {
        private Queue<TestScenario> scenariosQueue = new Queue<TestScenario>();
        private TestScenario currentScenario = null;

        public bool TestInProgress { get; private set; } = false;
        public event Action OnAllScenariosCompleted;

        private Dictionary<TestScenario, int> scenarioTimeLimits = new Dictionary<TestScenario, int>();

        private DateTime scenarioStartTime;
        private bool scenarioRunning = false;

        public void AddScenarios(IEnumerable<TestScenario> scenarioList, int defaultTimeLimit = 180)
        {
            foreach (var scenario in scenarioList)
            {
                scenariosQueue.Enqueue(scenario);
                scenarioTimeLimits[scenario] = defaultTimeLimit; 
            }
        }

        public TestScenario GetCurrentScenario()
        {
            return currentScenario;
        }

        public void StartNextScenario()
        {
            if (scenariosQueue.Count > 0)
            {
                currentScenario = scenariosQueue.Dequeue();
                SetupScenario(currentScenario);

                TestInProgress = true;
                scenarioRunning = true;
                scenarioStartTime = DateTime.Now;
            }
            else
            {
                currentScenario = null;
                TestInProgress = false;
                scenarioRunning = false;

                OnAllScenariosCompleted?.Invoke();
            }
        }
        public void UpdateTimeLimit(DrivingMetricsCollector dataCollector)
        {
            if (!scenarioRunning || currentScenario == null)
                return;
            
            double elapsed = (DateTime.Now - scenarioStartTime).TotalSeconds;

            int limit = scenarioTimeLimits[currentScenario];

            if (elapsed > limit)
            {
                currentScenario.EndScenario(dataCollector);

                scenarioRunning = false;
                TestInProgress = false;

                StartNextScenario();
            }
        }

        private void SetupScenario(TestScenario scenario)
        {
            Game.Player.Character.Position = scenario.StartPosition;
            Game.Player.Character.Heading = scenario.StartHeading;

            WaypointManager.SetWaypoint(
                scenario.WaypointPosition.X, 
                scenario.WaypointPosition.Y
            );

            if (scenario.SpawnObstacle)
            {
                ObstacleManager.AddObstacle();
            }
        }
    }
}
