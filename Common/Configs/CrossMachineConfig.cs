using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace CrossingMachine.Common.Configs;

public class CrossMachineConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;
    [Header("General")]
    [DefaultValue("127.0.0.1")]
    [ReloadRequired]
    public string CrossServerIP;
    [DefaultValue("12000")]
    [ReloadRequired]
    public string CrossServerPort;
    [DefaultValue("12001")]
    [ReloadRequired]
    public string CrossLocalPort;
}