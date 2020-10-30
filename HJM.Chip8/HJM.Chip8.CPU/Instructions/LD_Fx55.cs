using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// Fx55 - LD [I], Vx
    /// Store registers V0 through Vx in memory starting at location I.
    /// The interpreter copies the values of registers V0 through Vx into memory, starting at the address in I.
    /// </summary>
    public class LD_Fx55 : Instruction
    {
        public override string Description { get; set; } = "Store registers V0 through Vx in memory starting at location I.";

        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);

            for (int i = 0; i <= x; i++)
            {
                stateChange.MemoryChanges.Add(new ArrayChange<byte>()
                {
                    IndexChanged = state.IndexRegister + i,
                    NewValue = state.Registers[i]
                });
            }

            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
