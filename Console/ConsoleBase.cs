// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Text;

// Console Base Class (from System.Console)
namespace CustomConsole;

public static partial class Console
{
    public static TextReader In => System.Console.In;

    public static Encoding InputEncoding
    {
        get { return System.Console.InputEncoding; }
        set { System.Console.InputEncoding = value; }
    }

    public static Encoding OutputEncoding
    {
        get { return System.Console.OutputEncoding; }
        set { System.Console.OutputEncoding = value; }
    }

    public static bool KeyAvailable => System.Console.KeyAvailable;

    public static ConsoleKeyInfo ReadKey() => System.Console.ReadKey(false);

    public static ConsoleKeyInfo ReadKey(bool intercept) => System.Console.ReadKey(intercept);

    public static TextWriter Out => System.Console.Out;

    public static TextWriter Error => System.Console.Error;

    public static bool IsInputRedirected => System.Console.IsInputRedirected;

    public static bool IsOutputRedirected => System.Console.IsOutputRedirected;

    public static bool IsErrorRedirected => System.Console.IsErrorRedirected;

    public static int CursorSize
    {
        get { return System.Console.CursorSize; }
        set { System.Console.CursorSize = value; }
    }

    public static bool NumberLock => System.Console.NumberLock;

    public static bool CapsLock => System.Console.CapsLock;

    public static ConsoleColor BackgroundColor
    {
        get { return System.Console.BackgroundColor; }
        set { System.Console.BackgroundColor = value; }
    }

    public static ConsoleColor ForegroundColor
    {
        get { return System.Console.ForegroundColor; }
        set { System.Console.ForegroundColor = value; }
    }

    public static void ResetColor() => System.Console.ResetColor();

    public static int BufferWidth
    {
        get { return System.Console.BufferWidth; }
        set { System.Console.BufferWidth = value; }
    }

    public static int BufferHeight
    {
        get { return System.Console.BufferHeight; }
        set { System.Console.BufferHeight = value; }
    }

    public static void SetBufferSize(int width, int height) => System.Console.SetBufferSize(width, height);

    public static int WindowLeft
    {
        get { return System.Console.WindowLeft; }
        set { System.Console.WindowLeft = value; }
    }

    public static int WindowTop
    {
        get { return System.Console.WindowTop; }
        set { System.Console.WindowTop = value; }
    }

    public static int WindowWidth
    {
        get { return System.Console.WindowWidth; }
        set { System.Console.WindowWidth = value; }
    }

    public static int WindowHeight
    {
        get { return System.Console.WindowHeight; }
        set { System.Console.WindowHeight = value; }
    }

    public static void SetWindowPosition(int left, int top) => System.Console.SetWindowPosition(left, top);

    public static void SetWindowSize(int width, int height) => System.Console.SetWindowSize(width, height);

    public static int LargestWindowWidth => System.Console.LargestWindowWidth;

    public static int LargestWindowHeight => System.Console.LargestWindowHeight;

    public static bool CursorVisible
    {
        get { return System.Console.CursorVisible; }
        set { System.Console.CursorVisible = value; }
    }

    public static int CursorLeft
    {
        get { return System.Console.GetCursorPosition().Left; }
        set { System.Console.CursorLeft = value; }
    }

    public static int CursorTop
    {
        get { return System.Console.GetCursorPosition().Top; }
        set { System.Console.CursorTop = value; }
    }

    public static (int Left, int Top) GetCursorPosition() => System.Console.GetCursorPosition();

    public static string Title
    {
        get { return System.Console.Title; }
        set { System.Console.Title = value; }
    }

    public static void Beep() => System.Console.Beep();

    public static void Beep(int frequency, int duration) => System.Console.Beep(frequency, duration);

    public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft,
        int targetTop) =>
        System.Console.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop, ' ',
            ConsoleColor.Black, BackgroundColor);

    public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft,
        int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor) =>
        System.Console.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop,
            sourceChar, sourceForeColor, sourceBackColor);

    public static void Clear() => System.Console.Clear();

    public static void SetCursorPosition(int left, int top) => System.Console.SetCursorPosition(left, top);

    public static event ConsoleCancelEventHandler? CancelKeyPress
    {
        add { System.Console.CancelKeyPress += value; }
        remove { System.Console.CancelKeyPress -= value; }
    }

    public static bool TreatControlCAsInput
    {
        get { return System.Console.TreatControlCAsInput; }
        set { System.Console.TreatControlCAsInput = value; }
    }

    public static Stream OpenStandardInput() => System.Console.OpenStandardInput();

    public static Stream OpenStandardInput(int bufferSize) => System.Console.OpenStandardInput(bufferSize);

    public static Stream OpenStandardOutput() => System.Console.OpenStandardOutput();

    public static Stream OpenStandardOutput(int bufferSize) => System.Console.OpenStandardOutput(bufferSize);

    public static Stream OpenStandardError() => System.Console.OpenStandardError();

    public static Stream OpenStandardError(int bufferSize) => System.Console.OpenStandardError(bufferSize);

    public static void SetIn(TextReader newIn) => System.Console.SetIn(newIn);

    public static void SetOut(TextWriter newOut) => System.Console.SetOut(newOut);

    public static void SetError(TextWriter newError) => System.Console.SetError(newError);

    public static int Read() => System.Console.In.Read();

    public static string? ReadLine() => System.Console.In.ReadLine();

    public static void WriteLine() => System.Console.WriteLine();

    public static void WriteLine(bool value) => System.Console.WriteLine(value);

    public static void WriteLine(char value) => System.Console.WriteLine(value);

    public static void WriteLine(char[]? buffer) => System.Console.WriteLine(buffer);

    public static void WriteLine(char[] buffer, int index, int count) => System.Console.WriteLine(buffer, index, count);

    public static void WriteLine(decimal value) => System.Console.WriteLine(value);

    public static void WriteLine(double value) => System.Console.WriteLine(value);

    public static void WriteLine(float value) => System.Console.WriteLine(value);

    public static void WriteLine(int value) => System.Console.WriteLine(value);

    public static void WriteLine(uint value) => System.Console.WriteLine(value);

    public static void WriteLine(long value) => System.Console.WriteLine(value);

    public static void WriteLine(ulong value) => System.Console.WriteLine(value);

    public static void WriteLine(object? value) => System.Console.WriteLine(value);

    public static void WriteLine(string? value) => System.Console.WriteLine(value);

    public static void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0) =>
        System.Console.WriteLine(format, arg0);

    public static void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0,
        object? arg1) =>
        System.Console.WriteLine(format, arg0, arg1);

    public static void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0,
        object? arg1, object? arg2) =>
        System.Console.WriteLine(format, arg0, arg1, arg2);

    public static void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format,
        params object?[]? arg) =>
        System.Console.WriteLine(format, arg);

    public static void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0) =>
        System.Console.Write(format, arg0);

    public static void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0,
        object? arg1) =>
        System.Console.Write(format, arg0, arg1);

    public static void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0,
        object? arg1, object? arg2) =>
        System.Console.Write(format, arg0, arg1, arg2);

    public static void
        Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[]? arg) =>
        System.Console.Write(format, arg);

    public static void Write(bool value) => System.Console.Write(value);

    public static void Write(char value) => System.Console.Write(value);

    public static void Write(char[]? buffer) => System.Console.Write(buffer);

    public static void Write(char[] buffer, int index, int count) => System.Console.Write(buffer, index, count);

    public static void Write(double value) => System.Console.Write(value);

    public static void Write(decimal value) => System.Console.Write(value);

    public static void Write(float value) => System.Console.Write(value);

    public static void Write(int value) => System.Console.Write(value);

    public static void Write(uint value) => System.Console.Write(value);

    public static void Write(long value) => System.Console.Write(value);

    public static void Write(ulong value) => System.Console.Write(value);

    public static void Write(object? value) => System.Console.Write(value);

    public static void Write(string? value) => System.Console.Write(value);
}