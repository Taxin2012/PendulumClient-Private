﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace PendulumClient
{
    class SetScreenColorsApp
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            internal short X;
            internal short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SMALL_RECT
        {
            internal short Left;
            internal short Top;
            internal short Right;
            internal short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct COLORREF
        {
            internal uint ColorDWORD;

            internal COLORREF(Color color)
            {
                ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }

            internal COLORREF(uint r, uint g, uint b)
            {
                ColorDWORD = r + (g << 8) + (b << 16);
            }

            internal Color GetColor()
            {
                return Color.FromArgb((int)(0x000000FFU & ColorDWORD),
                                      (int)(0x0000FF00U & ColorDWORD) >> 8, (int)(0x00FF0000U & ColorDWORD) >> 16);
            }

            internal void SetColor(Color color)
            {
                ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CONSOLE_SCREEN_BUFFER_INFO_EX
        {
            internal int cbSize;
            internal COORD dwSize;
            internal COORD dwCursorPosition;
            internal ushort wAttributes;
            internal SMALL_RECT srWindow;
            internal COORD dwMaximumWindowSize;
            internal ushort wPopupAttributes;
            internal bool bFullscreenSupported;
            internal COLORREF black;
            internal COLORREF darkBlue;
            internal COLORREF darkGreen;
            internal COLORREF darkCyan;
            internal COLORREF darkRed;
            internal COLORREF darkMagenta;
            internal COLORREF darkYellow;
            internal COLORREF gray;
            internal COLORREF darkGray;
            internal COLORREF blue;
            internal COLORREF green;
            internal COLORREF cyan;
            internal COLORREF red;
            internal COLORREF magenta;
            internal COLORREF yellow;
            internal COLORREF white;
        }

        const int STD_OUTPUT_HANDLE = -11;                                        // per WinBase.h
        internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);    // per WinBase.h

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

        // Set a specific console color to an RGB color
        // The default console colors used are gray (foreground) and black (background)
        public static int SetColor(ConsoleColor consoleColor, Color targetColor)
        {
            return SetColor(consoleColor, targetColor.R, targetColor.G, targetColor.B);
        }

        public static int SetColor(ConsoleColor color, uint r, uint g, uint b)
        {
            CONSOLE_SCREEN_BUFFER_INFO_EX csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
            csbe.cbSize = (int)Marshal.SizeOf(csbe);                    // 96 = 0x60
            IntPtr hConsoleOutput = GetStdHandle(STD_OUTPUT_HANDLE);    // 7
            if (hConsoleOutput == INVALID_HANDLE_VALUE)
            {
                return Marshal.GetLastWin32Error();
            }
            bool brc = GetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
            if (!brc)
            {
                return Marshal.GetLastWin32Error();
            }

            switch (color)
            {
                case ConsoleColor.Black:
                    csbe.black = new COLORREF(r, g, b);
                    break;
                case ConsoleColor.DarkBlue:
                    csbe.darkBlue = new COLORREF(r, g, b);
                    break;
                case ConsoleColor.DarkGreen:
                    csbe.darkGreen = new COLORREF(r, g, b);
                    break;
                case ConsoleColor.DarkCyan:
                    csbe.darkCyan = new COLORREF(r, g, b);
                    break;
                case ConsoleColor.DarkRed:
                    csbe.darkRed = new COLORREF(r, g, b);
                    break;
                case ConsoleColor.DarkMagenta:
                    csbe.darkMagenta = new COLORREF(r, g, b);
                    break;
                case ConsoleColor.DarkYellow:
                    csbe.darkYellow = new COLORREF(r, g, b);
                    break;
                case ConsoleColor.Gray:
                    csbe.gray = new COLORREF(r, g, b);
                    break;
                case ConsoleColor.DarkGray:
                    csbe.darkGray = new COLORREF(r, g, b);
                    break;
                case ConsoleColor.Blue:
                    csbe.blue = new COLORREF(r, g, b);
                    break;
                case ConsoleColor.Green:
                    csbe.green = new COLORREF(r, g, b);
                    break;
                case ConsoleColor.Cyan:
                    csbe.cyan = new COLORREF(r, g, b);
                    break;
                case ConsoleColor.Red:
                    csbe.red = new COLORREF(r, g, b);
                    break;
                case ConsoleColor.Magenta:
                    csbe.magenta = new COLORREF(r, g, b);
                    break;
                case ConsoleColor.Yellow:
                    csbe.yellow = new COLORREF(r, g, b);
                    break;
                case ConsoleColor.White:
                    csbe.white = new COLORREF(r, g, b);
                    break;
            }
            ++csbe.srWindow.Bottom;
            ++csbe.srWindow.Right;
            brc = SetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
            if (!brc)
            {
                return Marshal.GetLastWin32Error();
            }
            return 0;
        }

        public static int SetScreenColors(Color foregroundColor, Color backgroundColor)
        {
            int irc;
            irc = SetColor(ConsoleColor.Gray, foregroundColor);
            if (irc != 0) return irc;
            irc = SetColor(ConsoleColor.Black, backgroundColor);
            if (irc != 0) return irc;

            return 0;
        }
    }

    class PendulumLogger
    {
        public static bool ColorsSetup = false;
        public static void SetupColors()
        {
            SetScreenColorsApp.SetColor(ConsoleColor.Red, Color.FromArgb(255, 253, 25, 90));
            SetScreenColorsApp.SetColor(ConsoleColor.DarkGreen, Color.FromArgb(255, 0, 255, 0));
            SetScreenColorsApp.SetColor(ConsoleColor.Magenta, Color.FromArgb(255, 100, 255, 100));
            SetScreenColorsApp.SetColor(ConsoleColor.DarkRed, Color.FromArgb(255, 255,100, 100));
            ConsoleFontManager.SetConsoleFont();
        }
        public static void Log(string Log)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("[LOG] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Log + "\n");
        }
        public static void Log(string Log, ConsoleColor color)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("[LOG] ");
            Console.ForegroundColor = color;
            Console.Write(Log + "\n");
        }
        public static void Log(ConsoleColor color, string Log)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("[LOG] ");
            Console.ForegroundColor = color;
            Console.Write(Log + "\n");
        }
        public static void Log(ConsoleColor color, string Log, params object[] args)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("[LOG] ");
            Console.ForegroundColor = color;
            Log = string.Format(Log, args);
            Console.Write(Log + "\n");
        }
        public static void Log(string Log, params object[] args)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("[LOG] ");
            Console.ForegroundColor = ConsoleColor.White;
            Log = string.Format(Log, args);
            Console.Write(Log + "\n");
        }
        public static void LogHarmony(string Log)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("[HARMONY] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Log + "\n");
        }
        public static void DebugLog(string Log)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("[DEBUG] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Log + "\n");
        }
        public static void DebugLog(string Log, params object[] args)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("[DEBUG] ");
            Console.ForegroundColor = ConsoleColor.White;
            Log = string.Format(Log, args);
            Console.Write(Log + "\n");
        }

        public static void EventLog(string Log)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("[EVENT] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Log + "\n");
        }
        public static void EventLog(string Log, params object[] args)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("[EVENT] ");
            Console.ForegroundColor = ConsoleColor.White;
            Log = string.Format(Log, args);
            Console.Write(Log + "\n");
        }
        public static void SocialLog(string Log)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("[SOCIAL] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Log + "\n");
        }
        public static void JoinLog(string Log)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("[JOIN] ");
            Console.Write(Log + "\n");
        }
        public static void LeaveLog(string Log)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("[LEAVE] ");
            Console.Write(Log + "\n");
        }

        public static void ModerationLog(string Log)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[MODERATION] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Log + "\n");
        }

        public static void LogWarning(string Warning)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[WARNING] " + Warning + "\n");
        }
        public static void LogError(string Error)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("[ERROR] " + Error + "\n");
        }
        public static void LogErrorSevere(string Error)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("[ERROR] " + Error + "\n");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("[ERROR] " + "Press any key to close" + "\n");
            Console.ReadKey();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        public static void Error(string Error)
        {
            if (ColorsSetup == false)
                SetupColors();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("PendulumClient");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("[ERROR] " + Error + "\n");
        }
    }

    internal class ConsoleFontManager
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal unsafe struct CONSOLE_FONT_INFO_EX
        {
            internal uint cbSize;
            internal uint nFont;
            internal COORD dwFontSize;
            internal int FontFamily;
            internal int FontWeight;
            internal fixed char FaceName[LF_FACESIZE];
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            internal short X;
            internal short Y;

            internal COORD(short x, short y)
            {
                X = x;
                Y = y;
            }
        }
        private const int STD_OUTPUT_HANDLE = -11;
        private const int TMPF_TRUETYPE = 4;
        private const int LF_FACESIZE = 32;
        private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetCurrentConsoleFontEx(
            IntPtr consoleOutput,
            bool maximumWindow,
            ref CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int dwType);


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int SetConsoleFont(
            IntPtr hOut,
            uint dwFontNum
            );
        public static bool SetConsoleFont(string fontName = "Secret Code")
        {
            try
            {
                unsafe
                {
                    IntPtr hnd = GetStdHandle(STD_OUTPUT_HANDLE);
                    if (hnd != INVALID_HANDLE_VALUE)
                    {
                        CONSOLE_FONT_INFO_EX info = new CONSOLE_FONT_INFO_EX();
                        info.cbSize = (uint)Marshal.SizeOf(info);

                        // Set console font to Lucida Console.
                        CONSOLE_FONT_INFO_EX newInfo = new CONSOLE_FONT_INFO_EX();
                        newInfo.cbSize = (uint)Marshal.SizeOf(newInfo);
                        newInfo.FontFamily = TMPF_TRUETYPE;
                        IntPtr ptr = new IntPtr(newInfo.FaceName);
                        Marshal.Copy(fontName.ToCharArray(), 0, ptr, fontName.Length);

                        // Get some settings from current font.
                        newInfo.dwFontSize = new COORD(info.dwFontSize.X, info.dwFontSize.Y);
                        newInfo.FontWeight = info.FontWeight;
                        SetCurrentConsoleFontEx(hnd, false, ref newInfo);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                PendulumLogger.Log("well guess no custom font for you");
                return false;
            }
        }
    }
}
