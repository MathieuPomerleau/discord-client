using Discord;
using Microsoft.Extensions.Logging;

namespace Injhinuity.Client.Discord.Converters
{
    public interface ILogSeverityConverter
    {
        LogSeverity FromLogLevel(LogLevel logLevel);
    }

    public class LogSeverityConverter : ILogSeverityConverter
    {
        public LogSeverity FromLogLevel(LogLevel logLevel) =>
            logLevel switch
            {
                LogLevel.Debug    => LogSeverity.Debug,
                LogLevel.Error    => LogSeverity.Error,
                LogLevel.Critical => LogSeverity.Critical,
                _                 => LogSeverity.Info
            };
    }
}
