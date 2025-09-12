using System;
using System.Threading.Tasks;
using CrossGameLibrary.Base;
using CrossGameLibrary.Net;
using CrossingMachine.Common.Configs;
using Serilog;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CrossingMachine.Common.Util;

public class UdpSystem : ModSystem
{
    public static UdpSystem Instance;
    private BaseUdpClient _client;
    private const string GameType = "Terraria";
    private string _gameId;
    public void Init()
    {
        if(Main.netMode == NetmodeID.MultiplayerClient) return;
        var ins = ModContent.GetInstance<CrossMachineConfig>();
        _client = new BaseUdpClient(int.Parse(ins.CrossLocalPort), ins.CrossServerIP, int.Parse(ins.CrossServerPort));
        // _client = new BaseUdpClient(12001, "192.168.1.102", 11000);
        _client.Start();
        _gameId = GenerateShortGuidId();
    }

    public override void OnWorldLoad()
    {
        Instance = this;
        base.OnWorldLoad();
        Init();
    }

    public override void OnWorldUnload()
    {
        Instance = null;
        base.OnWorldUnload();
        Close();
    }

    public MachineEntity Register(IMachineLogic logic)
    {
        var address = new MachineAddress()
        {
            GameId = _gameId,
            GameType = GameType,
        };
        var machineEntity = _client.Register(address with { MachineId = GenerateShortGuidId() }, logic);
        return machineEntity;
    }
    public async void Close()
    {
        await _client.RemoveMachines();
        _client?.Dispose();
    }
    public static string GenerateShortGuidId()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("/", "_")
            .Replace("+", "-")
            .Substring(0, 10);
    }
}