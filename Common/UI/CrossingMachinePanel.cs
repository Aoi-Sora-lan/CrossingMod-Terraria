using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.UI;

namespace CrossingMachine.Common.UI;

public class CrossingMachinePanel : UIElement
{
    private Asset<Texture2D> _texture;
    private float _progress = 0;

    public CrossingMachinePanel(Asset<Texture2D> texture)
    {
        _texture = texture;
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (ContainsPoint(Main.MouseScreen)) {
            Main.LocalPlayer.mouseInterface = true;
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (base.IsMouseHovering)
        {
            Main.LocalPlayer.mouseInterface = true;
        }
        var dimensions = GetDimensions();
        var point = new Point((int)dimensions.X, (int)dimensions.Y);
        var progressPoint = new Point(point.X + 170, point.Y + 60);
        var channelButtonPoint = new Point(point.X + 28, point.Y + 24);
        var ioButtonPoint = new Point(point.X + 28, point.Y + 74);
        DrawPanel(point, spriteBatch, _texture.Value);
        DrawProgress(progressPoint, spriteBatch, _texture.Value, _progress);
        DrawButton(channelButtonPoint, spriteBatch, _texture.Value,0);
        DrawButton(ioButtonPoint, spriteBatch, _texture.Value,1);
    }
    public void SetProgress(float progress)
    {
        if (progress > 1) progress = 1;
        _progress = progress;
    }
    private void DrawPanel(Point point, SpriteBatch spriteBatch, Texture2D texture)
    {
        spriteBatch.Draw(texture, new Vector2(point.X, point.Y),new Rectangle(0,0,351,195), Color.White);
    }
    private void DrawProgress(Point point, SpriteBatch spriteBatch, Texture2D texture, float progress)
    {
        spriteBatch.Draw(texture, new Vector2(point.X, point.Y),new Rectangle(352,0,16,(int)(52*progress)), Color.White);
    }
    private void DrawButton(Point point, SpriteBatch spriteBatch, Texture2D texture, int type)
    {
        spriteBatch.Draw(texture, new Vector2(point.X, point.Y),new Rectangle(384,0,40,44), Color.White);
        switch (type)
        {
            case 0:
                spriteBatch.Draw(texture, new Vector2(point.X+4, point.Y+4),new Rectangle(384,96,32,32), Color.White);
                break;
            case 1:
                spriteBatch.Draw(texture, new Vector2(point.X+4, point.Y+4),new Rectangle(416,96,32,32), Color.White);
                break;
        }
    }
    
}