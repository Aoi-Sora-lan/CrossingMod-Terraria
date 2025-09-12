using CrossGameLibrary.Net;
using CrossingMachine.Content.TileEntities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace CrossingMachine.Common.UI;

public class MachineUIState : UIState
{
    private MachineItemSlot[] _slots;
    public CrossingMachinePanel CrossingMachinePanel;
    private Item[] _openSlots;
    private MachineTileEntity _machineTileEntity;
    private UIButton _ioButton;
    private UIButton _channelButton;
    private UIText _nameText;
    public void BindTileEntity(MachineTileEntity machineTileEntity)
    {
        _machineTileEntity = machineTileEntity;
    }
    public void RefreshProgress(float progress)
    {
        CrossingMachinePanel.SetProgress(progress);
    }
    public void RefreshSlot(Item[] items)
    {
        var count = _slots.Length;
        for (var i = 0; i < count; i++)
        {
            if (_slots[i] == null) _openSlots = items;
            else _slots[i].SetSlots(items);
        }
    }

    public override void OnActivate()
    {
        base.OnActivate();
        RefreshUI();
    }
    
    public override void OnInitialize()
    {
        base.OnInitialize();
        Asset<Texture2D> uiTexture = ModContent.Request<Texture2D>($"{CrossingMachine.AssetPath}/Textures/UI/crossing_machine_gui");
        CrossingMachinePanel = new CrossingMachinePanel(uiTexture);
        CrossingMachinePanel.SetPadding(0);
        SetRectangle(CrossingMachinePanel, left: 600f, top: 100f, width: 410f, height: 205f);
        _slots = new MachineItemSlot[2];
        for (var i = 0; i < _slots.Length; i++)
        {
            _slots[i] = new MachineItemSlot( _openSlots, i, i != 0);
        }
        SetRectangle(_slots[0], left: 158, top: 20, width: 36f, height: 36f);
        SetRectangle(_slots[1], left: 158, top: 116f, width: 36f, height: 36f);
        CrossingMachinePanel.Append(_slots[0]);
        CrossingMachinePanel.Append(_slots[1]);
        // _nameText = new UIText("Default Machine");
        _channelButton = new UIButton();
        _ioButton = new UIButton();
        SetRectangle(_channelButton, left: 28f, top: 24f, 48f,48f);
        SetRectangle(_ioButton, left: 28f, top: 74f, 48f,48f);
        // SetRectangle(_nameText, left: 0f, top: 60f, 48f,48f);
        CrossingMachinePanel.Append(_channelButton);
        CrossingMachinePanel.Append(_ioButton);
        // CrossingMachinePanel.Append(_nameText);
        _channelButton.OnLeftClick += (_, __) =>
        {
            _machineTileEntity.ChangeChannel(1);
            RefreshUI();
        };
        _channelButton.OnRightClick += (_, __) =>
        {
            _machineTileEntity.ChangeChannel(-1);
            RefreshUI();
        };
        _ioButton.OnLeftClick += (_, __) =>
        {
            _machineTileEntity.SwitchIOType(1);
            RefreshUI();
        };
        _ioButton.OnRightClick += (_, __) =>
        {
            _machineTileEntity.SwitchIOType(-1);
            RefreshUI();
        };
        RefreshUI();
        Append(CrossingMachinePanel);
    }

    private void RefreshUI()
    {
        var type = _machineTileEntity.IOType switch
        {
            MachineIOType.Input => "输入",
            MachineIOType.Output => "输出",
            _ => "空"
        };
        _ioButton.Tooltip = $"模式：{type}";
        _channelButton.Tooltip = $"频道：{_machineTileEntity.Channel}";
        _slots[0].SetActivate(_machineTileEntity.IOType == MachineIOType.Input);
        // _nameText.SetText(_machineTileEntity.MachineName);
    }
    private void SetRectangle(UIElement uiElement, float left, float top, float width = -1, float height = -1) {
        uiElement.Left.Set(left, 0f);
        uiElement.Top.Set(top, 0f);
        if(width >= 0) uiElement.Width.Set(width, 0f);
        if(height >= 0) uiElement.Height.Set(height, 0f);
    }
}