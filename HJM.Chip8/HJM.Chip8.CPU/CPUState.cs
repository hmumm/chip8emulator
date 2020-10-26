using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU
{
    public class CPUState
    {
        /// <summary>
        /// Whether sound should be played or not 
        /// </summary>
        public bool SoundFlag { get; set; }
        public ushort OpCode { get; set; }
        /// <summary>
        /// 0x000-0x1FF - Chip 8 interpreter(contains font set in emu)
        /// 0x050-0x0A0 - Used for the built in 4x5 pixel font set(0-F)
        /// 0x200-0xFFF - Program ROM and work RAM
        /// </summary>
        public byte[] Memory { get; set; } = new byte[4096];
        public byte[] Registers { get; set; } = new byte[16];
        public ushort IndexRegister { get; set; }
        public ushort ProgramCounter { get; set; }
        public byte[] Graphics { get; set; } = new byte[64 * 32];
        public byte DelayTimer { get; set; }
        public byte SoundTimer { get; set; }
        public ushort[] Stack { get; set; } = new ushort[16];
        public ushort StackPointer { get; set; }
        public byte[] Key { get; set; } = new byte[16];

        /// <summary>
        /// Apply the given state change
        /// </summary>
        /// <param name="stateChange">state change to apply</param>
        public void ApplyStateChange(CPUStateChange stateChange)
        {
            foreach (AddressChange<byte> registerChange in stateChange.RegisterChanges)
            {
                Registers[registerChange.AddressChanged] = registerChange.NewValue;
            }

            foreach (AddressChange<byte> memoryChange in stateChange.MemoryChanges)
            {
                Memory[memoryChange.AddressChanged] = memoryChange.NewValue;
            }

            foreach (AddressChange<byte> graphicChange in stateChange.GraphicsChanges)
            {
                Graphics[graphicChange.AddressChanged] = graphicChange.NewValue;
            }

            if (stateChange.ProgramCounterChange != null)
            {
                ProgramCounter = stateChange.ProgramCounterChange.NewValue;
            }

            if (stateChange.IndexRegisterChange != null)
            {
                IndexRegister = stateChange.IndexRegisterChange.NewValue;
            }

            if (stateChange.StackChange != null)
            {
                if (stateChange.StackChange.StackPointerChange != null)
                {
                    StackPointer = stateChange.StackChange.StackPointerChange.NewValue;
                }

                if (stateChange.StackChange.AddressStackChange != null)
                {
                    Stack[stateChange.StackChange.AddressStackChange.AddressChanged] = stateChange.StackChange.AddressStackChange.NewValue;
                }
            }

            if (stateChange.SoundTimerChange != null)
            {
                SoundTimer = stateChange.SoundTimerChange.NewValue;
            }

            if (stateChange.DelayTimerChange != null)
            {
                DelayTimer = stateChange.DelayTimerChange.NewValue;
            }
        }

        /// <summary>
        /// Revert the given state change
        /// </summary>
        /// <param name="stateChange">State change to revert</param>
        public void RevertStateChange(CPUStateChange stateChange)
        {
            foreach (AddressChange<byte> registerChange in stateChange.RegisterChanges)
            {
                Registers[registerChange.AddressChanged] = registerChange.OldValue;
            }

            foreach (AddressChange<byte> memoryChange in stateChange.MemoryChanges)
            {
                Memory[memoryChange.AddressChanged] = memoryChange.OldValue;
            }

            foreach (AddressChange<byte> graphicChange in stateChange.GraphicsChanges)
            {
                Graphics[graphicChange.AddressChanged] = graphicChange.OldValue;
            }

            if (stateChange.ProgramCounterChange != null)
            {
                ProgramCounter = stateChange.ProgramCounterChange.OldValue;
            }

            if (stateChange.IndexRegisterChange != null)
            {
                IndexRegister = stateChange.IndexRegisterChange.OldValue;
            }

            if (stateChange.StackChange != null)
            {
                if (stateChange.StackChange.StackPointerChange != null)
                {
                    StackPointer = stateChange.StackChange.StackPointerChange.OldValue;
                }

                if (stateChange.StackChange.AddressStackChange != null)
                {
                    Stack[stateChange.StackChange.AddressStackChange.AddressChanged] = stateChange.StackChange.AddressStackChange.OldValue;
                }
            }

            if (stateChange.SoundTimerChange != null)
            {
                SoundTimer = stateChange.SoundTimerChange.OldValue;
            }

            if (stateChange.DelayTimerChange != null)
            {
                DelayTimer = stateChange.DelayTimerChange.OldValue;
            }
        }
    }
}
