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
        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            Change<ushort> programCounterChange = new Change<ushort>()
            {
                OldValue = state.ProgramCounter
            };

            if (state.Registers[(state.OpCode & 0x0F00) >> 8] == state.Registers[(state.OpCode & 0x00F0) >> 4])
            {
                programCounterChange.NewValue = (ushort)(state.ProgramCounter + 4);
            }
            else
            {
                programCounterChange.NewValue = (ushort)(state.ProgramCounter + 2);
            }

            stateChange.ProgramCounterChange = programCounterChange;

            return stateChange;
        }
    }
}
