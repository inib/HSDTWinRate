using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDTWinrate
{

    /// <summary>
    /// HSDT plugin class implementation
    /// </summary>

    public class WinRatePlugin : Hearthstone_Deck_Tracker.Plugins.IPlugin
    {
        private SettingsWindow _settingsWindow;

        public string Author
        {
            get { return "alpine"; }
        }

        public string ButtonText
        {
            get { return "Setttings"; }
        }

        public string Description
        {
            get { return "Description"; }
        }

        public System.Windows.Controls.MenuItem MenuItem
        {
            get { return null; }
        }

        public string Name
        {
            get { return "WinRatePlugin"; }
        }

        public void OnButtonPress()
        {
            OpenSettings();
        }

        public void OnLoad()
        {
            Winrate.Load();
        }

        public void OnUnload()
        {
            
        }

        public void OnUpdate()
        {
            
        }

        public Version Version
        {
            get { return new System.Version(0,1); }
        }

        /// <summary>
        /// Open/close the settings dialog
        /// </summary>

        private void OpenSettings()
        {
            if (_settingsWindow == null)
            {
                _settingsWindow = new SettingsWindow();
                _settingsWindow.Closed += (sender1, args1) => { _settingsWindow = null; };
                _settingsWindow.Show();
            }
            else
                _settingsWindow.Activate();
        }
    }
}
