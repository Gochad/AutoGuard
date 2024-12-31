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
            Model obstacleModel = new Model("prop_barrier_work01a");

            if (!obstacleModel.IsLoaded) obstacleModel.Request(1000);

            if (obstacleModel.IsLoaded)
            {
                World.CreateProp(obstacleModel, obstaclePosition, Game.Player.Character.Rotation, true, false);
            }
        }
    }
}