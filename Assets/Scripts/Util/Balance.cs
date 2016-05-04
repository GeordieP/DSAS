﻿public struct Bounds {
    public int top, bottom, right, left;
}

public static class Balance {
    // Bounding area that should be used in despawning objects
    public static Bounds ScreenBounds = new Bounds {
        top = 10,
        bottom = -10,
        right = 3,
        left = -3
    };

    public const float BULLET_INITIAL_SPEED = 30f;
    public const float PLAYER_BULLET_INITIAL_SPEED = 40f;
    public const float ENEMY_BULLET_INITIAL_SPEED = -30f;

    public const float PLAYER_INITIAL_HEALTH = 100f;
    public const float ENEMY_INITIAL_HEALTH = 100f;

    public const float BULLET_BASE_DMG = 25f;
    public const float ENEMY_BULLET_BASE_DMG = 25f;
    public const float PLAYER_BULLET_BASE_DMG = 50f;
}
