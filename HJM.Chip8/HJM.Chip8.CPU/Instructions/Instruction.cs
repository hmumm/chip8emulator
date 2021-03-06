﻿using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// Implement one instruction from the instruction set
    /// </summary>
    public abstract class Instruction
    {
        /// <summary>
        /// Description of what the instruction does
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        /// OpCode used to look up this instruction
        /// </summary>
        public abstract ushort OpCode { get; set; }

        /// <summary>
        /// Use the current state to execute the instruction and return the neccessary changes
        /// </summary>
        /// <param name="state">The current state of the cpu</param>
        /// <returns>What changes need to be made to the state</returns>
        public abstract CPUStateChange Execute(in CPUState state);
    }
}
