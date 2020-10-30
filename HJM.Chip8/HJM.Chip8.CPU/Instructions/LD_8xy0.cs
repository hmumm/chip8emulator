using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// 8xy0 - LD Vx, Vy
    /// Set Vx = Vy.
    /// Stores the value of register Vy in register Vx.
    /// </summary>
    public class LD_8xy0 : Instruction
    {
        public override string Description { get; set; } = "Set Vx = Vy.";

        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);
            byte y = (byte)((state.OpCode & 0x00F0) >> 4);

            stateChange.RegisterChanges.Add(new ArrayChange<byte>()
            {
                IndexChanged = x,
                NewValue = (byte)(state.Registers[y])
            });

            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
