using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    RapidFire,
    FirepowerAmp,
    HighCaliber,
    ForcedMarch,
    ArmorReinforce,
    BreakthroughImpact
}

public class AbilitySwapSystem : MonoBehaviour
{
    [Header("Initial Slot Presets")]
    [SerializeField] private AbilityType[] initialPlayerSlots = new AbilityType[]
    {
        AbilityType.RapidFire,
        AbilityType.HighCaliber,
        AbilityType.ArmorReinforce
    };

    [SerializeField] private AbilityType[] initialMonsterSlots = new AbilityType[]
    {
        AbilityType.FirepowerAmp,
        AbilityType.ForcedMarch,
        AbilityType.BreakthroughImpact
    };

    [Header("Runtime Slot State")]
    [SerializeField] private List<AbilityType> playerSlots = new List<AbilityType>();
    [SerializeField] private List<AbilityType> monsterSlots = new List<AbilityType>();
    [SerializeField] private int selectedMonsterSlotIndex = -1;
    [SerializeField] private int aiSelectedPlayerSlotIndex = -1;

    public IReadOnlyList<AbilityType> PlayerSlots => playerSlots;
    public IReadOnlyList<AbilityType> MonsterSlots => monsterSlots;
    public int SelectedMonsterSlotIndex => selectedMonsterSlotIndex;
    public int AiSelectedPlayerSlotIndex => aiSelectedPlayerSlotIndex;
    public bool CanConfirmSwap => selectedMonsterSlotIndex >= 0 && selectedMonsterSlotIndex < monsterSlots.Count
                                  && aiSelectedPlayerSlotIndex >= 0 && aiSelectedPlayerSlotIndex < playerSlots.Count;

    private void Awake()
    {
        if (playerSlots.Count == 0 && monsterSlots.Count == 0)
        {
            ResetAbilities();
        }
    }

    public void ResetAbilities()
    {
        playerSlots.Clear();
        monsterSlots.Clear();

        for (int i = 0; i < initialPlayerSlots.Length; i++)
        {
            playerSlots.Add(initialPlayerSlots[i]);
        }

        for (int i = 0; i < initialMonsterSlots.Length; i++)
        {
            monsterSlots.Add(initialMonsterSlots[i]);
        }

        ClearSelection();
    }

    public void BeginSwapDraft()
    {
        ClearSelection();
    }

    public void ClearSelection()
    {
        selectedMonsterSlotIndex = -1;
        aiSelectedPlayerSlotIndex = -1;
    }

    // 1. 플레이어가 상대 슬롯(3개) 중 가져올 효과 1개를 먼저 클릭하면,
    // 2. AI가 내 슬롯(3개) 중 가져올 효과 1개를 동일 확률(각 33.3%)로 선택한다.
    public bool SelectMonsterSlotForSwap(int monsterSlotIndex)
    {
        if (monsterSlotIndex < 0 || monsterSlotIndex >= monsterSlots.Count || playerSlots.Count == 0)
        {
            return false;
        }

        selectedMonsterSlotIndex = monsterSlotIndex;
        aiSelectedPlayerSlotIndex = Random.Range(0, playerSlots.Count);
        return true;
    }

    // [교환 확정] 버튼 클릭 시 두 슬롯의 데이터가 즉시 1:1 맞교환됨
    public bool ConfirmSwap(out AbilityType acquiredByPlayer, out AbilityType takenByAi)
    {
        acquiredByPlayer = AbilityType.RapidFire;
        takenByAi = AbilityType.RapidFire;

        if (!CanConfirmSwap)
        {
            return false;
        }

        acquiredByPlayer = monsterSlots[selectedMonsterSlotIndex];
        takenByAi = playerSlots[aiSelectedPlayerSlotIndex];

        monsterSlots[selectedMonsterSlotIndex] = takenByAi;
        playerSlots[aiSelectedPlayerSlotIndex] = acquiredByPlayer;

        ClearSelection();
        return true;
    }

    public bool PlayerHasAbility(AbilityType abilityType)
    {
        return playerSlots.Contains(abilityType);
    }

    public bool MonsterHasAbility(AbilityType abilityType)
    {
        return monsterSlots.Contains(abilityType);
    }

    public AbilityType GetPlayerSlotAbility(int index)
    {
        if (index < 0 || index >= playerSlots.Count)
        {
            return AbilityType.RapidFire;
        }
        return playerSlots[index];
    }

    public AbilityType GetMonsterSlotAbility(int index)
    {
        if (index < 0 || index >= monsterSlots.Count)
        {
            return AbilityType.RapidFire;
        }
        return monsterSlots[index];
    }

    // --- 각 진영 보유 능력에 따른 실시간 수치 보정 프로퍼티 ---

    // 연사 가속 (플레이어): 발사 간격 단축 (연사 속도 +10%)
    public float GetPlayerFireInterval()
    {
        if (PlayerHasAbility(AbilityType.RapidFire))
        {
            return GameConstants.PlayerFireInterval / (1f + GameConstants.RapidFirePlayerSpeedBonus);
        }
        return GameConstants.PlayerFireInterval;
    }

    // 연사 가속 (몬스터): 스폰 쿨타임 단축 (스폰 간격 10% 감소)
    public float GetMonsterSpawnIntervalMultiplier()
    {
        return MonsterHasAbility(AbilityType.RapidFire)
            ? (1f - GameConstants.RapidFireMonsterSpawnReduction)
            : 1f;
    }

    // 화력 증폭 (플레이어): 탄환 공격력 +20%
    public float GetPlayerBulletDamage()
    {
        return PlayerHasAbility(AbilityType.FirepowerAmp)
            ? GameConstants.PlayerAttackDamage * (1f + GameConstants.FirepowerPlayerDamageBonus)
            : GameConstants.PlayerAttackDamage;
    }

    // 화력 증폭 (몬스터): 방어선 접촉 시 주는 피해 +20%
    public float GetMonsterArrivalDamageMultiplier()
    {
        return MonsterHasAbility(AbilityType.FirepowerAmp)
            ? (1f + GameConstants.FirepowerMonsterDamageBonus)
            : 1f;
    }

    // 대구경 탄환 (플레이어): 탄환 크기/판정 반경 1.5배 증가
    public float GetPlayerBulletScaleMultiplier()
    {
        return PlayerHasAbility(AbilityType.HighCaliber)
            ? GameConstants.HighCaliberPlayerBulletScale
            : 1f;
    }

    public float GetPlayerBulletBonusHitRadius()
    {
        if (!PlayerHasAbility(AbilityType.HighCaliber))
        {
            return 0f;
        }
        return GameConstants.BulletVisualRadius * (GameConstants.HighCaliberPlayerBulletScale - 1f);
    }

    // 대구경 탄환 (몬스터): 몬스터 피격 판정 1.5배 확대 (맞히기 쉬워짐)
    public float GetMonsterHitboxScaleMultiplier()
    {
        return MonsterHasAbility(AbilityType.HighCaliber)
            ? GameConstants.HighCaliberMonsterHitboxScale
            : 1f;
    }

    // 강행군 (플레이어): 조준선 회전 속도 2배 상향
    public float GetPlayerAimSpeedMultiplier()
    {
        return PlayerHasAbility(AbilityType.ForcedMarch)
            ? GameConstants.ForcedMarchPlayerAimMultiplier
            : 1f;
    }

    // 강행군 (몬스터): 몬스터 이동 속도 +10%
    public float GetMonsterMoveSpeedMultiplier()
    {
        return MonsterHasAbility(AbilityType.ForcedMarch)
            ? (1f + GameConstants.ForcedMarchMonsterSpeedBonus)
            : 1f;
    }

    // 장갑 강화 (플레이어): 초당 체력 자연 감소율 -20% 완화
    public float GetBasePlayerHealthDecayRate()
    {
        return PlayerHasAbility(AbilityType.ArmorReinforce)
            ? GameConstants.PlayerHealthDecayPerSecond * GameConstants.ArmorPlayerDecayMultiplier
            : GameConstants.PlayerHealthDecayPerSecond;
    }

    // 장갑 강화 (몬스터): 몬스터 최대/시작 체력 +20%
    public float GetMonsterHealthMultiplier()
    {
        return MonsterHasAbility(AbilityType.ArmorReinforce)
            ? GameConstants.ArmorMonsterHealthMultiplier
            : 1f;
    }

    // 돌파 충격 (플레이어): 몬스터 처치 시 체력 즉시 회복 (+2)
    public float GetPlayerKillHealAmount()
    {
        return PlayerHasAbility(AbilityType.BreakthroughImpact)
            ? GameConstants.BreakthroughPlayerKillHeal
            : 0f;
    }

    // 돌파 충격 (몬스터): 방어선 도달 시 플레이어 체력 지속 감소치 2배
    public bool DoesMonsterArrivalDoublePlayerDecay()
    {
        return MonsterHasAbility(AbilityType.BreakthroughImpact);
    }

    // --- UI 표기용 헬퍼 함수 ---

    public static string GetAbilityName(AbilityType type)
    {
        switch (type)
        {
            case AbilityType.RapidFire:
                return "연사 가속";
            case AbilityType.FirepowerAmp:
                return "화력 증폭";
            case AbilityType.HighCaliber:
                return "대구경 탄환";
            case AbilityType.ForcedMarch:
                return "강행군";
            case AbilityType.ArmorReinforce:
                return "장갑 강화";
            case AbilityType.BreakthroughImpact:
                return "돌파 충격";
            default:
                return "알 수 없는 능력";
        }
    }

    public static string GetPlayerEffectDescription(AbilityType type)
    {
        switch (type)
        {
            case AbilityType.RapidFire:
                return "발사 간격 단축 (연사 속도 +10%)";
            case AbilityType.FirepowerAmp:
                return "탄환 공격력 +20% (12.0 → 14.4)";
            case AbilityType.HighCaliber:
                return "탄환 크기 및 판정 반경 1.5배 증가";
            case AbilityType.ForcedMarch:
                return "조준선 회전 속도 2배 상향 (인풋랙 해소)";
            case AbilityType.ArmorReinforce:
                return "초당 체력 자연 감소율 -20% 완화";
            case AbilityType.BreakthroughImpact:
                return "몬스터 처치 시 체력 즉시 회복 (+2.0)";
            default:
                return "";
        }
    }

    public static string GetMonsterEffectDescription(AbilityType type)
    {
        switch (type)
        {
            case AbilityType.RapidFire:
                return "스폰 쿨타임 단축 (스폰 간격 10% 감소)";
            case AbilityType.FirepowerAmp:
                return "방어선 접촉 시 주는 피해 +20%";
            case AbilityType.HighCaliber:
                return "몬스터 피격 판정 1.5배 확대 (맞히기 쉬워짐)";
            case AbilityType.ForcedMarch:
                return "몬스터 이동 속도 +10% 증가";
            case AbilityType.ArmorReinforce:
                return "몬스터 최대 체력 +20% 증가";
            case AbilityType.BreakthroughImpact:
                return "방어선 도달 시 플레이어 체력 지속 감소치 2배";
            default:
                return "";
        }
    }
}
