using CrossingMachine.Common.UI;
using CrossingMachine.Content.TileEntities;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CrossingMachine.Content.Tiles;

public class CrossMachine : ModTile
{
    public override void SetStaticDefaults()
    {
        //Main.tileShine[Type] = 1100;
        Main.tileSolid[Type] = true;
        Main.tileNoAttach[Type] = true;

        Main.tileSolidTop[Type] = true;

        TileID.Sets.IsAContainer[Type] = true;
        Main.tileContainer[Type] = true;
        AdjTiles = [TileID.Containers];
        Main.tileFrameImportant[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = false;
        //TileID.Sets.BasicChest[Type] = true;
        TileObjectData.newTile.HookPostPlaceMyPlayer = ModContent.GetInstance<MachineTileEntity>().Generic_HookPostPlaceMyPlayer;
        TileObjectData.newTile.AnchorInvalidTiles = [
            TileID.MagicalIceBlock,
            TileID.Boulder,
            TileID.BouncyBoulder,
            TileID.LifeCrystalBoulder,
            TileID.RollingCactus
        ];
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.addTile(Type);
        //VanillaFallbackOnModDeletion = TileID.MetalBars;
        //AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.MetalBar"));
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        base.DrawEffects(i, j, spriteBatch, ref drawData);
    }

    public override void HitWire(int i, int j)
    {
        base.HitWire(i, j);
        if (TileObjectData.TopLeft(i, j) != new Point16(i, j)) return;
        if (!TileEntity.TryGet(i, j, out MachineTileEntity tileEntity)) return;
        tileEntity.UdpEntity.SendSetSignalMessage(tileEntity.Channel);
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        base.KillMultiTile(i, j, frameX, frameY);
        ModContent.GetInstance<MachineTileEntity>().Kill(i, j);
    }
    
    public override bool RightClick(int i, int j)
    {
        if (!TileEntity.TryGet(i, j, out MachineTileEntity tileEntity)) return true;
        tileEntity.BindUISystem(ModContent.GetInstance<MachineUISystem>());

        // if (tileEntity.HoldItem != null)
        // {
        //     Main.LocalPlayer.QuickSpawnItem(new EntitySource_TileEntity(tileEntity), tileEntity.HoldItem, tileEntity.HoldItem.stack);
        //     tileEntity.HoldItem = null;
        // }
        // else
        // {
        //     tileEntity.HoldItem = Main.LocalPlayer.HeldItem.Clone(); 
        //     Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem].TurnToAir(true);
        // }
      
        return true; 
    }

    public override void NearbyEffects(int i, int j, bool closer)
    {

    }
}