using System.Globalization;
using System.Text;

namespace McPatch;
public static class Extensions
{
    public static string ToHexString(this byte[] bytes)
    {
        if (bytes == null) return string.Empty;
        return BitConverter.ToString(bytes).Replace("-", " ");
    }
    public static string ToHexStringO(this byte[] bytes)
    {
        if (bytes == null) return string.Empty;
        StringBuilder sb = new(bytes.Length * 2);
        foreach (byte b in bytes)
        {
            sb.AppendFormat("{0:x2}", b);
        }
        return sb.ToString();
    }

    public static byte[] FromHexString(this string str)
    {
        byte[] bytes = new byte[str.Length / 2];
        for (int i = 0; i < str.Length; i += 2)
        {
            bytes[i / 2] = byte.Parse(str.Substring(i, 2), NumberStyles.HexNumber);
        }
        return bytes;
    }
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

