using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Changes
{
    public class CPUStateChange
    {
        List<AddressChange<byte>> RegisterChanges { get; } = new List<AddressChange<byte>>();
        List<AddressChange<byte>> MemoryChanges { get; } = new List<AddressChange<byte>>();
        List<AddressChange<byte>> GraphicsChanges { get; } = new List<AddressChange<byte>>();
        Change<ushort>? ProgramCounterChange { get; set; }
        Change<ushort>? IndexRegisterChange { get; set; }
        StackChange? StackChange { get; set; }
        Change<bool>? SoundFlagChange { get; set; }
    }
}
