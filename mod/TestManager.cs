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
        private DrivingMetricsCollector metricsCollector;

        public bool TestInProgress { get; private set; } = false;
        public event Action OnAllScenariosCompleted;

        private Dictionary<TestScenario, int> scenarioTimeLimits = new Dictionary<TestScenario, int>();

        private DateTime scenarioStartTime;
        private bool scenarioRunning = false;

        public TestManager(DrivingMetricsCollector collector)
        {
            metricsCollector = collector ?? throw new ArgumentNullException(nameof(collector));
        }

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

                string scenarioName = currentScenario.Name;
                string filePath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    $"GTA_Data_{scenarioName}.csv"
                );

                metricsCollector.SetOutputFile(filePath);
            }
            else
            {
                currentScenario = null;
                TestInProgress = false;
                scenarioRunning = false;

                OnAllScenariosCompleted?.Invoke();
            }
        }
        
        public void CompleteScenario()
        {
            EndCurrentScenario(true);
            StartNextScenario();
        }

        public void UpdateTimeLimit()
        {
            if (!scenarioRunning || currentScenario == null) return;

            double elapsed = (DateTime.Now - scenarioStartTime).TotalSeconds;
            int limit = scenarioTimeLimits[currentScenario];

            if (elapsed > limit)
            {
                EndCurrentScenario(false);
                StartNextScenario();
            }
        }

        private void EndCurrentScenario(bool success)
        {
            if (currentScenario != null)
            {
                currentScenario.EndScenario(metricsCollector, success);
                metricsCollector.Close();
                scenarioRunning = false;
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
