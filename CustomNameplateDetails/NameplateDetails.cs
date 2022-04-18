using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using VRC;

namespace PendulumClient.CustomNameplateDetails
{
    class NameplateDetails : MonoBehaviour
    {
        public VRC.Player player;
        private byte fps;
        private byte ping;
        private int noFPSorPing = 0;
        private TextMeshProUGUI Textobj;
        private ImageThreeSlice nameplateBG;

        public NameplateDetails(IntPtr ptr) : base(ptr)
        {
        }

        void Start()
        {
            Transform stats = UnityEngine.Object.Instantiate<Transform>(this.gameObject.transform.Find("Contents/Quick Stats"), this.gameObject.transform.Find("Contents"));
            stats.parent = this.gameObject.transform.Find("Contents");
            stats.gameObject.SetActive(true);
            Textobj = stats.Find("Trust Text").GetComponent<TextMeshProUGUI>();
            Textobj.color = Color.white;
            stats.Find("Trust Icon").gameObject.SetActive(false);
            stats.Find("Performance Icon").gameObject.SetActive(false);
            stats.Find("Performance Text").gameObject.SetActive(false);
            stats.Find("Friend Anchor Stats").gameObject.SetActive(false);

            nameplateBG = this.gameObject.transform.Find("Contents/Main/Background").GetComponent<ImageThreeSlice>();

            nameplateBG._sprite = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Main/Glow").GetComponent<ImageThreeSlice>()._sprite;
            nameplateBG.color = Color.black;

            fps = player._playerNet.field_Private_Byte_0;
            ping = player._playerNet.field_Private_Byte_1;
        }

        void Update()
        {
            if (fps == player._playerNet.field_Private_Byte_0 && ping == player._playerNet.field_Private_Byte_1)
            {
                noFPSorPing++;
            }
            else
            {
                noFPSorPing = 0;
            }
            fps = player._playerNet.field_Private_Byte_0;
            ping = player._playerNet.field_Private_Byte_1;
            string text = "<color=green>Stable</color>";
            if (noFPSorPing > 30)
                text = "<color=yellow>Lagging</color>";
            if (noFPSorPing > 150)
                text = "<color=red>Crashed</color>";
            //statsText.text = $"[{player.GetPlatform()}] |" + $"{(player.GetIsMaster() ? " | [<color=#0352ff>HOST</color>] |" : "")}" + $" [{text}] |" + $" [FPS: {player.GetFramesColord()}] |" + $" [Ping: {player.GetPingColord()}] " + $" {(player.ClientDetect() ? " | [<color=red>ClientUser</color>]" : "")}";
        }
    }
}
