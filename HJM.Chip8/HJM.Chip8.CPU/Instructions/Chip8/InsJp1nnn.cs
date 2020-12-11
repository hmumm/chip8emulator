using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions.Chip8
{
    /// <summary>
    /// 1nnn - JP addr
    /// Jump to location nnn.
    /// The interpreter sets the program counter to nnn.
    /// </summary>
    public class InsJp1nnn : Instruction
    {
        public override string Description { get; set; } = "Jump to location nnn.";

        public override ushort OpCode { get; set; } = 0x1000;

        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            ushort nnn = (ushort)(state.OpCode & 0x0FFF);

            stateChange.ProgramCounterChange = new Change<ushort>
            {
                NewValue = nnn,
            };

            return stateChange;
        }
    }
}
