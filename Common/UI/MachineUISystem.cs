using System.Collections.Generic;
using CrossingMachine.Content.TileEntities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace CrossingMachine.Common.UI;

[Autoload(Side = ModSide.Client)]
public class MachineUISystem : ModSystem
{
    private MachineUIState _machineUIState;
    private UserInterface _menuBar;
    private MachineTileEntity _bindingTileEntity;
    public override void Load()
    {
        if (Main.dedServ) return;
        _bindingTileEntity = null;
        On_Player.ToggleInv += (orig, self) =>
        {
            if (Main.playerInventory && _menuBar.CurrentState != null) HideUI();
            orig(self);
        };
        _menuBar = new UserInterface();
        _machineUIState = new MachineUIState();
    }

    public override void OnWorldUnload()
    {
        HideUI();
        base.OnWorldUnload();
    }


    public void SetProgress(float progress)
    {
        _machineUIState.RefreshProgress(progress);
    }
    
    public void SetSlot(Item[] slots)
    {
        _machineUIState.RefreshSlot(slots);
    }

    public void ShowUI()
    {
        if(_menuBar?.CurrentState != null) HideUI();
        _menuBar?.SetState(_machineUIState);
        if(!Main.playerInventory) Main.playerInventory = true;
    }

    public void HideUI()
    {
        _menuBar?.SetState(null);
    }

    private GameTime _lastUpdateUiGameTime;
    public override void UpdateUI(GameTime gameTime)
    {
        _lastUpdateUiGameTime = gameTime;
        if (_menuBar?.CurrentState != null) {
            _menuBar.Update(gameTime);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "MyMod: MyInterface",
                delegate
                {
                    if (_lastUpdateUiGameTime != null && _menuBar?.CurrentState != null)
                    {
                        _menuBar.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                    }
                    return true;
                },
                InterfaceScaleType.UI));
        }
    }

    public void BindTileEntity(MachineTileEntity machineTileEntity)
    {
        if(machineTileEntity != _bindingTileEntity) _bindingTileEntity?.UnBind();
        _bindingTileEntity = machineTileEntity;
        _machineUIState.BindTileEntity(machineTileEntity);
    }
}