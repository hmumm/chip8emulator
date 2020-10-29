using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// 7xkk - ADD Vx, byte
    /// Set Vx = Vx + kk.
    /// Adds the value kk to the value of register Vx, then stores the result in Vx.
    /// </summary>
    public class ADD_7xkk : Instruction
    {
        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);
            byte kk = (byte)(state.OpCode & 0x00FF);

            stateChange.RegisterChanges.Add(new AddressChange<byte>()
            {
                AddressChanged = x,
                NewValue = (byte)(state.Registers[x] + kk)
            });

            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
