using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Setting
{
    // Settings
    public const int TARGET_FRAME_RATE = 120;

    // Id distribute keys
    public const string ID_ENTITY = "entity";
    public const string ID_ACTION = "action";
    public const string ID_WEIGHTED_PRIORITY_QUEUE = "weighted priority queue";

    // Action Priorities
    public const int ACTION_ATTACK_PRIORITY = 1;
    public const int ACTION_DEFEND_PRIORITY = 2;

    // Action Cooldowns
    public const float CD_CIRCLE_ATTACK = 1f;
    public const float CD_CIRCLE_GUARD = 1f;

    // Action Durations  (Frames)
    public const int DURATION_CIRCLE_ATTACK = 1;
    public const int DURATION_GUARD = 60;

    // Action Animation Durations  (Seconds)
    public const float ADURATION_GUARD = 0.5f;
}
