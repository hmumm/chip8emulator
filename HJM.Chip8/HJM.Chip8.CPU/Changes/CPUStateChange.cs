using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Changes
{
    public class CPUStateChange
    {
        public List<AddressChange<byte>> RegisterChanges { get; } = new List<AddressChange<byte>>();
        public List<AddressChange<byte>> MemoryChanges { get; } = new List<AddressChange<byte>>();
        public List<AddressChange<byte>> GraphicsChanges { get; } = new List<AddressChange<byte>>();
        public Change<ushort>? ProgramCounterChange { get; set; }
        public Change<ushort>? IndexRegisterChange { get; set; }
        public StackChange? StackChange { get; set; }
        public Change<bool>? SoundFlagChange { get; set; }

        public void IncrementProgramCounter(ushort oldValue)
        {
            Change<ushort> programCounterChange = new Change<ushort>();
            programCounterChange.OldValue = oldValue;
            programCounterChange.NewValue = (ushort)(oldValue + 2);
            ProgramCounterChange = programCounterChange;
        }
    }
}
