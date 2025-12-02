using System;
using System.Collections.Generic;
using System.Text;

namespace ZConnect.Models
{
    public interface ICommunicationStatus
    {
        byte[]? Data { get; }
        Enum StatusType { get; }    // Use Enum as a general type.
    }
}
