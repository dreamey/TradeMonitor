using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using Console = Colorful.Console;
using System.Threading;

namespace TradeMonitor
{
    class FX_Parser : IDisposable
    {
        static int InstanceCount = 0;
        private static string InitialTitle;
        public bool IsValid = false;
        public string Path;

        private Setup.AccountSetup CurrentAccount = new Setup.AccountSetup();

        public FX_Parser(string path)
        {
            if(InstanceCount != 0) { throw new InvalidOperationException("Instance of this class already exists"); }
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }
            Path = path;
            InstanceCount++;
        }

        public bool Read()
        {
            try
            {
                using (BinaryReader b = new BinaryReader(File.Open(Path, FileMode.Open)))
                {
                    CurrentAccount.AccountNumber = b.ReadInt32();
                    Cinstance.Out($"Account Number: {CurrentAccount.AccountNumber}", Color.HotPink);
                    if (Cinstance.AccountTitle)
                    {
                        InitialTitle = Console.Title;
                        Console.Title = CurrentAccount.AccountNumber.ToString();
                    }
                    CurrentAccount.AccountName = b.ReadString();
                    Cinstance.Out($"Account name: {CurrentAccount.AccountName}", Color.HotPink);
                    CurrentAccount.AccountEquity = b.ReadDouble();
                    Cinstance.Out($"Account equity: {CurrentAccount.AccountEquity}", Color.HotPink);
                    CurrentAccount.Currency = b.ReadChar();
                    Cinstance.Out($"Account currency: {CurrentAccount.Currency}", Color.HotPink, 1);
                }
                IsValid = true;
                return true;
            }
            catch (Exception)
            {
                Cinstance.Out("Could not read the binary file, either it is corrupt or it was created using an older version of TradeMonitor", Color.IndianRed);
                return false;
            }
        }

        public void Use()
        {
            Cinstance.Out($"Succesfully loaded {CurrentAccount.AccountName} ({CurrentAccount.AccountNumber})", Color.LightGoldenrodYellow);
            Utils.Pause(50000);
        }

        public void Dispose()
        {
            Console.Title = InitialTitle;
            InstanceCount -= 1;
        }
    }

    class FX_Scribe
    {
        private Setup.AccountSetup Account;

        public FX_Scribe(Setup.AccountSetup account)
        {
            if(account.AccountEquity != 0 && account.AccountName != null && account.AccountNumber != 0 && account.Currency != null)
            {
                this.Account = account;
            }
        }

        public void WriteToFile(ref bool _writeComplete)
        {
            using (BinaryWriter b = new BinaryWriter(File.Open($"{Account.AccountNumber}.fx", FileMode.Create)))
            {
                Thread.Sleep(2000);
                b.Write(Account.AccountNumber);
                b.Write(Account.AccountName);
                b.Write(Account.AccountEquity);
                b.Write(Account.Currency);
            }
            _writeComplete = true;
        }

        public void WriteToFileOnNewThread()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                using (BinaryWriter b = new BinaryWriter(File.Open($"{Account.AccountNumber}.fx", FileMode.Create)))
                {
                    b.Write(Account.AccountNumber);
                    b.Write(Account.AccountName);
                    b.Write(Account.AccountEquity);
                    b.Write(Account.Currency);
                }
            }).Start();
        }
    }
}
