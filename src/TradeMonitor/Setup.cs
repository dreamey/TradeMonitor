using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Console = Colorful.Console;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

namespace TradeMonitor
{
    static class Setup
    {
        public struct AccountSetup
        {
            public int AccountNumber;
            public string AccountName;
            public double AccountEquity;
            public char Currency;
        }

        private static readonly Dictionary<char, char> Index2Currency = new Dictionary<char, char>
        {
            { 'a','£' },
            { 'b','$' },
            { 'c','€' },
            { 'd','Z' }
        };

        private static readonly Dictionary<string, string> Index2Description = new Dictionary<string, string>
        {
            { "a","GBP" },
            { "b","USD" },
            { "c","EUR" },
            { "d","ZAR" }
        };

        public static void Run()
        {
            var _account = new AccountSetup();

            Console.WriteAscii("Setup", Color.DodgerBlue);
            Cinstance.Out("The information provided here can be changed once setup is finished", Color.CornflowerBlue, 1);

            bool AccountNumberEntry = false;
            string AccountNumber = "";
            while (!AccountNumberEntry)
            {
                Cinstance.Out("Account Number: ", Color.HotPink, _nextBufferOnSameLine:true);
                AccountNumber = Console.ReadLine();
                if(File.Exists(AccountNumber.ToString() + ".fx"))
                {
                    Cinstance.Out("Account file already exists, to prevent this, change the account number or delete the current file", Color.OrangeRed);
                }
                else if (!Regex.IsMatch(AccountNumber, "^[0-9]+$"))
                {
                    Cinstance.Out("Value was not numerical (regex used - ^\\d$)", Color.OrangeRed);
                }
                else { AccountNumberEntry = true; }
            }
            _account.AccountNumber = Convert.ToInt32(AccountNumber);
            Cinstance.Out($"Account number bound to value ", Color.HotPink, _nextBufferOnSameLine: true);
            Cinstance.Out(_account.AccountNumber.ToString(), Color.DeepPink, 1);

            bool AccountNameEntry = false;
            string AccountName = "";
            while (!AccountNameEntry)
            {
                Cinstance.Out("Account Name: ", Color.HotPink, _nextBufferOnSameLine: true);
                AccountName = Console.ReadLine();
                if (AccountName.Length > 16 || !Regex.IsMatch(AccountName, "[a-zA-Z]+"))
                {
                    Cinstance.Out("Value violated input rules", Color.OrangeRed);
                    Cinstance.Out("     -16 chars max", Color.CadetBlue);
                    Cinstance.Out("     -Letters only", Color.CadetBlue);
                }
                else { AccountNameEntry = true; }
            }
            _account.AccountName = AccountName;
            Cinstance.Out($"Account name bound to value ", Color.HotPink, _nextBufferOnSameLine: true);
            Cinstance.Out(_account.AccountName, Color.DeepPink, 1);

            bool AccountCurrencyEntry = false;
            char AccountCurrency = ' ';
            while (!AccountCurrencyEntry)
            {
                Cinstance.Out("Account Currency -select index-: ", Color.HotPink);
                Cinstance.Out("     a) £ (GBP)", Color.LightPink);
                Cinstance.Out("     b) $ (USD)", Color.LightPink);
                Cinstance.Out("     c) € (EUR)", Color.LightPink);
                Cinstance.Out("     d) R (ZAR)", Color.LightPink);
                AccountCurrency = Console.ReadLine()[0];
                if (!Index2Currency.ContainsKey(AccountCurrency.ToString().ToLower()[0]))
                {
                    Cinstance.Out("Dictionary did not contain this index", Color.OrangeRed);
                }
                else { AccountCurrencyEntry = true; }
            }
            _account.Currency = Index2Currency[Convert.ToChar(AccountCurrency)];
            Cinstance.Out($"Account currency bound to  ", Color.HotPink, _nextBufferOnSameLine: true);
            Cinstance.Out(Index2Description[AccountCurrency.ToString()], Color.DeepPink, 2);

            bool AccountEquityEntry = false;
            string AccountEquity = "";
            while (!AccountEquityEntry)
            {
                Cinstance.Out("Account Equity: ", Color.HotPink, _nextBufferOnSameLine: true);
                AccountEquity = Console.ReadLine();
                if (Regex.IsMatch(AccountEquity, "[a-zA-Z]+"))
                {
                    Cinstance.Out("Value was not numerical (regex used - ^\\d$)", Color.OrangeRed);
                }
                else { AccountEquityEntry = true; }
            }
            _account.AccountEquity = Convert.ToDouble(AccountEquity);
            Cinstance.Out($"Account equity set to ", Color.HotPink, _nextBufferOnSameLine: true);
            Cinstance.Out(_account.AccountEquity.ToString(), Color.DeepPink, 2);

            Cinstance.Out($"Your account is being written to a new file -  {_account.AccountNumber}.fx", Color.ForestGreen, 1);
            Utils.Pause();

            bool _scribeComplete = false;
            bool _initDisplayComplete = false;
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Cinstance.InitDisplay(ref _scribeComplete, ref _initDisplayComplete, loadText:"Writing");
            }).Start();

            var scribe = new FX_Scribe(_account);
            scribe.WriteToFile(ref _scribeComplete);
            Utils.Pause();
            Cinstance.Out("Account Successfully Written", Color.CadetBlue, _prefixWithNewLines:1);
        }
    }
}
