using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU
{
    public class CPUState
    {
        /// <summary>
        /// Whether sound should be played or not.
        /// </summary>
        public bool SoundFlag { get; set; }

        public ushort OpCode { get; set; }

        /// <summary>
        /// 0x000-0x1FF - Chip 8 interpreter(contains font set in emu)
        /// 0x050-0x0A0 - Used for the built in 4x5 pixel font set(0-F)
        /// 0x200-0xFFF - Program ROM and work RAM.
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
        /// Apply the given state change.
        /// </summary>
        /// <param name="stateChange">state change to apply.</param>
        public void ApplyStateChange(CPUStateChange stateChange)
        {
            ApplyRegisterChanges(stateChange);

            ApplyMemoryChanges(stateChange);

            ApplyGraphicsChanges(stateChange);

            ApplyProgramRegisterChange(stateChange);

            ApplyIndexRegisterChange(stateChange);

            ApplyStackChange(stateChange);

            ApplySoundTimerChange(stateChange);

            ApplyDelayTimerChange(stateChange);
        }

        private void ApplyRegisterChanges(CPUStateChange stateChange)
        {
            foreach (ArrayChange<byte> registerChange in stateChange.RegisterChanges)
            {
                Registers[registerChange.IndexChanged] = registerChange.NewValue;
            }
        }

        private void ApplyMemoryChanges(CPUStateChange stateChange)
        {
            foreach (ArrayChange<byte> memoryChange in stateChange.MemoryChanges)
            {
                Memory[memoryChange.IndexChanged] = memoryChange.NewValue;
            }
        }

        private void ApplyGraphicsChanges(CPUStateChange stateChange)
        {
            foreach (ArrayChange<byte> graphicChange in stateChange.GraphicsChanges)
            {
                Graphics[graphicChange.IndexChanged] = graphicChange.NewValue;
            }
        }

        private void ApplyProgramRegisterChange(CPUStateChange stateChange)
        {
            if (stateChange.ProgramCounterChange != null)
            {
                ProgramCounter = stateChange.ProgramCounterChange.NewValue;
            }
        }

        private void ApplyIndexRegisterChange(CPUStateChange stateChange)
        {
            if (stateChange.IndexRegisterChange != null)
            {
                IndexRegister = stateChange.IndexRegisterChange.NewValue;
            }
        }

        private void ApplyStackChange(CPUStateChange stateChange)
        {
            if (stateChange.StackChange != null)
            {
                if (stateChange.StackChange.StackPointerChange != null)
                {
                    StackPointer = stateChange.StackChange.StackPointerChange.NewValue;
                }

                if (stateChange.StackChange.AddressStackChange != null)
                {
                    Stack[stateChange.StackChange.AddressStackChange.IndexChanged] = stateChange.StackChange.AddressStackChange.NewValue;
                }
            }
        }

        private void ApplySoundTimerChange(CPUStateChange stateChange)
        {
            if (stateChange.SoundTimerChange != null)
            {
                SoundTimer = stateChange.SoundTimerChange.NewValue;
            }
        }

        private void ApplyDelayTimerChange(CPUStateChange stateChange)
        {
            if (stateChange.DelayTimerChange != null)
            {
                DelayTimer = stateChange.DelayTimerChange.NewValue;
            }
        }
    }
}
