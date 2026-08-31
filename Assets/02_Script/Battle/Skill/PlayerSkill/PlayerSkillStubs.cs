using System.Collections.Generic;
using IdleBattle;

// ===== Stub cho PlayerSkill (data player, pet sub-value...) — trả về default an toàn. =====

// Pet sub-list value: kéo dài / giảm hồi chiêu (game gốc). PA: 0.
public class PetSubValue
{
    public float ExtraDuration = 0f;
    public float ExtraCoolDown = 0f;
}

// Mảng có indexer trả về default (level 1) cho mọi fieldID.
public class SkillLevelTable
{
    private readonly Dictionary<int, int> _levels = new Dictionary<int, int>();
    public int this[int fieldID]
    {
        get { int v; return _levels.TryGetValue(fieldID, out v) ? v : 1; }
        set { _levels[fieldID] = value; }
    }
}

// Đếm số lần dùng skill (incrementable).
public class SkillCountTable
{
    private readonly Dictionary<int, int> _counts = new Dictionary<int, int>();
    public int this[int fieldID]
    {
        get { int v; return _counts.TryGetValue(fieldID, out v) ? v : 0; }
        set { _counts[fieldID] = value; }
    }
}

public class PetSubValueTable
{
    private readonly Dictionary<int, PetSubValue> _values = new Dictionary<int, PetSubValue>();
    public PetSubValue this[int fieldID]
    {
        get
        {
            PetSubValue v;
            if (!_values.TryGetValue(fieldID, out v)) { v = new PetSubValue(); _values[fieldID] = v; }
            return v;
        }
    }
}

// Dữ liệu skill của player (stub).
public class SkillDataStore
{
    public SkillLevelTable Levels = new SkillLevelTable();
    public PetSubValueTable petSubListValueList = new PetSubValueTable();
    public SkillCountTable SkillActiveCount = new SkillCountTable();

    public int GetSkillMaxLevel() { return 10; }
    public List<int> GetEquippedIndex(bool passive) { return new List<int>(); }
}

// UseHealth: game gốc trừ % máu hiện tại khi dùng skill. PA: no-op an toàn.
public static class BerserkerObjectSkillExtensions
{
    public static void UseHealth(this BerserkerObject owner, float percent) { }
    public static void UseHealth(this BerserkerObject owner, double percent) { }
}

// Trigger hệ effect passive (game gốc). PA: stub no-op, giữ đủ member để skill compile.
public enum Enum_EffectTrigger
{
    Equip, UseSkill,
    UseFireStone, UseWaterStone, UseGrassStone,
    UseNormalAttack,
    BattleEnter, StageBossEnter, GetEquipment,
    DamageFire, DamageWater, DamageGrass,
    MonsterKill, DestroyObelisk, UseSuperCriticalChance, DieCharacter,
    UseSkill28Effect, GetSoul, Equip_HealthUP,
    GetEnchantStone, UseShield, HealthRecover,
    None,
}
