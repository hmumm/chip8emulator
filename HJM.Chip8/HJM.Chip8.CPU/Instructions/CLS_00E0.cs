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
                AddressChange<byte> graphicsChange = new AddressChange<byte>();
                graphicsChange.AddressChanged = (byte)i;
                graphicsChange.OldValue = state.Graphics[i];
                graphicsChange.NewValue = 0;
                stateChange.GraphicsChanges.Add(graphicsChange);
            }

            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
