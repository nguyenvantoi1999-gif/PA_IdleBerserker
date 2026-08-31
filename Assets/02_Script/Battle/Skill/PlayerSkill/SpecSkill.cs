// PA-native: mirror SpecCompanionSkill style. No ObscuredTypes/DataFieldBase.
public class SpecSkill
{
    public int fieldID;
    public Enum_ItemGrade grade;
    public float value = 100f;
    public float increaseValue = 0f;
    public float subValue = 30f;
    public float increaseSubValue = 0f;
    public float time = 3f;
    public int targetCount = 5;
    public float cooldown = 6f;
    public bool isPassive = false;
    public bool ignoreGacha = false;

    public int GetBuffID() { return (int)Enum_BuffFrom.Skill * 1000 + fieldID; }
}
