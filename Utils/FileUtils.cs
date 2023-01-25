namespace McPatch;
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
            if (i.MeetsChunkSize()) Console.Write($"\r{Console.Prefix("FileUtils")} Writing to {Path.GetFileName(path)}: {Console.Value($"{fs.Position / (float)bytes.Length * 100}%")} complete   ");
            i++;
        }
        Console.Write("\r                                                                                \r");
        fs.Close();
    }
}
