using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// Fx65 - LD Vx, [I]
    /// Read registers V0 through Vx from memory starting at location I.
    /// The interpreter reads values from memory starting at location I into registers V0 through Vx.
    /// </summary>
    public class LD_Fx65 : Instruction
    {
        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);

            for (int i = 0; i <= x; i++)
            {
                stateChange.RegisterChanges.Add(new AddressChange<byte>()
                {
                    AddressChanged = i,
                    OldValue = state.Registers[i],
                    NewValue = state.Memory[state.IndexRegister + 1]
                });
            }

            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
