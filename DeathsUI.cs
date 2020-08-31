using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace GeoffsDeathCount
{
    internal class DeathsUI : UIState
    {

        public DragUIPanel MainPanel;
        public static bool Visible = true;
        public bool HasDeathsChanged = false;

        public override void OnInitialize()
        {
            MainPanel = new DragUIPanel();
            MainPanel.SetPadding(0);

            MainPanel.Left.Set(400f, 0f);
            MainPanel.Top.Set(100f, 0f);
            MainPanel.Width.Set(220f, 0f);
            MainPanel.Height.Set(70f, 0f);
            MainPanel.BackgroundColor = new Color(73, 94, 171);

            /*UIList list = new UIList();
            list.Left.Set(0, 0f);
            list.Top.Set(0, 0f);
            list.Width.Set(100, 0);
            list.Height.Set(123, 0f);*/

            MainPanel.OnRightClick += MainPanel_OnRightClick;
            Append(MainPanel);
        }

        public override void Update(GameTime gameTime)
        {
            if(HasDeathsChanged)
            {
                MainPanel.RemoveAllChildren();

                UIText title = new UIText("Player Deaths");
                title.HAlign = UIAlign.Center;
                title.Left.Set(0, 0);
                title.Top.Set(10, 0f);
                MainPanel.Append(title);

                int listNum = 1;
                foreach(Player pl in Main.player)
                {
                    if(pl.active)
                    {
                        int deaths = pl.GetModPlayer<DeathCountModPlayer>().playerDeathCount;

                        string playerName = pl.name;
                        //if(playerName.Length > 15)
                        //{
                        //    playerName = playerName.Substring(0, 12);
                        //    playerName += "...";
                        //}

                        UIText name = new UIText(playerName, 0.8f);
                        name.HAlign = UIAlign.Left;
                        name.Left.Set(10, 0f);
                        name.Top.Set(10 + (26 * listNum), 0f);

                        string deathNum = deaths + " time(s)";

                        if(deaths < 0)
                        {
                            deathNum = "Unsynced";
                        }
                        UIText num = new UIText(deathNum, 0.8f);
                        num.HAlign = UIAlign.Right;
                        num.Left.Set(-10, 0);
                        num.Top.Set(10 + (26 * listNum), 0f);

                        MainPanel.Append(name);
                        MainPanel.Append(num);
                        listNum++;
                    }
                }

                MainPanel.Height.Set(20 + 26 * listNum, 0f);

                HasDeathsChanged = false;
            }
            base.Update(gameTime);
        }

        private void MainPanel_OnRightClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Visible = false;
        }
    }
}
