using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDTWinrate
{

    /// <summary>
    /// Databinding model for the config file
    /// </summary>

    public class ConfigProp
    {
        public TimeFrame TimeDeckProperty
        {
            get { return Config.Instance.StatDeckTime; }
            set { Config.Instance.StatDeckTime = value; }
        }

        public TimeFrame TimeOverallProperty
        {
            get { return Config.Instance.StatOverallTime; }
            set { Config.Instance.StatOverallTime = value; }
        }

        public DeckVersion VersionDeckProperty
        {
            get { return Config.Instance.StatDeckVer; }
            set { Config.Instance.StatDeckVer = value; }
        }

        public OverallVersion VersionOverallProperty
        {
            get { return Config.Instance.StatOverallVer; }
            set { Config.Instance.StatOverallVer = value; }
        }

        public bool OutputDeckName
        {
            get { return Config.Instance.OutputDeckName; }
            set { Config.Instance.OutputDeckName = value; }
        }

        public bool OutputDeckStats
        {
            get { return Config.Instance.OutputDeckStats; }
            set { Config.Instance.OutputDeckStats = value; }
        }

        public bool OutputDeckCombined
        {
            get { return Config.Instance.OutputDeckCombined; }
            set { Config.Instance.OutputDeckCombined = value; }
        }

        public bool OutputOverall
        {
            get { return Config.Instance.OutputOverall; }
            set { Config.Instance.OutputOverall = value; }
        }

        public string OutputPath
        {
            get
            {
                return Config.Instance.OutputPath.Substring(0, 16) + "..." + Config.Instance.OutputPath.Substring((Config.Instance.OutputPath.Length - 16), 16);
            }
        }
    }
}
