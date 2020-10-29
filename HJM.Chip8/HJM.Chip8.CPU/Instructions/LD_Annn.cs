using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// Annn - LD I, addr
    ///  Set I = nnn.
    /// The value of register I is set to nnn.
    /// </summary>
    public class LD_Annn : Instruction
    {
        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            ushort nnn = (ushort)(state.OpCode & 0x0FFF);

            stateChange.IndexRegisterChange = new Change<ushort>()
            {
                NewValue = nnn
            };

            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
