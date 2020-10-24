using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// 6xkk - LD Vx, byte
    /// Set Vx = kk.
    /// The interpreter puts the value kk into register Vx.
    /// </summary>
    public class LD_6xkk : Instruction
    {
        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);
            byte kk = (byte)(state.OpCode & 0x00FF);

            AddressChange<byte> registerChange = new AddressChange<byte>()
            {
                AddressChanged = x,
                OldValue = state.Registers[x],
                NewValue = kk
            };

            stateChange.RegisterChanges.Add(registerChange);

            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
