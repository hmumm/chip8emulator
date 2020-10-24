using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// 3xkk - SE Vx, byte
    /// Skip next instruction if Vx = kk.
    /// The interpreter compares register Vx to kk, and if they are equal, increments the program counter by 2.
    /// </summary>
    public class SE_3xkk : Instruction
    {
        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);
            byte kk = (byte)(state.OpCode & 0x00FF);

            Change<ushort> programCounterCounter = new Change<ushort>
            {
                OldValue = state.ProgramCounter
            };

            if (state.Registers[x] == kk)
            {
                programCounterCounter.NewValue = (ushort)(state.ProgramCounter + 4);
            }
            else
            {
                programCounterCounter.NewValue = (ushort)(state.ProgramCounter + 2);
            }

            stateChange.ProgramCounterChange = programCounterCounter;

            return stateChange;
        }
    }
}
