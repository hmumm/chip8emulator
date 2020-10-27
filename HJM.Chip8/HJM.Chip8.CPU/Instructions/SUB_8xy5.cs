using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// 8xy5 - SUB Vx, Vy
    /// Set Vx = Vx - Vy, set VF = NOT borrow.
    /// If Vx > Vy, then VF is set to 1, otherwise 0. Then Vy is subtracted from Vx, and the results stored in Vx.
    /// </summary>
    public class SUB_8xy5 : Instruction
    {
        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);
            byte y = (byte)((state.OpCode & 0x00F0) >> 4);

            stateChange.RegisterChanges.Add(new AddressChange<byte>()
            {
                AddressChanged = x,
                OldValue = state.Registers[x],
                NewValue = (byte)(state.Registers[x] - state.Registers[y])
            });

            AddressChange<byte> carryChange = new AddressChange<byte>()
            {
                AddressChanged = 0xf,
                OldValue = state.Registers[0xf]
            };

            if (state.Registers[x] > state.Registers[y])
                carryChange.NewValue = 1; //carry
            else
                carryChange.NewValue = 0;

            stateChange.RegisterChanges.Add(carryChange);
            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
