using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using System.IO;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.UI;

namespace GeoffsDeathCount
{
	class GeoffsDeathCount : Mod
	{
        public static GeoffsDeathCount Instance;
        internal DeathsUI DeathsUI;

        private UserInterface _userInterface;

		public GeoffsDeathCount()
		{
            Instance = this;
		}

        public override void Load()
        {
            DeathsUI = new DeathsUI();
            DeathsUI.Activate();

            _userInterface = new UserInterface();
            _userInterface.SetState(DeathsUI);
        }

        public override void Unload()
        {
            Instance = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if(DeathsUI.Visible)
            {
                //DeathsUI.HasDeathsChanged = true;
                _userInterface.Update(gameTime);
            }

        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "DeathsUI: Show Deaths",
                    delegate {
                        if (DeathsUI.Visible)
                        {
                            _userInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            DeathCountMessageType packetID = (DeathCountMessageType) reader.ReadByte();
            int playerID = reader.ReadInt32();
            switch (packetID)
            {
                case DeathCountMessageType.PlayerSync:
                    Main.player[playerID].GetModPlayer<DeathCountModPlayer>().playerDeathCount = reader.ReadInt32();
                    GeoffsDeathCount.Instance.DeathsUI.HasDeathsChanged = true;
                    break;
                case DeathCountMessageType.PlayerValueChange:
                    int deathCount = reader.ReadInt32();
                    Main.player[playerID].GetModPlayer<DeathCountModPlayer>().playerDeathCount = deathCount;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket pack = GetPacket();
                        pack.Write((byte)DeathCountMessageType.PlayerValueChange);
                        pack.Write(playerID);
                        pack.Write(deathCount);
                        pack.Send(-1, playerID);
                    }
                    GeoffsDeathCount.Instance.DeathsUI.HasDeathsChanged = true;
                    break;
            }
        }
    }

    internal enum DeathCountMessageType: byte
    {
        PlayerSync,
        PlayerValueChange,
    }
}
