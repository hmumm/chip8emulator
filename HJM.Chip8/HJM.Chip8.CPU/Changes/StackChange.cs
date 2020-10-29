using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Changes
{
    /// <summary>
    /// Represents a change to the stack
    /// </summary>
    public class StackChange
    {
        public ArrayChange<ushort>? AddressStackChange { get; set; }
        public Change<ushort>? StackPointerChange { get; set; }
    }
}
