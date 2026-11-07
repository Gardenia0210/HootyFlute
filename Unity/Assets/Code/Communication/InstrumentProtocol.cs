using System;
// 115200 baud, newline-delimited ASCII:
// Unity -> Arduino: E,<expected note 0..8> every 100 ms (0 stops training).
// Arduino -> Unity: HF1,<note>,<ADC A>,<ADC B>,<ADC C>,<contact>,<sounding>,<ready>.
// Plain note numbers from older demo firmware are rejected.
public struct InstrumentFrame
{
    public int Note;
    public bool Contact;
    public bool Sounding;
    public static bool TryParse(string line, out InstrumentFrame frame)
    {
        frame = default;
        string[] fields = line.Split(',');
        if (fields.Length != 8 || fields[0] != "HF1") return false;
        int[] values = new int[7];
        for (int i = 0; i < values.Length; i++)
            if (!int.TryParse(fields[i + 1], out values[i])) return false;
        if (values[0] < 0 || values[0] > 8) return false;
        for (int i = 1; i <= 3; i++)
            if (values[i] < 0 || values[i] > 1023) return false;
        if (values[4] < 0 || values[4] > 1 || values[5] < 0 || values[5] > 1 || values[6] != 1)
            return false;
        if (values[5] == 1 && (values[4] == 0 || values[0] == 0)) return false;
        frame.Note = values[0];
        frame.Contact = values[4] == 1;
        frame.Sounding = values[5] == 1;
        return true;
    }
}
