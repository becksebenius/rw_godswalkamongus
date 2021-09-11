using Verse;

namespace GodsWalkAmongUs
{
    public static class RebootHelper
    {
        [DebugAction(category = "Reboot", allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void RebootFromMap() => GenCommandLine.Restart();
        
        [DebugAction(category = "Reboot", allowedGameStates = AllowedGameStates.Entry)]
        private static void RebootFromEntry() => GenCommandLine.Restart();
    }
}