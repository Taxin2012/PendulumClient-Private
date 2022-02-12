using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PendulumClient.QMLogAndPlayerlist
{
    class QM_PlayerListV2
    {
        public static VRC_PLScrollRect PlayerList;
        public static VRC_DebugScrollRect DebugLog;
        public static void OnUI()
        {
            PlayerList = new VRC_PLScrollRect("Right_PL_Closed", "PC_RightWingPlayerList", 2222, 0/*609.902f, 457.9203f*/, GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Right/Button").transform);
            DebugLog = new VRC_DebugScrollRect("Left_DL_Closed", "PC_LeftWingDebugLog", 1522, 0/*609.902f, 457.9203f*/, GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left/Button").transform);
        }
    }
}
