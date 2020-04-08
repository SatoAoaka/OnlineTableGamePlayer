using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineTableGamePlayer.Model
{
    class OpenSettingMenu
    {
        internal static void OpenSettingMenuWindow()
        {
            var window = new SettingWinodw();
            window.ShowDialog();
        }
    }
}
