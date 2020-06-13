using System.Collections.Generic;

public static class GameCore {

    public static float MoveTime { get; private set; }
    public static int UserObjectKinds { get; private set; }
    public static int TotalObjectKinds { get; private set; }
    public static int MaxObjectOnScreen { get; private set; }

    private static readonly float BaseMoveTime = 6.0f;
    private static readonly int BaseUserObjectKinds = 3;
    private static readonly int BaseTotalObjectKinds = 2;
    private static readonly int BaseMaxObjectOnScreen = 6;

    public static int CurrentLevel { get; private set; } = 0;

    public static void ResetLevel() {
        CurrentLevel = 1;
        MoveTime = BaseMoveTime;
        UserObjectKinds = BaseUserObjectKinds;
        TotalObjectKinds = BaseTotalObjectKinds;
        MaxObjectOnScreen = BaseMaxObjectOnScreen;
    }

    public static void LevelUp() {
        CurrentLevel++;
        MoveTime -= 0.3f;
        UserObjectKinds = BaseUserObjectKinds + CurrentLevel / 3;
        TotalObjectKinds = BaseTotalObjectKinds + CurrentLevel / 2;
        MaxObjectOnScreen = BaseMaxObjectOnScreen + CurrentLevel / 2;
    }
}
