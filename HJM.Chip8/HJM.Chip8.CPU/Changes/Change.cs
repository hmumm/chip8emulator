using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Changes
{
    public class Change<T>
    {
        public string Description { get; set; } = string.Empty;
        public T OldValue { get; set; }
        public T NewValue { get; set; }
    }
}
