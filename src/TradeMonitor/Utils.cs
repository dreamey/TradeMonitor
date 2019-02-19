using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Media;
using Console = Colorful.Console;

namespace TradeMonitor
{
    static class Utils
    {
        public static void Pause(int _ms = 500)
        {
            Thread.Sleep(_ms);
        }

        public static void ClearCurrentConsoleLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public static bool Init(ref bool _initComplete)
        {
            Thread.Sleep(1000);
            string[] savedAccounts = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.fx",SearchOption.TopDirectoryOnly);
            if (savedAccounts.Length >= 1)
            {
                Cinstance.Out($"Found {savedAccounts.Length} accounts", Color.BlueViolet, 1, _prefixWithNewLines:1);
                _initComplete = true;
                //User file exists
                foreach (var account in savedAccounts)
                {
                    Cinstance.Out(Path.GetFileNameWithoutExtension(account), Color.BlueViolet);
                }
                return true;
            }
            else
            {
                _initComplete = true;
                Pause();
                Cinstance.Out("No saved accounts could not be found, to ensure they are found on startup, ensure they are in the root directory, setup is loading", Color.HotPink, 1, _prefixWithNewLines:1);
                Pause(1000);
                Console.Clear();
                Setup.Run();
                return false;
            }
        }
    }

    static class Cinstance
    {
        public static readonly INI Settings = new INI("settings.ini");

        private static readonly bool PlaySounds = Convert.ToBoolean(Settings.Read("Sounds"));
        private static readonly int ProgressiveDelay = Convert.ToInt32(Settings.Read("ProgressiveTextDelay"));
        public static readonly bool AccountTitle = Convert.ToBoolean(Settings.Read("AccountTitle"));


        public static void Out(string text, Color _c, int _newLineCount = 0, bool _disableSound = false, bool _nextBufferOnSameLine = false, int _prefixWithNewLines = 0)
        {
            if(_prefixWithNewLines != 0)
            {
                for (int i = 0; i < _prefixWithNewLines; i++)
                {
                    Console.WriteLine();
                }
            }
            for (int i = 0; i < text.Length; i++)
            {
                Console.Write(text[i], _c);
                Utils.Pause(ProgressiveDelay);
            }
            if(_newLineCount == 0 && _nextBufferOnSameLine == false)
            {
                Console.Write(Environment.NewLine);
            }

            if (_newLineCount != 0) { _newLineCount = _newLineCount * 2; }
            for (int i = 0; i < _newLineCount; i++)
            {
                Console.WriteLine();
            }

            if (PlaySounds)
            {
                if (!_disableSound)
                {
                    SoundPlayer _whenType = new SoundPlayer($@"{AppDomain.CurrentDomain.BaseDirectory}\assets\whenType.wav");
                    _whenType.PlaySync();
                }
            }
        }

        //Run this function from a new thread created when it is called, code runs on default thread by default
        public static void InitDisplay(ref bool _taskComplete, ref bool _initLoopComplete, string loadText = "Init", int _manuallyClearLastLine = 0)
        {
            Console.CursorVisible = false;
            int _dotCount = 3;
            while (!_taskComplete)
            {
                if (_dotCount == 3)
                {
                    Utils.ClearCurrentConsoleLine();
                    Cinstance.Out($"{loadText}.", Color.DarkSlateBlue, _disableSound: true);
                    _dotCount = 1;
                    Utils.Pause(100);
                }
                else if (_dotCount == 2)
                {
                    Utils.ClearCurrentConsoleLine();
                    Cinstance.Out($"{loadText}...", Color.DarkSlateBlue, _disableSound: true);
                    _dotCount = 3;
                    Utils.Pause(100);
                }
                else if (_dotCount == 1)
                {
                    Utils.ClearCurrentConsoleLine();
                    Cinstance.Out($"{loadText}..", Color.DarkSlateBlue, _disableSound: true);
                    _dotCount = 2;
                    Utils.Pause(100);
                }
            }
            if(_manuallyClearLastLine != 0)
            {
                for (int i = 0; i < _manuallyClearLastLine; i++)
                {
                    Utils.ClearCurrentConsoleLine();
                }
            }
            Utils.Pause();
            Console.CursorVisible = true;
            _taskComplete = true;
        }
    }
}
