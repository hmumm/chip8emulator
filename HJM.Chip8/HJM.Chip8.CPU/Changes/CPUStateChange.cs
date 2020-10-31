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
        public List<ArrayChange<byte>> RegisterChanges { get; } = new List<ArrayChange<byte>>();
        public List<ArrayChange<byte>> MemoryChanges { get; } = new List<ArrayChange<byte>>();
        public List<ArrayChange<byte>> GraphicsChanges { get; } = new List<ArrayChange<byte>>();
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
                NewValue = (ushort)(oldValue + 4)
            };
        }
    }
}
