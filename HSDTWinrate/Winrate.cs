using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;
using Hearthstone_Deck_Tracker.Stats;
using System.IO;


namespace HSDTWinrate
{
    /// <summary>
    /// class for the core methods
    /// </summary>

    static class Winrate
    {
        private const string _deckNameFile = "DeckName.txt";
        private const string _deckStatsFile = "DeckStats.txt";
        private const string _deckCombinedFile = "DeckWL.txt";
        private const string _overallStatsFile = "OverallWL.txt";

        /// <summary>
        /// Plugin initialization
        /// </summary>

        public static void Load()
        {
            RegisterMethods();
            RefreshStats();
        }

        /// <summary>
        /// Register stats calculation method to game events
        /// </summary>

        private static void RegisterMethods()
        {
            GameEvents.OnGameStart.Add(RefreshStats);
            GameEvents.OnInMenu.Add(RefreshStats);

            DeckManagerEvents.OnDeckSelected.Add(RefreshStats);
            DeckManagerEvents.OnDeckUpdated.Add(RefreshStats);
        }

        /// <summary>
        /// Ugly solution for the whole refresh process
        /// ## made ready for future ideas
        /// </summary>

        public static void RefreshStats()
        {
            RefreshStats(true);
        }

        private static void RefreshStats(Deck obj)
        {
            RefreshStats(true);
        }

        public static void RefreshStats(bool calculateOverallStats)
        {
            var deck = DeckList.Instance.ActiveDeck;            

            if (calculateOverallStats && Config.Instance.OutputOverall)
            {
                var defgames = DefaultDeckStats.Instance.DeckStats.SelectMany(x => x.Games).Where(TimeFrameFilter(Config.Instance.StatOverallTime)).Where(GameModeFilter(Config.Instance.StatOverallVer)).ToList();
                var games = DeckStatsList.Instance.DeckStats.SelectMany(ds => ds.Games).Where(TimeFrameFilter(Config.Instance.StatOverallTime)).Where(GameModeFilter(Config.Instance.StatOverallVer)).ToList();
                int total = games.Count() + defgames.Count();
                int wins = games.Where(wns => wns.Result == GameResult.Win).Count() + defgames.Where(wns => wns.Result == GameResult.Win).Count();                
                int winrate;

                if (total > 0)
                {
                    winrate = (int)Math.Round(100.0 * wins / total);
                }
                else
                {
                    winrate = 50;
                }

                string content = string.Format("{0} - {1} ({2} %)", wins, total - wins, winrate);
                updateFile(content, _overallStatsFile);
            }

            if (deck != null)
            {
                if (deck.IsArenaDeck)
                {

                    // build strings

                    string deckName;
                    string deckStats;
                    
                    deckName = "Arena: " + deck.Class;
                    deckStats = deck.WinLossString + "(" + deck.WinPercentString + ")";

                    // update files

                    if (Config.Instance.OutputDeckName)
                    {                        
                        updateFile(deckName, _deckNameFile);
                    }

                    if (Config.Instance.OutputDeckStats)
                    {                        
                        updateFile(deckStats, _deckStatsFile);
                    }

                    if (Config.Instance.OutputDeckCombined)
                    {
                        string[] lines = { deckName, deckStats };
                        updateFile(lines, _deckCombinedFile);
                    }
                }
                else
                {
                    // select relevant games

                    var games = DeckStatsList.Instance.DeckStats.SelectMany(ds => ds.Games).Where(dck => dck.DeckId == deck.DeckId).Where(rnk => rnk.GameMode == GameMode.Ranked).ToList();
                    if (Config.Instance.StatDeckVer == DeckVersion.current)
                    {
                        games = games.Where(ver => ver.PlayerDeckVersion == deck.SelectedVersion).Where(TimeFrameFilter(Config.Instance.StatDeckTime)).ToList();
                    }
                    else
                    {
                        games = games.Where(TimeFrameFilter(Config.Instance.StatDeckTime)).ToList();
                    }
                    
                    int wins = games.Where(x => x.Result == GameResult.Win).Count();
                    int total = games.Count();
                    int winrate;
                    if (total > 0)
                    {
                        winrate = (int)Math.Round(100.0 * wins / total);
                    }
                    else
                    {
                        winrate = 50;
                    }

                    // build strings

                    string deckName = deck.Name;
                    string deckStats = string.Format("{0} - {1} ({2} %)", wins, total - wins, winrate);

                    // update files

                    if (Config.Instance.OutputDeckName)
                    {
                        updateFile(deckName, _deckNameFile);
                    }

                    if (Config.Instance.OutputDeckStats)
                    {
                        updateFile(deckStats, _deckStatsFile);
                    }

                    if (Config.Instance.OutputDeckCombined)
                    {
                        string[] lines = { deckName, deckStats };
                        updateFile(lines, _deckCombinedFile);
                    }
                }                
            }
        }

        /// <summary>
        /// Selects the games played during a specific timeframe
        /// (Stolen from Epix'37 twitch chat plugin)
        /// </summary>

        public static Func<GameStats, bool> TimeFrameFilter(TimeFrame timeFrame)
        {
            switch (timeFrame)
            {
                case TimeFrame.today:
                    return game => game.StartTime.Date == DateTime.Today;
                case TimeFrame.week:
                    return game => game.StartTime > DateTime.Today.AddDays(-7);
                case TimeFrame.season:
                    return game => game.StartTime.Date.Year == DateTime.Today.Year && game.StartTime.Date.Month == DateTime.Today.Month;
                case TimeFrame.total:
                    return game => true;
                default:
                    return game => false;
            }
        }

        /// <summary>
        /// Selects the games played in a specific game mode
        /// </summary>

        public static Func<GameStats, bool> GameModeFilter(OverallVersion overallVersion)
        {
            switch (overallVersion)
            {
                case OverallVersion.arena:
                    return game => game.GameMode == GameMode.Arena;
                case OverallVersion.constructed:
                    return game => game.GameMode == GameMode.Ranked;
                case OverallVersion.both:
                    return game => (game.GameMode == GameMode.Ranked || game.GameMode == GameMode.Arena);
                // problematic, fix it! ## dirty fix, fix it again
                case OverallVersion.auto:
                    {
                        if (DeckList.Instance.ActiveDeck != null)
                        {
                            return game => game.GameMode == (DeckList.Instance.ActiveDeck.IsArenaDeck ? GameMode.Arena : GameMode.Ranked);
                        }
                        else
                        {
                            return game => false;
                        }
                    }                    
                default:
                    return game => false;
            }
        }

        /// <summary>
        /// Update a text file
        /// </summary>

        private static void updateFile(string[] content, string fileName)
        {
            try
            {
                System.IO.File.WriteAllLines(Path.Combine(Config.Instance.OutputPath, fileName), content);
            }
            catch (SystemException ex)
            {
                string line = "WinRatePlugin:UpdateFileMethod: " + ex.Message;
                Hearthstone_Deck_Tracker.Logger.WriteLine(line);
            }
        }

        private static void updateFile(string content, string fileName)
        {
            try
            {
                System.IO.File.WriteAllText(Path.Combine(Config.Instance.OutputPath, fileName), content);
            }
            catch (SystemException ex)
            {
                string line = "WinRatePlugin:UpdateFileMethod: " + ex.Message;
                Hearthstone_Deck_Tracker.Logger.WriteLine(line);
            }
        }
    }
}
