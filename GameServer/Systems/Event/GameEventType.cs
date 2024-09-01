namespace GameServer.Systems.Event;

internal enum GameEventType
{
    Login = 1,
    EnterGame = 2,
    PushDataDone = 3,

    // Actions
    PlayerPositionChanged = 4,
    FormationUpdated = 5,
    VisionSkillChanged = 6,

    // Debug
    DebugUnlockAllRoles = 7,
    DebugUnlockAllItems = 8
}