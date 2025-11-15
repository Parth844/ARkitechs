// 15-11-2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using System.IO;

public class WAV
{
    public byte[] AudioData { get; private set; }
    public int SampleCount { get; private set; }
    public int Frequency { get; private set; }

    public WAV(byte[] wav)
    {
        using (var stream = new MemoryStream(wav))
        using (var reader = new BinaryReader(stream))
        {
            // Skip the header
            reader.ReadBytes(44);

            // Read the data
            AudioData = reader.ReadBytes((int)(stream.Length - stream.Position));

            // Set sample count and frequency (hardcoded for simplicity)
            SampleCount = AudioData.Length / 2; // Assuming 16-bit audio
            Frequency = 44100; // Default frequency
        }
    }
}