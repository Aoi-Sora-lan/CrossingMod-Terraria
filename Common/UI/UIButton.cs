using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.UI;

namespace CrossingMachine.Common.UI;

public class UIButton : UIElement
{
    private float _visibilityActive = 1f;
    private float _visibilityInactive = 0.4f;

    public string Tooltip = "Defaut"; 
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (IsMouseHovering)
        {
            Main.instance.MouseText(Tooltip);
        }
    }
}