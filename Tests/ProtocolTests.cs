using System;

public static class ProtocolTests
{
    public static void Main()
    {
        int count = 0;
        string[] invalid = { "1", "HF1,1,500,500,500,0,1,1", "HF1,9,500,500,500,1,1,1",
            "HF1,1,1024,500,500,1,1,1", "HF1,1,500,500,500,1,1,0", "", "HF1,1,500,500,500,2,1,1",
            "HF1,0,500,500,500,1,1,1", "HF1,1,500,500,500,1,1,1,1", "HF1,1,-1,500,500,1,1,1" };
        foreach (string line in invalid)
        {
            if (InstrumentFrame.TryParse(line, out _)) throw new Exception("Invalid packet accepted: " + line);
            count++;
        }
        for (int note = 0; note <= 8; note++)
        {
            string line = "HF1," + note + ",500,500,500," + (note > 0 ? "1,1,1" : "0,0,1");
            if (!InstrumentFrame.TryParse(line, out InstrumentFrame frame) || frame.Note != note)
                throw new Exception("Valid packet rejected.");
            count++;
        }
        Console.WriteLine("PASS: " + count + " protocol cases.");
    }
}
