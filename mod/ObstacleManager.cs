using System;
using GTA;
using GTA.Math;

namespace drivingMod
{
    public static class ObstacleManager
    {
        public static void AddObstacle()
        {
            Vector3 playerPosition = Game.Player.Character.Position;
            Vector3 obstaclePosition = playerPosition + Game.Player.Character.ForwardVector * 5;
            
            Model obstacleModel = new Model("prop_barrier_work04a");

            if (!obstacleModel.IsLoaded)
            {
                obstacleModel.Request();

                int timeout = 0;
                while (!obstacleModel.IsLoaded && timeout < 50)
                {
                    timeout++;
                }
            }

            if (obstacleModel.IsLoaded)
            {
                World.CreateProp(obstacleModel, obstaclePosition, false, false);
            }
        }
        
        public static void AddPedObstacle()
        {
            Vector3 spawnPosition = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 5;

            Model pedModel = new Model(PedHash.Business01AMM);

            if (!pedModel.IsLoaded)
            {
                pedModel.Request();
                int timeout = 0;
                while (!pedModel.IsLoaded && timeout < 50)
                {
                    Script.Yield();
                    timeout++;
                }
            }

            if (pedModel.IsLoaded)
            {
                Ped newPed = World.CreatePed(pedModel, spawnPosition);
            }
        }
        
        public static void AddConesInFrontOfPlayer(int count, float spacing = 2.0f)
        {
            Model coneModel = new Model("prop_roadcone02a");
            if (!coneModel.IsLoaded) coneModel.Request(1000);

            if (coneModel.IsLoaded)
            {
                var playerPos = Game.Player.Character.Position;
                var forwardVector = Game.Player.Character.ForwardVector;

                for (int i = 0; i < count; i++)
                {
                    Vector3 spawnPos = playerPos + forwardVector * (i + 1) * spacing;

                    World.CreateProp(coneModel, spawnPos, false, false);
                }
            }
        }
    }
}