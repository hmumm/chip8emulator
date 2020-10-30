﻿using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// 2nnn - CALL addr
    ///  Call subroutine at nnn.
    /// The interpreter increments the stack pointer, then puts the current PC on the top of the stack. The PC is then set to nnn.
    /// </summary>
    public class CALL_2nnn : Instruction
    {
        public override string Description { get; set; } = "Call subroutine at nnn.";

        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            ushort nnn = (ushort)(state.OpCode & 0x0FFF);

            stateChange.StackChange = new StackChange();

            stateChange.StackChange.AddressStackChange = new ArrayChange<ushort>
            {
                IndexChanged = state.StackPointer,
                NewValue = state.ProgramCounter
            };

            stateChange.StackChange.StackPointerChange = new Change<ushort>
            {
                NewValue = (ushort)(state.StackPointer + 1)
            };

            stateChange.ProgramCounterChange = new Change<ushort>
            {
                NewValue = nnn
            };

            return stateChange;
        }
    }
}
