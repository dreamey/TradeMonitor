using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using Console = Colorful.Console;

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
            InstanceCount = InstanceCount - 1;
        }
    }

    class FX_Scribe
    {
        public FX_Scribe(Setup.AccountSetup account)
        {
            if(account.AccountEquity != 0 && account.AccountName != null && account.AccountNumber != 0 && account.Currency != null)
            {
                using (BinaryWriter b = new BinaryWriter(File.Open($"{account.AccountNumber}.fx", FileMode.Create)))
                {
                    b.Write(account.AccountNumber);
                    b.Write(account.AccountName);
                    b.Write(account.AccountEquity);
                    b.Write(account.Currency);
                }
            }
        }
    }
}
