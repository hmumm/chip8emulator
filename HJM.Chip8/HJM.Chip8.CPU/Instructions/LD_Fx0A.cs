using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// Fx0A - LD Vx, K
    /// Wait for a key press, store the value of the key in Vx.
    /// All execution stops until a key is pressed, then the value of that key is stored in Vx.
    /// </summary>
    public class LD_Fx0A : Instruction
    {
        public override string Description { get; set; } = "Wait for a key press, store the value of the key in Vx.";

        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);

            // loop through the keys pressed, if one is pressed capture the first one
            // probably not the best way to do this, can't think of a better way right now
            int keyValue = -1;
            for (int i = 0; i < 16; i++)
            {
                if (state.Key[i] > 0)
                {
                    keyValue = i;
                    break;
                }
            }

            if (keyValue > -1)
            {
                stateChange.RegisterChanges.Add(new ArrayChange<byte>()
                {
                    IndexChanged = x,
                    NewValue = (byte)keyValue
                });

                stateChange.IncrementProgramCounter(state.ProgramCounter);
            }

            // if no value is found, don't update the programCounter i.e. continue executing this instruction
            return stateChange;
        }
    }
}
