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
    public BaseUdpClient Client;
    private const string GameType = "Terraria";
    private string _gameId;
    public void Init()
    {
        if(Main.netMode == NetmodeID.MultiplayerClient) return;
        var ins = ModContent.GetInstance<CrossMachineConfig>();
        Client = new BaseUdpClient(int.Parse(ins.CrossLocalPort), ins.CrossServerIP, int.Parse(ins.CrossServerPort));
        // _client = new BaseUdpClient(12001, "192.168.1.102", 11000);
        Client.Start();
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
        var machineEntity = Client.Register(address with { MachineId = GenerateShortGuidId() }, logic);
        return machineEntity;
    }
    public async void Close()
    {
        await Client.RemoveMachines();
        Client?.Dispose();
    }
    public static string GenerateShortGuidId()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("/", "_")
            .Replace("+", "-")
            .Substring(0, 10);
    }
}