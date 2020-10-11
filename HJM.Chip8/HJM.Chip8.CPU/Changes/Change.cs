using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Changes
{
    public class Change<T>
    {
        string? Description { get; set; }
        T OldValue { get; set; }
        T NewValue { get; set; }
    }
}
