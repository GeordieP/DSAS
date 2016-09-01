using System.Linq;

public struct Bounds {
    public int top, bottom, right, left, width, height;
}

public static class Balance {
    public static Bounds ScreenBounds = new Bounds {
        top = 7,
        bottom = -7,
        right = 4,
        left = -4,
        width = 8,
        height = 14
    };

    // Bounding area that should be used in despawning objects
    public static Bounds DespawnBounds = new Bounds {
        top = ScreenBounds.top + 1,
        bottom = ScreenBounds.bottom - 1,
        right = ScreenBounds.right + 1,
        left = ScreenBounds.left - 1,
        width = ScreenBounds.width + 2,
        height = ScreenBounds.height + 2
    };

    public static Bounds SpawnBounds = new Bounds {
        top = DespawnBounds.top - 2,
        bottom = DespawnBounds.bottom + 2,
        right = DespawnBounds.right - 2,
        left = DespawnBounds.left + 2,
        width = DespawnBounds.width - 4,
        height = DespawnBounds.height - 4
    };

    public static Bounds BossSpawnBounds = new Bounds {
        top = DespawnBounds.top + 4,
        bottom = DespawnBounds.bottom - 4,
        right = DespawnBounds.right + 4,
        left = DespawnBounds.left - 4,
        width = DespawnBounds.width + 8,
        height = DespawnBounds.height + 8
    };

    public const float BULLET_Z_POSITION = 1f;

    /*---
    * Move Speeds
    ---*/
    // entities    
    public const float PLAYER_BASE_MOVE_SPEED = 10f;
    public const float ENEMY_BASE_MOVE_SPEED = 1.4f;
    public const float POWERUP_BASE_MOVE_SPEED = 1.2f;

    // bullets
    public const float BULLET_INITIAL_SPEED = 6f;
    public const float PLAYER_BULLET_INITIAL_SPEED = 15f;
    public const float ENEMY_BULLET_INITIAL_SPEED = -6f;
    public const float BOSS_BULLET_INITIAL_SPEED = -6f;

    /*---
    * Timers / Rates / Duration
    ---*/
    public const float PLAYER_FIRE_RATE = 9f;
    public const float ENEMY_WAVE_SPAWN_RATE = 7f;
    public const float DMG_FLASH_DURATION = 0.03f;
	public static readonly UnityEngine.WaitForSeconds DMG_FLASH_WAITFORSECONDS = new UnityEngine.WaitForSeconds(DMG_FLASH_DURATION);

    /*---
    * Health / Max Levels
    ---*/
    public const float BOSS_BASE_HEALTH = 250f;
    public const float PLAYER_INITIAL_HEALTH = 100f;
    public const float ENEMY_INITIAL_HEALTH = 99f;
    public const float NUKE_MAX_CHARGE_LEVEL = 35;

    // multiplied by each health value to determine max health for that enemy type
    public const float HEALTH_CAP_MULTIPLIER = 2.5f;
    // every time stage advances, increase the health of each enemy spawned
    public const float HEALTH_ADDED_EACH_STAGE = 4f;
    // each enemy type has a different base health
    public static float[] ENEMY_TYPES_BASE_HEALTHS = {70f, 99f, 65f, 68f, 70f, 85f};
    // multiply each base health by the cap multiplier to determine each enemy type's max health
    public static float[] ENEMY_TYPES_MAX_HEALTHS = ENEMY_TYPES_BASE_HEALTHS.Select(health => health * HEALTH_CAP_MULTIPLIER).ToArray();


    /*---
    * Damage
    ---*/    
    public const float BULLET_BASE_DMG = 25f;
    public const float ENEMY_BULLET_BASE_DMG = 5f;
    public const float PLAYER_BULLET_BASE_DMG = 33f;
    public const float BOSS_BULLET_BASE_DMG = 25f;

    // every stage, enemy bullet damage will increase
    // formula for damage scaling is (as of writing this) enemyDmg = baseDmg + DMG_SCALING_PER_STAGE * stage
    public const float ENEMY_DMG_SCALING_PER_STAGE = 1.5f;
    // each enemy type has a different base bullet damage
    public static float[] ENEMY_TYPES_BASE_DMG = {5f, 8f, 8f, 6f, 10f, 8f};
    // no enemy bullet should do more than this amount of damage
    public const float ENEMY_MAX_SHOT_DMG = 50f;

    /*---
    * Point values
    ---*/
    // amount to advance the first stage, and base score limit calculations for each stage off of
    public const int INITIAL_STAGE_ADVANCEMENT_SCORE_CAP = 4000;
    // multiplied by stage and initial score cap to determine score cap for next level
    public const int STAGE_ADVANCEMENT_SCORE_MULTIPLIER = 2;
    // stages will never require more than this amount of score to advance to next stage
    public const int STAGE_ADVANCEMENT_SCORE_CAP = 10000;


    /*---
    * Pool sizes
    ---*/
    public const int POOL_SIZE_ENEMY = 20;
    public const int POOL_SIZE_ENEMY_BULLET = 100;
    public const int POOL_SIZE_PLAYER_BULLET = 50;
    public const int POOL_SIZE_EXPLOSION_FRAGMENT = 180;
    public const int POOL_SIZE_BOSS_BULLET = 150;
    public const int POOL_SIZE_POWERUP = 5;
    
    /*---
    * Other
    ---*/    
    public const float PLAYER_BULLET_KNOCKBACK_DISTANCE = 0.08f;
    public const float ENEMY_BULLET_KNOCKBACK_DISTANCE = 0.05f;
    public const int ENEMY_WAVE_MIN_SIZE = 5;
    public const int ENEMY_WAVE_MAX_SIZE = 15;
    public const float DAMAGED_ENEMY_NEXT_SHOT_DELAY = 1f;
    public const float DAMAGED_PLAYER_NEXT_SHOT_DELAY = 0.5f;
}
