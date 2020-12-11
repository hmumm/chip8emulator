using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions.Chip8.SubInstructions.SubF
{
    /// <summary>
    /// Fx1E - ADD I, Vx
    /// Set I = I + Vx.
    /// The values of I and Vx are added, and the results are stored in I.
    /// </summary>
    public class InsAddFx1E : Instruction
    {
        public override string Description { get; set; } = "Set I = I + Vx.";

        public override ushort OpCode { get; set; } = 0x001E;

        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);

            stateChange.IndexRegisterChange = new Change<ushort>()
            {
                NewValue = (ushort)(state.Registers[x] + state.IndexRegister),
            };

            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
