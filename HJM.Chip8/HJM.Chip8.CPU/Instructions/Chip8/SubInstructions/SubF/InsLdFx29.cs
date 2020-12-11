using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions.Chip8.SubInstructions.SubF
{
    /// <summary>
    /// Fx29 - LD F, Vx
    /// Set I = location of sprite for digit Vx.
    /// The value of I is set to the location for the hexadecimal sprite corresponding to the value of Vx. See section 2.4, Display, for more information on the Chip-8 hexadecimal font.
    /// </summary>
    public class InsLdFx29 : Instruction
    {
        public override string Description { get; set; } = "Set I = location of sprite for digit Vx.";

        public override ushort OpCode { get; set; } = 0x0029;

        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);

            stateChange.IndexRegisterChange = new Change<ushort>()
            {
                NewValue = (ushort)(state.Registers[x] * 0x5),
            };

            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
