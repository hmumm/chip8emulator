using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions.Chip8.SubInstructions.Sub8
{
    /// <summary>
    /// 8xy1 - OR Vx, Vy
    /// Set Vx = Vx OR Vy.
    /// Performs a bitwise OR on the values of Vx and Vy, then stores the result in Vx.
    /// A bitwise OR compares the corrseponding bits from two values, and if either bit is 1, then the same bit in the result is also 1. Otherwise, it is 0.
    /// </summary>
    public class InsOr8xy1 : Instruction
    {
        public override string Description { get; set; } = "Set Vx = Vx OR Vy.";

        public override ushort OpCode { get; set; } = 0x0001;

        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);
            byte y = (byte)((state.OpCode & 0x00F0) >> 4);

            stateChange.RegisterChanges.Add(new ArrayChange<byte>()
            {
                IndexChanged = x,
                NewValue = (byte)(state.Registers[x] | state.Registers[y]),
            });

            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
