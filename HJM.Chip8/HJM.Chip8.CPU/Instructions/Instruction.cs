using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    public abstract class Instruction
    {
        string Description { get; set; } = string.Empty;

        public abstract CPUStateChange Execute(in CPUState state);
    }
}
