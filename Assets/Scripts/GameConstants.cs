using UnityEngine;

public static class GameConstants
{
    public const float ScreenWidth = 1920f;
    public const float ScreenHeight = 1080f;
    public const float OrthoHalfHeight = 540f;

    public const float SpawnMinX = -460f;
    public const float SpawnMaxX = 460f;
    public const float SpawnLineY = 390f;
    public const float DefenseLineY = -260f;
    public const float PlayerPosX = 0f;
    public const float PlayerPosY = -340f;

    public const float PlayerMaxHealth = 100.0f;
    public const float PlayerStartHealth = 100.0f;
    public const float PlayerHealthDecayPerSecond = 3.0f;
    public const float PlayerAttackDamage = 12.0f;
    public const float PlayerFireInterval = 0.25f;
    public const float PlayerSwapCooldown = 6.0f;
    public const float LowHealthWarningThreshold = 25.0f;

    public const float DefaultAimRotationLerpSpeed = 60.0f;
    public const float DefaultHitboxMultiplier = 1.3f;

    public const float BulletSpeed = 1680.0f;
    public const float BulletVisualRadius = 3.0f;
    public const float BulletOutBoundsMargin = 100.0f;

    public const float EnemyMaxHealth = 100.0f;
    public const float FastEnemyStartHealth = 30.0f;
    public const float SlowEnemyStartHealth = 90.0f;

    public const float FastEnemyMoveSpeed = 84.0f;
    public const float SlowEnemyMoveSpeed = 42.0f;
    public const float EnemyDashSpeed = 252.0f;

    public const float FastEnemyArrivalDamage = 10.0f;
    public const float SlowEnemyArrivalDamage = 20.0f;

    public const float FastEnemyBodyRadius = 15.0f;
    public const float SlowEnemyBodyRadius = 24.0f;

    public const float FastEnemyHitRadius = 25.0f;
    public const float SlowEnemyHitRadius = 27.0f;

    public const float FastEnemySelectRadius = 15.0f;
    public const float SlowEnemySelectRadius = 24.0f;

    public const float InitialSpawnDelay = 1.0f;
    public const float Phase1EndTime = 20.0f;
    public const float Phase2EndTime = 40.0f;

    public const float Phase1SpawnInterval = 2.4f;
    public const float Phase2SpawnInterval = 1.8f;
    public const float Phase3SpawnInterval = 1.3f;

    public const float HitFlashDuration = 0.18f;
    public const float PlayerDamageEffectDuration = 0.35f;
    public const float SwapEffectDuration = 0.8f;
    public const float SystemNoticeDuration = 1.8f;
}
