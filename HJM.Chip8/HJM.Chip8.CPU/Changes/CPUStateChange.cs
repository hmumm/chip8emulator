using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Changes
{
    /// <summary>
    /// Represents the changes in a CPUState
    /// </summary>
    public class CPUStateChange
    {
        public List<AddressChange<byte>> RegisterChanges { get; } = new List<AddressChange<byte>>();
        public List<AddressChange<byte>> MemoryChanges { get; } = new List<AddressChange<byte>>();
        public List<AddressChange<byte>> GraphicsChanges { get; } = new List<AddressChange<byte>>();
        public Change<ushort>? ProgramCounterChange { get; set; }
        public Change<ushort>? IndexRegisterChange { get; set; }
        public StackChange? StackChange { get; set; }
        public Change<byte>? SoundTimerChange { get; set; }
        public Change<byte>? DelayTimerChange { get; set; }

        /// <summary>
        /// Increments the program counter by 2 moving to the next instruction
        /// </summary>
        /// <param name="oldValue">Previous value of the program counter</param>
        public void IncrementProgramCounter(ushort oldValue)
        {
            ProgramCounterChange = new Change<ushort>
            {
                OldValue = oldValue,
                NewValue = (ushort)(oldValue + 2)
            };
        }

        /// <summary>
        /// Increments the program counter by 4 instead of 2, skipping the next instruction
        /// </summary>
        /// <param name="oldValue">Previous value of the program counter</param>
        public void SkipNextInstruction(ushort oldValue)
        {
            ProgramCounterChange = new Change<ushort>
            {
                OldValue = oldValue,
                NewValue = (ushort)(oldValue + 4)
            };
        }
    }
}
