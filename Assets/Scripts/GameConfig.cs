using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;

public class GameConfig {
    public float MoveTime { get; private set; }
    public int UserObjectKinds { get; private set; }
    public int TotalObjectKinds { get; private set; }
    public float ObjectTimeDelta { get; private set; }
    
    private static readonly float BaseMoveTime = 6.0f;
    private static readonly float MinMoveTime = 2.0f;
    private static readonly int BaseUserObjectKinds = 4;
    private static readonly int BaseTotalObjectKinds = 8;
    private static readonly float BaseObjectTimeDelta = 0.8f;

    public static readonly int BaseLifeAmount = 3;
    public static readonly float ShowcaseTime = 7.0f;
    public static readonly float LevelTime = 30.0f;

    private GameConfig() {}
    
    public static GameConfig GetForLevel(int level) {
        var cfg = new GameConfig();
        cfg.MoveTime = BaseMoveTime - 0.3f*level;
        if (cfg.MoveTime < MinMoveTime)
            cfg.MoveTime = MinMoveTime;
        cfg.UserObjectKinds = BaseUserObjectKinds + level / 3;
        cfg.TotalObjectKinds = BaseTotalObjectKinds + level / 2;
        cfg.ObjectTimeDelta = BaseObjectTimeDelta - level *0.05f;
        if (cfg.ObjectTimeDelta < 0.05f)
            cfg.ObjectTimeDelta = 0.05f;
        return cfg;
    }
}
