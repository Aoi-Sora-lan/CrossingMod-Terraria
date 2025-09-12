using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossingMachine.Common.LoggerSink;
using Serilog;
using Serilog.Formatting.Json;
using Terraria.ModLoader;

namespace CrossingMachine
{
	public class CrossingMachine : Mod
	{
		public override void Load()
		{
			base.Load();
			Log.Logger = new LoggerConfiguration()
				.WriteTo.TerrariaLogSink(this)
				.CreateLogger();
			Log.Information("Log配置完成");
			Logger.Info("Terraria Logger 正常工作");
		}

		public override void Unload()
		{
			base.Unload();
			Log.CloseAndFlush();
		}

		public const string AssetPath = $"{nameof(CrossingMachine)}/Assets";
	}
}
