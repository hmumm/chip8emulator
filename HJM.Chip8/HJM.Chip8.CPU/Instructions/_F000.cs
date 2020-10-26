using HJM.Chip8.CPU.Changes;
using HJM.Chip8.CPU.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// Call the correct instruction that starts with F in hex
    /// </summary>
    public class _F000 : Instruction
    {
        private Dictionary<int, Instruction> Instructions = new Dictionary<int, Instruction>();

        public _F000()
        {
            Instructions.Add(0x0007, new LD_Fx07());
            Instructions.Add(0x000A, new LD_Fx0A());
            Instructions.Add(0x0015, new LD_Fx15());
            Instructions.Add(0x0018, new LD_Fx18());
            Instructions.Add(0x001E, new ADD_Fx1E());
            Instructions.Add(0x0029, new LD_Fx29());
            Instructions.Add(0x0033, new LD_Fx33());
            Instructions.Add(0x0055, new LD_Fx55());
            Instructions.Add(0x0065, new LD_Fx65());
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
