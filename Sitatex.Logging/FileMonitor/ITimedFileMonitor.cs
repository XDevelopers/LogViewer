using System;

namespace Sitatex.Logging.FileMonitor
{
    public interface ITimedFileMonitor : IFileMonitor
    {
        TimeSpan TimerInterval { get; set; }
    }
}
