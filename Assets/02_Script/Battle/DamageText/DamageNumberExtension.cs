
// Bản rút gọn của DoubleExtension.ToUnitString game gốc cho playable ad:
// < 1000 hiển thị số nguyên; lớn hơn thì rút gọn K/M/B/T/aa...
public static class DamageNumberExtension
{
    private static readonly string[] Units = { "", "K", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af" };

    public static string ToUnitString(this double value)
    {
        if (value < 0) value = 0;
        if (value < 1000) return ((long)value).ToString();

        int u = 0; double v = value;
        while (v >= 1000 && u < Units.Length - 1) { v /= 1000.0; u++; }
        string fmt = v >= 100 ? "0" : (v >= 10 ? "0.#" : "0.##");
        return v.ToString(fmt) + Units[u];
    }

    public static string ToUnitString(this float value) { return ToUnitString((double)value); }
    public static string ToUnitString(this int value) { return ToUnitString((double)value); }
}
