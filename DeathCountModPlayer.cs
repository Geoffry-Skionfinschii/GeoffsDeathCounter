using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace GeoffsDeathCount
{
    class DeathCountModPlayer : ModPlayer
    {
        public int playerDeathCount = -1;


        public override void Load(TagCompound tag)
        {
            playerDeathCount = tag.GetInt("deaths");
            GeoffsDeathCount.Instance.DeathsUI.HasDeathsChanged = true;
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            DelayedSync(toWho, fromWho, newPlayer);
        }

        private async void DelayedSync(int toWho, int fromWho, bool newPlayer)
        {
            await Task.Delay(2000);
            ModPacket pack = mod.GetPacket();
            pack.Write((byte)DeathCountMessageType.PlayerSync);
            pack.Write(player.whoAmI);
            pack.Write(playerDeathCount);
            pack.Send(toWho, fromWho);
        }

        /*public override void OnEnterWorld(Player player)
        {
            ModPacket pack = mod.GetPacket();
            pack.Write((byte)DeathCountMessageType.PlayerSync);
            pack.Write(this.player.whoAmI);
            pack.Write(playerDeathCount);
            pack.Send(-1, this.player.whoAmI);
        }*/

        public override TagCompound Save()
        {
            return new TagCompound
            {
                {"deaths", playerDeathCount}
            };
        }

        private double numUpdates = -600;

        public override void PreUpdate()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && numUpdates % 3600 == 0 && player.whoAmI == Main.LocalPlayer.whoAmI)
            {
                ModPacket pack = mod.GetPacket();
                pack.Write((byte)DeathCountMessageType.PlayerValueChange);
                pack.Write(player.whoAmI);
                pack.Write(playerDeathCount);
                pack.Send(-1, player.whoAmI);
            }
            numUpdates++;
            base.PreUpdate();
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (Main.LocalPlayer.whoAmI == player.whoAmI && Main.netMode == NetmodeID.MultiplayerClient)
            {
                playerDeathCount++;
                ModPacket pack = mod.GetPacket();
                pack.Write((byte)DeathCountMessageType.PlayerValueChange);
                pack.Write(player.whoAmI);
                pack.Write(playerDeathCount);
                pack.Send(-1, player.whoAmI);
            }
            if(Main.netMode == NetmodeID.SinglePlayer)
            {
                playerDeathCount++;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                GeoffsDeathCount.Instance.DeathsUI.HasDeathsChanged = true;

                int deaths = playerDeathCount;
                Color col = Colors.CoinGold;
                Main.NewText(player.name + " has died " + deaths + " time(s)", col);
            }
        }
    }
}
