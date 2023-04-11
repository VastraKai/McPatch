namespace McPatch.Utils;
public static class FileUtils
{
    // Method for writing a byte array to a file, with progress
    public static void WriteFile(string path, byte[] bytes)
    {
        if (File.Exists(path)) File.Delete(path);
        using System.IO.FileStream fs = new(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);

        int i = 0;
        int length = bytes.Length;
        foreach (byte b in bytes)
        {
            fs.WriteByte(b);
            if (i.MeetsChunkSize())
                Console.Log.Write("FileUtils", $"Writing to &v{Path.GetFileName(path)}&r: &v{fs.Position / (float)bytes.Length * 100}%&r complete        \r", LogLevel.Debug);
            i++;
        }
        Console.Write("\r                                                                                \r");
        fs.Close();
    }
}
