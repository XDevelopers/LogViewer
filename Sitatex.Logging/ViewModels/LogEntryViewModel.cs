using System;
using System.Linq;

namespace Sitatex.Logging.ViewModels
{
    public class LogEntryViewModel
    {
        const char Separator = '§';
        const char MessageSeparator = '¦';

        public LogEntryViewModel(string logEntry)
        {
            // Template
            //2018-11-07 10:48:58,595 | [1] | ERROR | Sitatex.Logging.App - Button: Log Error was clicked!
            //System.Exception: Exception created dynamically!
            if(logEntry.IndexOf(Separator) < 0)
            {
                EntryException = logEntry;
                return;
            }

            var slices = logEntry.Split(Separator);
            if (slices == null || slices.Any() == false) return;

            for (int i = 0; i < slices.Length; i++)
            {
                var slice = slices[i].Trim();
                if (i == 0) // Date
                {
                    EntryDate = Convert.ToDateTime(slice);
                }

                if (i == 2) // LogType
                {
                    EntryType = (LogType)Enum.Parse(typeof(LogType), slice);
                }

                if (i == 3) // EntryMessage
                {
                    // We need to apply a new split if there is any excetion or some like that! :)
                    var msg = slice.Split(MessageSeparator);
                    if (msg == null || msg.Any() == false)
                    {
                        EntryMessage = slice;
                    }
                    else
                    {
                        Namespace = msg[0];
                        EntryMessage = msg[1];
                    }
                }
            }
        }

        public DateTime EntryDate { get; set; }

        public LogType EntryType { get; set; }

        public string EntryMessage { get; set; }

        public string Namespace { get; set; }

        public string EntryException { get; set; }
    }

    public enum LogType
    {
        INFO,
        DEBUG,
        WARN,
        ERROR,
        FATAL
    }
}
