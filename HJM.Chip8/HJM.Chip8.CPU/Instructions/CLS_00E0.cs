using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// 00E0 - CLS
    /// Clear the display.
    /// </summary>
    public class CLS_00E0 : Instruction
    {
        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            for (int i = 0; i < 64 * 32; i++)
            {
                stateChange.GraphicsChanges.Add(new AddressChange<byte>
                {
                    AddressChanged = (byte)i,
                    OldValue = state.Graphics[i],
                    NewValue = 0
                });
            }

            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
