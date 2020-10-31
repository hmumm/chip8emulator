using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions.Chip8.SubInstructions._F
{
    /// <summary>
    /// Fx33 - LD B, Vx
    /// Store BCD representation of Vx in memory locations I, I+1, and I+2.
    /// The interpreter takes the decimal value of Vx, and places the hundreds digit in memory at location in I, the tens digit at location I+1, and the ones digit at location I+2.
    /// </summary>
    public class LD_Fx33 : Instruction
    {
        public override string Description { get; set; } = "Store BCD representation of Vx in memory locations I, I+1, and I+2.";

        public override ushort OpCode { get; set; } = 0x0033;

        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);

            stateChange.MemoryChanges.Add(new ArrayChange<byte>()
            {
                IndexChanged = state.IndexRegister,
                NewValue = (byte)(state.Registers[x] / 100)
            });

            stateChange.MemoryChanges.Add(new ArrayChange<byte>()
            {
                IndexChanged = state.IndexRegister + 1,
                NewValue = (byte)(state.Registers[x] / 10 % 10)
            });

            stateChange.MemoryChanges.Add(new ArrayChange<byte>()
            {
                IndexChanged = state.IndexRegister + 2,
                NewValue = (byte)(state.Registers[x] % 100 % 10)
            });

            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
