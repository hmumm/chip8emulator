using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// 8xy7 - SUBN Vx, Vy
    /// Set Vx = Vy - Vx, set VF = NOT borrow.
    /// If Vy > Vx, then VF is set to 1, otherwise 0. Then Vx is subtracted from Vy, and the results stored in Vx.
    /// </summary>
    public class SUBN_8xy7 : Instruction
    {
        public override string Description { get; set; } = "Set Vx = Vy - Vx, set VF = NOT borrow.";

        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);
            byte y = (byte)((state.OpCode & 0x00F0) >> 4);

            stateChange.RegisterChanges.Add(new ArrayChange<byte>()
            {
                IndexChanged = x,
                NewValue = (byte)(state.Registers[y] - state.Registers[x])
            });

            ArrayChange<byte> carryChange = new ArrayChange<byte>()
            {
                IndexChanged = 0xf
            };

            if (state.Registers[y] > state.Registers[x])
                carryChange.NewValue = 1; //carry
            else
                carryChange.NewValue = 0;

            stateChange.RegisterChanges.Add(carryChange);
            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
