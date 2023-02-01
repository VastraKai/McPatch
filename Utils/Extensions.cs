using System.Globalization;
using System.Text;
using System.Threading;

namespace McPatch;
public static class Extensions
{
    #region IsValidStaticAddress overrides
    /// <summary>
    /// Checks if the provided address starts with hex 7F.
    /// </summary>
    /// <param name="address">The address to check.</param>
    /// <returns>If the address starts with hex 7F.</returns>
    public static bool IsValidStaticAddress(this long address) => address.ToString("X").StartsWith("7F");
    /// <summary>
    /// Checks if the provided address starts with hex 7F.
    /// </summary>
    /// <param name="address">The address to check.</param>
    /// <returns>If the address starts with hex 7F.</returns>
    public static bool IsValidStaticAddress(this ulong address) => address.ToString("X").StartsWith("7F");
    /// <summary>
    /// Checks if the provided address starts with hex 7F.
    /// </summary>
    /// <param name="address">The address to check.</param>
    /// <returns>If the address starts with hex 7F.</returns>
    public static bool IsValidStaticAddress(this nint address) => address.ToString("X").StartsWith("7F"); // nint is the same as IntPtr
    /// <summary>
    /// Checks if the provided address starts with hex 7F.
    /// </summary>
    /// <param name="address">The address to check.</param>
    /// <returns>If the address starts with hex 7F.</returns>
    public static bool IsValidStaticAddress(this nuint address) => address.ToString("X").StartsWith("7F"); // nuint is the same as UIntPtr
    /// <summary>
    /// Checks if the provided address starts with hex 7F.
    /// </summary>
    /// <param name="address">The address to check.</param>
    /// <returns>If the address starts with hex 7F.</returns>
    public static bool IsValidStaticAddress(this string address) => address.StartsWith("7F");
    #endregion
    #region Hex string and byte array manipulation methods 
    /// <summary>
    /// Default method for converting a byte array to a hex string.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>The converted hex string.</returns>
    public static string ToHexString(this byte[] bytes)
    {
        if (bytes == null) return string.Empty;
        return BitConverter.ToString(bytes).Replace("-", " ");
    }
    /// <summary>
    /// Default method for converting a hex string to a byte array.
    /// </summary>
    /// <param name="str">The hex string to convert.</param>
    /// <returns>The converted byte array.</returns>
    public static byte[] FromHexString(this string str)
    {
        str = str.Filter("abcdefABCDEF1234567890"); // Make sure the string doesn't contain any illegal characters
        byte[] bytes = new byte[str.Length / 2];
        for (int i = 0; i < str.Length; i += 2)
        {
            bytes[i / 2] = byte.Parse(str.Substring(i, 2), NumberStyles.HexNumber);
        }
        return bytes;
    }

    /// <summary>
    /// Optimized method for converting a byte array to a hex string.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>The converted hex string.</returns>
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
    /// <summary>
    /// Optimized method for converting a hex string to a byte array.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <returns>The converted byte array.</returns>
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
    /// <summary>
    /// Converts a byte to a hex string.
    /// </summary>
    /// <param name="b">The byte to convert.</param>
    /// <returns>The converted hex string.</returns>
    public static string ToHexString(this byte b)
    {
        return BitConverter.ToString(new byte[1] { b }).Replace("-", " ");
    }
    /// <summary>
    /// Converts an int to a hex string.
    /// </summary>
    /// <param name="i">The int to convert.</param>
    /// <returns>The converted hex string.</returns>
    public static string ToHexString(this int i)
    {
        return BitConverter.ToString(BitConverter.GetBytes(i)).Replace("-", "");
    }
    /// <summary>
    /// Checks if the given bytesWritten value has a decimal when divided by chunkSize.
    /// If true, it means that the value meets the chunkSize.
    /// </summary>
    /// <param name="bytesWritten">The amount of bytes written so far</param>
    /// <param name="chunkSize">The chunk size of each section of bytes</param>
    /// <returns>If the bytesWritten meets the chunkSize.</returns>
    public static bool MeetsChunkSize(this int bytesWritten, int chunkSize = (1000 * 128)) => (bytesWritten / chunkSize) == (bytesWritten / (float)(chunkSize));
    #endregion
    #region String stuff
    /// <summary>
    /// Removes spaces from the provided string.
    /// </summary>
    /// <param name="str">The string to remove spaces from.</param>
    /// <returns>The string, without spaces.</returns>
    public static string NS(this string str)
    {
        return str.Replace(" ", "");
    }
    /// <summary>
    /// The valid filter modes.
    /// </summary>
    public enum FilterMode
    {
        /// <summary>
        /// Filter the string to include characters from the character map.
        /// </summary>
        Include,
        /// <summary>
        /// Filter the string to include characters from the character map.
        /// </summary>
        Exclude
    }
    /// <summary>
    /// Filters a string to either include or exclude characters.
    /// </summary>
    /// <param name="input">The string to filter.</param>
    /// <param name="charMap">The map of characters to keep or remove in the string.</param>
    /// <param name="mode">The filter mode.</param>
    /// <returns>The new, filtered string.</returns>
    public static string Filter(this string input, string charMap, FilterMode mode = FilterMode.Include)
    {
        // Create a new StringBuilder
        StringBuilder sb = new();
        // Loop through each character in the input string
        foreach (char c in input)
        {
            bool contains = charMap.Contains(c);
            if (mode == FilterMode.Exclude) contains = !contains;
            if (contains)
                sb.Append(c);
        }
        // Return the filtered string
        return sb.ToString();
    }
    #endregion
}

