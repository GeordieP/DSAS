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

    public const float BULLET_INITIAL_SPEED = 30f;
    public const float PLAYER_BULLET_INITIAL_SPEED = 40f;
    public const float ENEMY_BULLET_INITIAL_SPEED = -30f;

    public const float PLAYER_FIRE_RATE = 5f;

    public const float PLAYER_INITIAL_HEALTH = 100f;
    public const float ENEMY_INITIAL_HEALTH = 100f;

    public const float BULLET_BASE_DMG = 25f;
    public const float ENEMY_BULLET_BASE_DMG = 25f;
    public const float PLAYER_BULLET_BASE_DMG = 25f;

    public const float PLAYER_BULLET_KNOCKBACK_DISTANCE = 0.05f;

    public const float DMG_FLASH_DURATION = 0.03f;

}
