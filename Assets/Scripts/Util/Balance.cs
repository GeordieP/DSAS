public struct Bounds {
    public int top, bottom, right, left, width, height;
}

public static class Balance {
    // Bounding area that should be used in despawning objects
    public static Bounds ScreenBounds = new Bounds {
        top = 8,
        bottom = -8,
        right = 5,
        left = -5,
        width = 10,
        height = 16
    };

    public static Bounds SpawnBounds = new Bounds {
        top = ScreenBounds.top - 2,
        bottom = ScreenBounds.bottom + 2,
        right = ScreenBounds.right - 2,
        left = ScreenBounds.left + 2,
        width = ScreenBounds.width - 4,
        height = ScreenBounds.height - 4
    };

    public const float PLAYER_BASE_MOVE_SPEED = 10f;

    public const float BULLET_INITIAL_SPEED = 10f;
    public const float PLAYER_BULLET_INITIAL_SPEED = 40f;
    public const float ENEMY_BULLET_INITIAL_SPEED = -10f;

    public const float PLAYER_FIRE_RATE = 5f;

    public const float PLAYER_INITIAL_HEALTH = 100f;
    public const float ENEMY_INITIAL_HEALTH = 100f;

    public const float BULLET_BASE_DMG = 25f;
    public const float ENEMY_BULLET_BASE_DMG = 25f;
    public const float PLAYER_BULLET_BASE_DMG = 25f;

    public const float PLAYER_BULLET_KNOCKBACK_DISTANCE = 0.05f;

    public const float DMG_FLASH_DURATION = 0.03f;

    public const int ENEMY_BASE_SCORE_VALUE = 15;

}
