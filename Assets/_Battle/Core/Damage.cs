namespace IdleBattle
{
    // Giữ nguyên định nghĩa struct Damage của game gốc.
    public struct Damage
    {
        public double OriginValue;
        public double Value;
        public Enum_DamageType DamageType;
        public Enum_CriticalType CriticalType;
        public Enum_PlayerState PlayerState;
    }
}
