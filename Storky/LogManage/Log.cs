using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storky.LogManage
{
    internal sealed class Log : ILog
    {
        private const string LogName = "Storky";
        private const string Source = "Storky";

        public const int IdNormal = 1;
        public const int IdDebug = 16;

        private bool _exist = false;

        private Log()
        {
            try
            {
                if (!EventLog.SourceExists(Source))
                    EventLog.CreateEventSource(Source, LogName);

                _exist = EventLog.SourceExists("Storky");
            }
            catch (Exception)
            {
                _exist = false;
            }
        }

        internal static Log Instance { get; } = new Log();

        /// <summary>
        /// Writes a message to the log.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="entryType">Type of message</param>
        /// <param name="id">Id of message.</param>
        public void Write(string message, EventLogEntryType entryType = EventLogEntryType.Information, int id = IdNormal)
        {
            if (!_exist)
                return;

            // Write the start messagge in the log
            using (EventLog eventLog = new EventLog())
            {
                eventLog.Source = Source;
                eventLog.WriteEntry(message, EventLogEntryType.Information, id);
            }
        }

        /// <summary>
        /// Clear all message from log.
        /// </summary>
        /// <remarks>
        /// Requires administrative rights.
        /// </remarks>
        public void Clear()
        {
            if (!_exist)
                return;

            try
            {
                // Clear all message from log
                using (EventLog eventLog = new EventLog())
                {
                    eventLog.Source = Source;
                    eventLog.Clear();
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// Delete the log.
        /// </summary>
        /// <remarks>
        /// Requires administrative rights.
        /// </remarks>
        public void Delete()
        {
            try
            {
                // Delete the log
                EventLog.Delete(LogName);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
