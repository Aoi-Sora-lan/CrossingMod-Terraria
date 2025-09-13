#nullable enable
using System;
using System.Linq;
using System.Threading.Tasks;
using CrossGameLibrary.Base;
using CrossGameLibrary.Message;
using CrossGameLibrary.Net;
using CrossingMachine.Common.LoggerSink;
using CrossingMachine.Common.UI;
using CrossingMachine.Common.Util;
using CrossingMachine.Content.Tiles;
using Serilog;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CrossingMachine.Content.TileEntities;

public class MachineTileEntity : ModTileEntity, IMachineLogic
{
    // 机器序列化属性
    public int Channel;
    public MachineIOType IOType;
    public string MachineName = "Default Machine";

    private const int SLOT_COUNT = 2;
    private Item _tempSendItem = new();
    public MachineEntity UdpEntity = null!;
    private MachineUISystem? _uiSystem;
    private readonly float _totalProgress = 100f;
    public float NowProgress;
    public Item?[] HoldItems = new Item[SLOT_COUNT];
    public override bool IsTileValidForEntity(int x, int y)
    {
        var tile = Main.tile[x, y];
        return tile.HasTile && tile.TileType == ModContent.TileType<CrossMachine>();
    }
    public override void LoadData(TagCompound tag)
    { 
        Channel = tag.GetInt(nameof(Channel));
        IOType = (MachineIOType)tag.GetInt(nameof(IOType));
        MachineName = tag.GetString(nameof(MachineName));
        if(MachineName == string.Empty) MachineName = "Default Machine";
        NowProgress = tag.GetFloat(nameof(NowProgress));
        HoldItems = tag.GetList<TagCompound>(nameof(HoldItems)).Select(ItemIO.Load).ToArray();
        for (var i = 0; i < SLOT_COUNT; i++)
        {
            HoldItems[i] ??= new Item();
        }
    }
    public override void SaveData(TagCompound tag)
    {
        var tags = HoldItems.Select(ItemIO.Save).ToList();
        tag[nameof(HoldItems)] = tags;
        tag[nameof(NowProgress)] = NowProgress;
        tag[nameof(Channel)] = Channel;
        tag[nameof(IOType)] = (int)IOType;
        tag[nameof(MachineName)] = MachineName;
    }
    
    private void TryRequest()
    {
        var itemId = ItemIdLookup.GetNameById((short)HoldItems[0].type);
        if(itemId == null) return;
        UdpEntity.SendItemRequestMessage(Channel, itemId, HoldItems[0].stack);
    }

    private void Start()
    {
        HoldItems[0] ??= new Item();
        HoldItems[1] ??= new Item();
        UdpEntity = UdpSystem.Instance.Register(this);
        Task.Run(async () =>
        {
            await UdpEntity.SendRegisterMachineMessage();
            await UdpEntity.SendSetChannelMessage(Channel, IOType);
        });
    }

    public override void OnKill()
    {
        base.OnKill();
        var point = Position;
        var i = point.X;
        var j = point.Y;
        if (HoldItems[0] != null)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j),
                i * 16, j * 16,
                16, 16,
                HoldItems[0].type,
                HoldItems[0].stack);
        }
        if (HoldItems[1] != null)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j),
                i * 16, j * 16,
                16, 16,
                HoldItems[1].type,
                HoldItems[1].stack);
        }
        UdpSystem.Instance.Client.RemoveMachine(UdpEntity);
    }

    private bool _firstUpdate = true;
    public override void Update()
    {
        if (_firstUpdate)
        {
            Start();
            _firstUpdate = false;
        }
        if (NowProgress<_totalProgress) {
            if(!_tempSendItem.IsAir || !HoldItems[1].IsAir || HoldItems[0].IsAir)
            {
                NowProgress = 0;
                _uiSystem?.SetProgress(NowProgress/_totalProgress);
                return;
            }
            NowProgress += 1f;
            _uiSystem?.SetProgress(NowProgress/_totalProgress);
        }
        else
        {
            TryRequest();
            NowProgress = 0;
            _uiSystem?.SetProgress(NowProgress/_totalProgress);
        }
    }
    private int times = 0;
    private bool _isLocked = false;
    public void BindUISystem(MachineUISystem uiSystem)
    {
        _uiSystem = uiSystem;
        _uiSystem.BindTileEntity(this);
        _uiSystem.ShowUI();
        _uiSystem.SetSlot(HoldItems);
    }

    public void ChangeMachineName()
    {
        UdpEntity.SendChangeMachineNameMessage("", Channel);
    }
    
    public void UnBind()
    {
        _uiSystem = null;
    }

    public bool CanTransfer(string itemId, int itemCount)
    {
        if (HoldItems[1].IsAir) return true;
        var item = ItemIdLookup.GetIdByName(itemId);
        if(item == null) return false;
        var idDiff = item != HoldItems[1].type;
        if (idDiff) return false;
        var transCount = Math.Max(0, 64 - HoldItems[1].stack);
        return transCount > 0;
    }

    public int GetMaxNeedCount()
    {
        return Math.Max(0, 64 - HoldItems[1].stack);
    }

    public void PreSend()
    {
        if (!BaseUdpClient.JudgeOnline()) return;
        _isLocked = true;
        _tempSendItem = HoldItems[0].Clone();
        HoldItems[0].TurnToAir();
    }

    public void SendSuccess(ItemResponse contentValue)
    {
        var left = _tempSendItem.stack - contentValue.ItemCount;
        if (left > 0)
        {
            HoldItems[1] = new Item(_tempSendItem.type, left);
        }
        _tempSendItem.TurnToAir();
    }

    public void SendFailure()
    {
        _isLocked = false;
        HoldItems[1] = _tempSendItem.Clone();
        _tempSendItem.TurnToAir();
    }

    public void GenerateItem(ItemPackage package)
    {
        var itemId = ItemIdLookup.GetIdByName(package.ItemId);
        if (!HoldItems[1].IsAir && HoldItems[1].type == (int)(itemId!.Value))
        {
            HoldItems[1].stack += package.ItemCount;
        }
        else {
            HoldItems[1] = new Item((int)itemId!, package.ItemCount);
        }
    }

    public void OnSignal()
    {
        if (IOType == MachineIOType.Output)
        {
            Wiring.TripWire(Position.X, Position.Y,1,1);
        }
    }
    
    public void ChangeChannel(int delta)
    {
        if (!BaseUdpClient.JudgeOnline()) return;
        Channel += delta;
        Task.Run(SetChannel);
    }
    public void SwitchIOType(int delta)
    {
        if (!BaseUdpClient.JudgeOnline()) return;
        IOType = (MachineIOType)(((int)IOType + delta)%3);
        Task.Run(SetChannel);
    }
    private async Task SetChannel()
    {
        await UdpEntity.SendSetChannelMessage(Channel, IOType);
    }
}