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

    // 1. 페이즈 시스템 전환 기준 (생존 시간 0~30초, 30~60초, 60초 이후)
    public const float InitialSpawnDelay = 1.0f;
    public const float Phase1EndTime = 30.0f;
    public const float Phase2EndTime = 60.0f;

    // 스폰 주기(간격)
    public const float Phase1SpawnInterval = 2.0f;
    public const float Phase2SpawnInterval = 1.2f;
    public const float Phase3SpawnInterval = 0.6f;

    // 1회 스폰 수
    public const int Phase1SpawnBatchCount = 1;
    public const int Phase2SpawnBatchCount = 2;
    public const int Phase3SpawnBatchCount = 3;

    // 화면 내 최대 몬스터 수
    public const int Phase1MaxMonsterCount = 3;
    public const int Phase2MaxMonsterCount = 6;
    public const int Phase3MaxMonsterCount = 10;

    // 3. 능력 교환 AI 시스템 수치
    public const int AbilitySlotCount = 3;
    public const float AbilitySwapCycleInterval = 30.0f;
    public const float AbilitySwapResumeCountdown = 3.0f;

    // 2. 교환 효과 세부 수치
    public const float RapidFirePlayerSpeedBonus = 0.10f;
    public const float RapidFireMonsterSpawnReduction = 0.10f;

    public const float FirepowerPlayerDamageBonus = 0.20f;
    public const float FirepowerMonsterDamageBonus = 0.20f;

    public const float HighCaliberPlayerBulletScale = 1.5f;
    public const float HighCaliberMonsterHitboxScale = 1.5f;

    public const float ForcedMarchPlayerAimMultiplier = 2.0f;
    public const float ForcedMarchMonsterSpeedBonus = 0.10f;

    public const float ArmorPlayerDecayMultiplier = 0.80f;
    public const float ArmorMonsterHealthMultiplier = 1.20f;

    public const float BreakthroughPlayerKillHeal = 2.0f;
    public const float BreakthroughMonsterDecayPenaltyMultiplier = 2.0f;

    public const float HitFlashDuration = 0.18f;
    public const float PlayerDamageEffectDuration = 0.35f;
    public const float SwapEffectDuration = 0.8f;
    public const float SystemNoticeDuration = 1.8f;
}
