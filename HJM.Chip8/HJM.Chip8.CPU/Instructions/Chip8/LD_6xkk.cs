using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions.Chip8
{
    /// <summary>
    /// 6xkk - LD Vx, byte
    /// Set Vx = kk.
    /// The interpreter puts the value kk into register Vx.
    /// </summary>
    public class LD_6xkk : Instruction
    {
        public override string Description { get; set; } = "Set Vx = kk.";

        public override ushort OpCode { get; set; } = 0x6000;

        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte x = (byte)((state.OpCode & 0x0F00) >> 8);
            byte kk = (byte)(state.OpCode & 0x00FF);

            ArrayChange<byte> registerChange = new ArrayChange<byte>()
            {
                IndexChanged = x,
                NewValue = kk
            };

            stateChange.RegisterChanges.Add(registerChange);

            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
