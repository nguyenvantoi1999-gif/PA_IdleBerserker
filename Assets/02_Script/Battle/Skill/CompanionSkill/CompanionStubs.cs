using System.Collections.Generic;
using UnityEngine;

// ===== Stub các hệ chưa port sang PA (data player, UI, cheat, localize...) =====

public static class PCDebug
{
    public static void Log(object m) { Debug.Log(m); }
    public static void LogError(object m) { Debug.LogError(m); }
}

public static class Localize
{
    public static string GetString(string key) { return key; }
}

// Dữ liệu 1 companion (game gốc: level, sao...). PA: mặc định level 1.
public class CompanionInfo
{
    public int Level = 1;
    public float GetStarFactor() { return 1f; }
}

public class CEData { public int level; }

public class CompanionDataStore
{
    private readonly CompanionInfo _default = new CompanionInfo();
    public CompanionInfo GetCompanion(int fieldID) { return _default; }
    public Dictionary<Enum_CompanionEquipmentType, CEData> GetCECache(int fieldID)
    {
        return new Dictionary<Enum_CompanionEquipmentType, CEData>();
    }
}

public class EWPresetGrowth
{
    public int[] CompanionLevels = new int[128];
    public int[] Levels = new int[128];
    public EWPresetGrowth() { for (int i = 0; i < CompanionLevels.Length; i++) { CompanionLevels[i] = 1; Levels[i] = 1; } }
}

public static class PlayerDataManager
{
    public static CompanionDataStore CompanionData = new CompanionDataStore();
    public static EWPresetGrowth EWPresetGrowthData = new EWPresetGrowth();
    public static SkillDataStore SkillData = new SkillDataStore();
}

// UI hiệu ứng khi player dùng companion skill (stub).
public class FX_UI_Companion
{
    private static FX_UI_Companion _instance;
    public static FX_UI_Companion Instance { get { if (_instance == null) { _instance = new FX_UI_Companion(); } return _instance; } }
    public void SetActiveObj() { }
    public void PlayAnim(int fieldID) { }
}

// Cheat/tool: bỏ cooldown (chỉ dùng trong editor gốc). PA: luôn false.
public class PlayerSkillManager
{
    private static PlayerSkillManager _instance;
    public static PlayerSkillManager Instance { get { if (_instance == null) { _instance = new PlayerSkillManager(); } return _instance; } }
    public bool ZeroCoolTime = false;
    public System.Collections.Generic.List<int> GetEquippedCompanionSkill() { return new System.Collections.Generic.List<int>(); }
    public CompanionActiveSkill GetCompanionActiveSkill(int index) { return null; }
}

// Stub quản lý skill của enemy (PVP) — PA không dùng.
public class EnemySkillManager
{
    private static EnemySkillManager _instance;
    public static EnemySkillManager Instance { get { if (_instance == null) { _instance = new EnemySkillManager(); } return _instance; } }
    public System.Collections.Generic.List<int> GetEquippedCompanionSkill() { return new System.Collections.Generic.List<int>(); }
    public CompanionActiveSkill GetCompanionActiveSkill(int index) { return null; }
}

// Tiện ích xác suất (game gốc). PA: dùng Random.
public static class UtilCode
{
    public static bool GetChance(float percent) { return UnityEngine.Random.value * 100f < percent; }
}

// Trigger hệ effect passive theo sự kiện (stub, chưa port).
public static class BerserkerEffectManager
{
    public static void Trigger(object owner, object trigger, int fieldID, object targets = null) { }
    public static void Trigger(object owner, Enum_EffectTrigger trigger) { }
}
