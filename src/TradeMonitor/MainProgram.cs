using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Console = Colorful.Console;
using System.Media;
using System.Threading;

namespace TradeMonitor
{
    class MainProgram
    {
        //public static FX_Parser _AccountInstance;

        static void Main(string[] args)
        {
            DisplayPages.DisplayTitle();

            if (args.Length != 0)
            {
                Cinstance.Out($"cmd line arg found", Color.ForestGreen, 1);
                Cinstance.Out($"Attempting to load from {args[0]}", Color.LightSeaGreen, 1);
                var account = new FX_Parser(args[0]);
                if (account.Read())
                {
                    account.Use();
                }
                else
                {
                    account.Dispose();
                    Cinstance.Out("Returning to menu", Color.IndianRed);
                    Utils.Pause();
                    Console.Clear();
                    Main(new string[0]);
                }
            }

            if (DisplayPages.Init())
            {
                DisplayPages.PrintMenu();
            }

            while (true)
            {
                Console.ReadKey();
            }
        }
    }

    static class DisplayPages
    {
        public static void DisplayTitle()
        {
            Console.WindowWidth = Convert.ToInt32(Console.WindowWidth * 1.2);
            Console.WriteAscii("Trade Monitor by Weii", Color.DarkSlateBlue);
            Utils.Pause();
            Cinstance.Out("This program was written for personal use, do not distribute without direct permission", Color.DarkSlateBlue, 1);
        }

        public static bool Init()
        {
            bool _initComplete = false;

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Console.CursorVisible = false;
                int _dotCount = 3;
                while (!_initComplete)
                {
                    if (_dotCount == 3)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Utils.ClearCurrentConsoleLine();
                        Cinstance.Out("Init.", Color.DarkSlateBlue, _disableSound: true);
                        _dotCount = 1;
                        Utils.Pause(100);
                    }
                    else if (_dotCount == 2)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Utils.ClearCurrentConsoleLine();
                        Cinstance.Out("Init...", Color.DarkSlateBlue, _disableSound: true);
                        _dotCount = 3;
                        Utils.Pause(100);
                    }
                    else if (_dotCount == 1)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Utils.ClearCurrentConsoleLine();
                        Cinstance.Out("Init..", Color.DarkSlateBlue, _disableSound: true);
                        _dotCount = 2;
                        Utils.Pause(100);
                    }
                }
                Console.CursorVisible = true;
            }).Start();
            return Utils.Init(ref _initComplete);
        }

        public static void PrintMenu()
        {
            Cinstance.Out(@"""d"" for database functions (e.x search)", Color.CadetBlue); Utils.Pause(100);
            Cinstance.Out(@"""n"" to create new entry", Color.CadetBlue); Utils.Pause(100);
            Cinstance.Out(@"""d"" to delete entry", Color.CadetBlue); Utils.Pause(100);
            Cinstance.Out(@"""s"" to edit settings", Color.CadetBlue); Utils.Pause(100);
        }
    }
}
