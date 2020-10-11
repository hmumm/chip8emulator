using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Changes
{
    public class StackChange
    {
        AddressChange<byte> AddressStackChange { get; set; } = new AddressChange<byte>();
        Change<byte> StackPointerChange { get; set; } = new Change<byte>();
    }
}
