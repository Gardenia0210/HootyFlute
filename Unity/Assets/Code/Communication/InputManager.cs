using System;
using System.IO.Ports;
using UnityEngine;
// One serial owner per gameplay scene; all Unity calls stay on the main thread.
public class InputManager : MonoBehaviour
{
    public string portName = "";
    public int baudRate = 115200;
    private SerialPort serialPort;
    private string buffer = "";
    private float nextCommand;
    private float lastFrame;
    private bool sounding;
    private Note target;
    protected virtual void OnEnable()
    {
        string selected = Environment.GetEnvironmentVariable("HOOTY_FLUTE_PORT");
        if (string.IsNullOrWhiteSpace(selected)) selected = portName;
        try
        {
            if (string.IsNullOrWhiteSpace(selected))
            {
                string[] ports = SerialPort.GetPortNames();
                if (ports.Length != 1)
                    throw new InvalidOperationException("Set HOOTY_FLUTE_PORT or the Inspector portName to the instrument port.");
                selected = ports[0];
            }
            serialPort = new SerialPort(selected, baudRate) {
                NewLine = "\n", ReadTimeout = 20, WriteTimeout = 50, DtrEnable = true
            };
            serialPort.Open();
            buffer = "";
            sounding = false;
            nextCommand = 0;
            lastFrame = Time.realtimeSinceStartup;
        }
        catch (Exception error)
        {
            Debug.LogError("Instrument connection failed: " + error.Message, this);
            Close();
        }
    }
    protected virtual void Update()
    {
        if (serialPort == null || !serialPort.IsOpen) return;
        try
        {
            Note current = Time.timeScale > 0 ? Note.FindTarget() : null;
            if (current != target || Time.realtimeSinceStartup >= nextCommand)
            {
                target = current;
                serialPort.WriteLine("E," + (target != null ? target.noteNumber : 0));
                nextCommand = Time.realtimeSinceStartup + 0.1f;
            }
            // Bound per-frame work and reject oversized or fragmented invalid packets.
            int count = Math.Min(serialPort.BytesToRead, 2048);
            if (count > 0)
            {
                byte[] bytes = new byte[count];
                int read = serialPort.Read(bytes, 0, count);
                buffer += System.Text.Encoding.ASCII.GetString(bytes, 0, read);
            }
            int newline;
            while ((newline = buffer.IndexOf('\n')) >= 0)
            {
                string line = buffer.Substring(0, newline).Trim();
                buffer = buffer.Substring(newline + 1);
                if (!InstrumentFrame.TryParse(line, out InstrumentFrame frame)) continue;
                lastFrame = Time.realtimeSinceStartup;
                if (frame.Sounding && !sounding && target != null && frame.Note == target.noteNumber)
                    target.TryHit(frame.Note);
                sounding = frame.Sounding;
            }
            if (buffer.Length > 256) throw new InvalidOperationException("Invalid instrument packet size.");
            if (Time.realtimeSinceStartup - lastFrame > 3f)
                throw new TimeoutException("Instrument telemetry stopped.");
        }
        catch (Exception error)
        {
            Debug.LogError("Instrument disconnected: " + error.Message, this);
            Close();
        }
    }
    private void Close()
    {
        if (serialPort == null) return;
        try { if (serialPort.IsOpen) serialPort.WriteLine("E,0"); }
        catch (Exception) { /* The firmware watchdog also stops disconnected outputs. */ }
        serialPort.Dispose();
        serialPort = null;
        buffer = "";
        target = null;
        sounding = false;
    }
    protected virtual void OnDisable() { Close(); }
    protected virtual void OnApplicationQuit() { Close(); }
}
