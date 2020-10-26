using HJM.Chip8.CPU.Changes;
using HJM.Chip8.CPU.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// Call the correct instruction that starts with E in hex
    /// </summary>
    public class _E000 : Instruction
    {
        private Dictionary<int, Instruction> Instructions = new Dictionary<int, Instruction>();

        public _E000()
        {
            Instructions.Add(0x009E, new SKP_Ex9E());
            Instructions.Add(0x00A1, new SKNP_ExA1());
        }

        public override CPUStateChange Execute(in CPUState state)
        {
            Instruction? instruction = Instructions.GetValueOrDefault(state.OpCode & 0x00FF);

            if (instruction == null)
            {
                throw new InvalidOpCodeException(state.OpCode);
            }

            return instruction.Execute(state);
        }
    }
}
