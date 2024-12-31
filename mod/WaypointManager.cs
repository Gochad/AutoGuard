using GTA.Native;

namespace drivingMod
{
    public static class WaypointManager
    {
        public static void SetWaypoint(float x, float y)
        {
            Function.Call(Hash.SET_NEW_WAYPOINT, x, y);
        }
    }
}
