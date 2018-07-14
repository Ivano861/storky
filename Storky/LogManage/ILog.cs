using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storky.LogManage
{
    internal interface ILog
    {
        void Write(string message, EventLogEntryType entryType = EventLogEntryType.Information, int id = 0);
    }
}
