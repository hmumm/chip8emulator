﻿using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// ExA1 - SKNP Vx
    /// Skip next instruction if key with the value of Vx is not pressed.
    /// Checks the keyboard, and if the key corresponding to the value of Vx is currently in the up position, PC is increased by 2.
    /// </summary>
    public class SKNP_ExA1 : Instruction
    {
        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);

            if (state.Key[state.Registers[x]] == 0)
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
