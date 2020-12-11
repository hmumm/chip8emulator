using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions.Chip8
{
    /// <summary>
    /// Bnnn - JP V0, addr
    /// Jump to location nnn + V0.
    /// The program counter is set to nnn plus the value of V0.
    /// </summary>
    public class InsJpBnnn : Instruction
    {
        public override string Description { get; set; } = "Jump to location nnn + V0.";

        public override ushort OpCode { get; set; } = 0xB000;

        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            ushort nnn = (ushort)(state.OpCode & 0x0FFF);

            stateChange.ProgramCounterChange = new Change<ushort>()
            {
                NewValue = (ushort)(nnn + state.Registers[0]),
            };

            return stateChange;
        }
    }
}
