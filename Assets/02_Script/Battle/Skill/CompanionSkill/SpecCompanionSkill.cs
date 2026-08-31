using System.Collections.Generic;

// PA-native: bỏ ObscuredTypes/Facebook/DataFieldBase của game gốc, giữ field + API.
public class SpecCompanionSkill
{
    public int fieldID;
    public Enum_ItemGrade grade;
    public string activeSkillID = "";
    public float skillCooldown = 8f;
    public float skillDuration = 3f;

    public int targetCount_1 = 5;
    public float effectValue_1 = 0.5f;
    public float effectValueIncrease_1 = 0.02f;
    public int tickTime_1 = 1;
    public int tickCount_1 = 3;

    public float effectValue_2 = 0.3f;
    public float effectValueIncrease_2 = 0.01f;

    public string LocalizeID = "";

    public int GetBuffID() { return (int)Enum_BuffFrom.Companion * 1000 + fieldID; }
    public float GetMainValue(int level) { return effectValue_1 + (effectValueIncrease_1 * (level - 1)); }
    public float GetSubValue(int level) { return effectValue_2 + (effectValueIncrease_2 * (level - 1)); }
}

// CE sub-stat (option cấp 5): kéo dài thời gian skill.
public class SpecCE_SubStat
{
    public int companionIndex;
    public float duration;
    public float value;
    public float subValue;
}

// Nguồn spec: PA dùng bảng mặc định (game gốc nạp từ server/JSON).
public class SpecDataManager
{
    private static SpecDataManager _instance;
    public static SpecDataManager Instance { get { if (_instance == null) { _instance = new SpecDataManager(); } return _instance; } }

    private readonly Dictionary<int, SpecCompanionSkill> _companionSkills = new Dictionary<int, SpecCompanionSkill>();
    private readonly Dictionary<int, SpecSkill> _playerSkills = new Dictionary<int, SpecSkill>();
    private List<SpecCE_SubStat> _ceSubStats;

    public SpecCompanionSkill GetSpecCompanionSkill(int fieldID)
    {
        SpecCompanionSkill spec;
        if (!_companionSkills.TryGetValue(fieldID, out spec))
        {
            spec = new SpecCompanionSkill { fieldID = fieldID };
            _companionSkills[fieldID] = spec;
        }
        return spec;
    }

    public SpecSkill GetSpecSkill(int fieldID)
    {
        SpecSkill spec;
        if (!_playerSkills.TryGetValue(fieldID, out spec))
        {
            spec = new SpecSkill
            {
                fieldID = fieldID,
                grade = Enum_ItemGrade.Normal,
                value = 100f,
                increaseValue = 0f,
                subValue = 30f,
                increaseSubValue = 0f,
                time = 3f,
                targetCount = 5,
                cooldown = 6f,
                isPassive = false,
                ignoreGacha = false
            };
            _playerSkills[fieldID] = spec;
        }
        return spec;
    }

    public List<SpecCE_SubStat> GetSpecCE_SubStat()
    {
        if (_ceSubStats == null)
        {
            _ceSubStats = new List<SpecCE_SubStat>();
            for (int i = 0; i <= 40; i++) { _ceSubStats.Add(new SpecCE_SubStat { companionIndex = i, duration = 2f, value = 0.5f, subValue = 0.3f }); }
        }
        return _ceSubStats;
    }
}
