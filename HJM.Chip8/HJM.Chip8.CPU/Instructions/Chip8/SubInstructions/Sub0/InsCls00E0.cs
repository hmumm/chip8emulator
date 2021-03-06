﻿using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions.Chip8.SubInstructions.Sub0
{
    /// <summary>
    /// 00E0 - CLS
    /// Clear the display.
    /// </summary>
    public class InsCls00E0 : Instruction
    {
        public override string Description { get; set; } = "Clear the display.";

        public override ushort OpCode { get; set; } = 0x00E0;

        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            for (int i = 0; i < 64 * 32; i++)
            {
                stateChange.GraphicsChanges.Add(new ArrayChange<byte>
                {
                    IndexChanged = i,
                    NewValue = 0,
                });
            }

            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
