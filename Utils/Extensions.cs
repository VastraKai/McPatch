using System.Globalization;
using System.Text;

namespace McPatch;
public static class Extensions
{
    // Default method for converting a byte array to a hex string
    public static string ToHexString(this byte[] bytes)
    {
        if (bytes == null) return string.Empty;
        return BitConverter.ToString(bytes).Replace("-", " ");
    }
    // Default method for converting a hex string to a byte array
    public static byte[] FromHexString(this string str)
    {
        byte[] bytes = new byte[str.Length / 2];
        for (int i = 0; i < str.Length; i += 2)
        {
            bytes[i / 2] = byte.Parse(str.Substring(i, 2), NumberStyles.HexNumber);
        }
        return bytes;
    }
    // Optimized method for converting a byte array to a hex string
    public static string ToHexStringO(this byte[] bytes)
    {
        if (bytes == null) return string.Empty;
        StringBuilder sb = new(bytes.Length * 2);
        int i = 0;
        int length = bytes.Length;
        foreach (byte b in bytes)
        {
            sb.AppendFormat("{0:x2}", b);
            // {Console.Value($"{(float)i / (float)length * 100}%")}
            if (i.MeetsChunkSize()) Console.Write($"\r{Console.Prefix("FileUtils")} Converting to hex string: {Console.Value($"{i / (float)length * 100}%")} complete           ");
            i++;
        }
        Console.Write("\r                                                                                \r");
        return sb.ToString();
    }
    // Optimized method for converting a hex string to a byte array
    public static byte[] FromHexStringO(this string str)
    {
        byte[] bytes = new byte[str.Length / 2];
        int length = bytes.Length;
        for (int i = 0; i < str.Length; i += 2)
        {
            int iDiv = i / 2;
            bytes[iDiv] = Convert.ToByte(str.Substring(i, 2), 16);
            if (iDiv.MeetsChunkSize()) Console.Write($"\r{Console.Prefix("FileUtils")} Converting to byte array: {Console.Value($"{iDiv / (float)length * 100}%")} complete           ");
        }
        Console.Write("\r                                                                                \r");
        return bytes;
    }

    public static bool MeetsChunkSize(this int i, int chunkSize = (1024 * 128)) => (i / chunkSize) == (i / (float)(chunkSize));
    public static string NS(this string str)
    {
        return str.Replace(" ", "");
    }
    public static string ToHexString(this byte b)
    {
        return BitConverter.ToString(new byte[1] { b }).Replace("-", " ");
    }
    public static string ToHexString(this int i)
    {
        return BitConverter.ToString(new byte[1] { (byte)i }).Replace("-", " ");
    }

    public static string FilterString(this string input, string charMap)
    {
        // Create a new StringBuilder
        StringBuilder sb = new();
        // Loop through each character in the input string
        foreach (char c in input)
        {
            // Check if the character is in the character map
            if (charMap.Contains(c))
            {
                // Add the character to the StringBuilder
                sb.Append(c);
            }
        }
        // Return the filtered string
        return sb.ToString();
    }
}

