﻿using Microsoft.Extensions.Configuration;
using Neo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Neo
{
    class ReportNeoBlockchain
    {
	public DateTime? d;
	public string header;

	public ReportNeoBlockchain(string _header)
	{
	   this.d = DateTime.Now;
	   this.header = _header;
           Console.WriteLine("Starting test at " + header);
	}

	public void appendElapsedTime()
	{
            string output = "./Report-" + header + ".txt";
	    DateTime? d2 = DateTime.Now;
            string reportMessage= "Elapsed in seconds are " + header + ": " + (double)(d2 - d).GetValueOrDefault().TotalSeconds + "\n";
	    Console.WriteLine(reportMessage);
	    File.AppendAllText(output, reportMessage);
            //File.WriteAllText("./Report.txt", reportMessage);
	}          
    }


    internal class Settings
    {
        public uint Magic { get; private set; }
        public byte AddressVersion { get; private set; }
        public int MaxTransactionsPerBlock { get; private set; }
        public string[] StandbyValidators { get; private set; }
        public string[] SeedList { get; private set; }
        public IReadOnlyDictionary<TransactionType, Fixed8> SystemFee { get; private set; }
        public uint SecondsPerBlock { get; private set; }

        public static Settings Default { get; private set; }

        static Settings()
        {
            IConfigurationSection section = new ConfigurationBuilder().AddJsonFile("protocol.json").Build().GetSection("ProtocolConfiguration");
            Default = new Settings(section);
        }

        public Settings(IConfigurationSection section)
        {
            this.Magic = uint.Parse(section.GetSection("Magic").Value);
            this.AddressVersion = byte.Parse(section.GetSection("AddressVersion").Value);
            this.MaxTransactionsPerBlock = GetValueOrDefault(section.GetSection("MaxTransactionsPerBlock"), 500, p => int.Parse(p));
            this.StandbyValidators = section.GetSection("StandbyValidators").GetChildren().Select(p => p.Value).ToArray();
            this.SeedList = section.GetSection("SeedList").GetChildren().Select(p => p.Value).ToArray();
            this.SystemFee = section.GetSection("SystemFee").GetChildren().ToDictionary(p => (TransactionType)Enum.Parse(typeof(TransactionType), p.Key, true), p => Fixed8.Parse(p.Value));
            this.SecondsPerBlock = GetValueOrDefault(section.GetSection("SecondsPerBlock"), 15u, p => uint.Parse(p));
        }

        public T GetValueOrDefault<T>(IConfigurationSection section, T defaultValue, Func<string, T> selector)
        {
            if (section.Value == null) return defaultValue;
            return selector(section.Value);
        }
    }
}
