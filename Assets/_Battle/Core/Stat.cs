namespace IdleBattle
{
    // Bảng chỉ số: index theo Enum_StatType, giá trị double, mặc định 0.
    // Bản port bỏ ObscuredDouble (AntiCheat) của game gốc.
    public sealed class Stat
    {
        private readonly double[] _values = new double[(int)Enum_StatType.Count];

        public double this[Enum_StatType type]
        {
            get { return _values[(int)type]; }
            set { _values[(int)type] = value; }
        }

        public double this[int index]
        {
            get { return _values[index]; }
            set { _values[index] = value; }
        }

        public void Clear()
        {
            for (int i = 0; i < _values.Length; i++)
            {
                _values[i] = 0;
            }
        }

        public Stat Copy()
        {
            Stat s = new Stat();
            System.Array.Copy(_values, s._values, _values.Length);
            return s;
        }
    }
}
