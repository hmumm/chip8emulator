using HJM.Chip8.CPU.Changes;
using HJM.Chip8.CPU.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// Call the correct instruction that starts with 8 in hex
    /// </summary>
    public class _8000 : Instruction
    {
        private Dictionary<int, Instruction> Instructions = new Dictionary<int, Instruction>();
        public _8000()
        {
            Instructions.Add(0x0000, new LD_8xy0());
            Instructions.Add(0x0001, new OR_8xy1());
            Instructions.Add(0x0002, new AND_8xy2());
            Instructions.Add(0x0003, new XOR_8xy3());
            Instructions.Add(0x0004, new ADD_8xy4());
            Instructions.Add(0x0005, new SUB_8xy5());
            Instructions.Add(0x0006, new SHR_8xy6());
            Instructions.Add(0x0007, new SUBN_8xy7());
            Instructions.Add(0x000E, new SHL_8xyE());
        }

        public override CPUStateChange Execute(in CPUState state)
        {
            Instruction? instruction = Instructions.GetValueOrDefault(state.OpCode & 0x000F);

            if (instruction == null)
            {
                throw new InvalidOpCodeException(state.OpCode);
            }

            return instruction.Execute(state);
        }
    }
}
