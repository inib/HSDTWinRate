using System.IO;
using Hearthstone_Deck_Tracker;
using System;
using System.Windows.Data;

namespace HSDTWinrate
{
    public enum TimeFrame
    {
        today,
        week,
        season,
        total,
        custom
    }

    public enum DeckVersion
    {
        current,
        all
    }

    public enum OverallVersion
    {
        constructed,
        arena,
        both,
        auto
    }

	public class Config
	{
		private static Config _instance;

		public Config()
		{
			StatDeckTime = TimeFrame.today;            
			StatDeckVer = DeckVersion.current;
            StatDeckCustomTime = DateTime.Now;

            StatOverallTime = TimeFrame.season;
            StatOverallVer = OverallVersion.auto;
            StatOverallCustomTime = DateTime.Now;

            OutputDeckName = false;
            OutputDeckStats = false;
            OutputDeckCombined = true;
            OutputOverall = false;

            OutputPath = Hearthstone_Deck_Tracker.Config.Instance.DataDir;
		}

		public static Config Instance
		{
			get { return _instance ?? Load(); }
		}
        		
        public TimeFrame StatDeckTime { get; set; }
        public DateTime StatDeckCustomTime { get; set; }
		public DeckVersion StatDeckVer { get; set; }

        public TimeFrame StatOverallTime { get; set; }
        public DateTime StatOverallCustomTime { get; set; }
        public OverallVersion StatOverallVer { get; set; }

        public bool OutputDeckStats { get; set; }
        public bool OutputDeckName { get; set; }
        public bool OutputDeckCombined { get; set; }
        public bool OutputOverall { get; set; }
        public string OutputPath { get; set; }

		private static string FilePath
		{
			get { return Path.Combine(Hearthstone_Deck_Tracker.Config.Instance.ConfigDir, "WinRatePlugin.xml"); }
		}

		public static T GetConfigItem<T>(string name)
		{
			object prop = Instance.GetType().GetProperty(name).GetValue(Instance, null);
			if(prop == null)
				return default(T);
			return (T)prop;
		}

		public static void Save()
		{
			XmlManager<Config>.Save(FilePath, Instance);
		}

		private static Config Load()
		{
			return _instance = File.Exists(FilePath) ? XmlManager<Config>.Load(FilePath) : new Config();
		}
	}
}