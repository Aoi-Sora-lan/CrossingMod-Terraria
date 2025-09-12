using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Terraria;
using Terraria.ModLoader;

namespace CrossingMachine.Common.LoggerSink;
public class TerrariaLogSink : ILogEventSink
{
    private readonly Mod _mod;
    private readonly IFormatProvider _formatProvider;

    public TerrariaLogSink(Mod mod, IFormatProvider formatProvider = null)
    {
        _mod = mod;
        _formatProvider = formatProvider;
    }

    public void Emit(LogEvent logEvent)
    {
        var message = logEvent.RenderMessage(_formatProvider);
        switch (logEvent.Level)
        {
            case LogEventLevel.Verbose:
            case LogEventLevel.Debug:
                _mod.Logger.Debug(message);
                break;
            case LogEventLevel.Information:
                _mod.Logger.Info(message);
                break;
            case LogEventLevel.Warning:
                _mod.Logger.Warn(message);
                break;
            case LogEventLevel.Error:
            case LogEventLevel.Fatal:
                Main.NewText($"[c/FF0000:{message}]");
                _mod.Logger.Error(message);
                if (logEvent.Exception != null)
                {
                    _mod.Logger.Error(logEvent.Exception.ToString());
                }
                break;
        }
    }
}
public static class TerrariaLogSinkExtensions
{
    public static LoggerConfiguration TerrariaLogSink(
        this LoggerSinkConfiguration loggerConfiguration,
        Mod mod,
        IFormatProvider formatProvider = null)
    {
        return loggerConfiguration.Sink(new TerrariaLogSink(mod, formatProvider));
    }
}