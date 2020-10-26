﻿using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// 00EE - RET
    /// Return from a subroutine.
    /// The interpreter sets the program counter to the address at the top of the stack, then subtracts 1 from the stack pointer.
    /// </summary>
    public class RET_00EE : Instruction
    {
        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            stateChange.StackChange = new StackChange();

            stateChange.StackChange.StackPointerChange = new Change<ushort>
            {
                OldValue = state.StackPointer,
                NewValue = (ushort)(state.StackPointer - 1)
            };

            stateChange.ProgramCounterChange = new Change<ushort>
            {
                OldValue = state.ProgramCounter,
                NewValue = (ushort)(state.Stack[state.StackPointer - 1] + 2)
            };

            return stateChange;
        }
    }
}
