using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions.Chip8.SubInstructions.SubF
{
    /// <summary>
    /// Fx65 - LD Vx, [I]
    /// Read registers V0 through Vx from memory starting at location I.
    /// The interpreter reads values from memory starting at location I into registers V0 through Vx.
    /// </summary>
    public class InsLdFx65 : Instruction
    {
        public override string Description { get; set; } = "Read registers V0 through Vx from memory starting at location I.";

        public override ushort OpCode { get; set; } = 0x0065;

        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);

            for (int i = 0; i <= x; i++)
            {
                stateChange.RegisterChanges.Add(new ArrayChange<byte>()
                {
                    IndexChanged = i,
                    NewValue = state.Memory[state.IndexRegister + i],
                });
            }

            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
