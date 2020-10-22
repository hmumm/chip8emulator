using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    public class SE_3xkk : Instruction
    {
        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            Change<ushort> programCounterCounter = new Change<ushort>();
            programCounterCounter.OldValue = state.ProgramCounter;

            if (state.Registers[(ushort)((state.OpCode & 0x0F00) >> 8)] == (ushort)(state.OpCode & 0x00FF))
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
