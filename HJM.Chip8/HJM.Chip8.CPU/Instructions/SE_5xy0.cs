using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// 5xy0 - SE Vx, Vy
    /// Skip next instruction if Vx = Vy.
    /// The interpreter compares register Vx to register Vy, and if they are equal, increments the program counter by 2.
    /// </summary>
    public class SE_5xy0 : Instruction
    {
        public override string Description { get; set; } = "Skip next instruction if Vx = kk.";

        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);
            byte y = (byte)((state.OpCode & 0x00F0) >> 4);

            if (state.Registers[x] == state.Registers[y])
            {
                stateChange.SkipNextInstruction(state.ProgramCounter);
            }
            else
            {
                stateChange.IncrementProgramCounter(state.ProgramCounter);
            }

            return stateChange;
        }
    }
}
