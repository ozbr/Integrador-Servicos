using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Helper
{
    public enum FlowStatus
    {
        Failed = 0,
        Downloaded = 1,
        ProcessingQueue = 2,
        Processing = 3,
        Processed = 4,
        SendQueue = 5,
        Sending = 6,
        Sent = 7,
        OCR = 10,
        Stored = 11
    }
}
