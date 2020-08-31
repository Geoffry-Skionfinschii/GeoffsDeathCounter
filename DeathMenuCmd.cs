using Terraria.ModLoader;

namespace GeoffsDeathCount
{
    class DeathMenuCmd : ModCommand
    {
        public override CommandType Type {
            get {
                return CommandType.Chat;
            }
        }

        public override string Command
        {
            get
            {
                return "deaths";
            }
        }
        public override string Description
        {
            get
            {
                return "Shows deaths";
            }
        }
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            DeathsUI.Visible = true;
        }
    }
}
