using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions.Chip8.SubInstructions.SubF
{
    /// <summary>
    /// Fx18 - LD ST, Vx
    /// Set sound timer = Vx.
    /// ST is set equal to the value of Vx.
    /// </summary>
    public class InsLdFx18 : Instruction
    {
        public override string Description { get; set; } = "Set sound timer = Vx.";

        public override ushort OpCode { get; set; } = 0x0018;

        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);

            stateChange.SoundTimerChange = new Change<byte>()
            {
                NewValue = state.Registers[x],
            };

            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
