using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// Bnnn - JP V0, addr
    /// Jump to location nnn + V0.
    /// The program counter is set to nnn plus the value of V0.
    /// </summary>
    public class JP_Bnnn : Instruction
    {
        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            ushort nnn = (ushort)(state.OpCode & 0x0FFF);

            stateChange.ProgramCounterChange = new Change<ushort>()
            {
                OldValue = state.ProgramCounter,
                NewValue = (ushort)(nnn + state.Registers[0])
            };

            return stateChange;
        }
    }
}
