using HJM.Chip8.CPU.Changes;
using HJM.Chip8.CPU.Exceptions;
using HJM.Chip8.CPU.Instructions.Chip8.SubInstructions._8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HJM.Chip8.CPU.Instructions.Chip8
{
    /// <summary>
    /// Call the correct instruction that starts with 8 in hex
    /// </summary>
    public class _8000 : Instruction
    {
        private Dictionary<ushort, Instruction> Instructions = new Dictionary<ushort, Instruction>();

        public override string Description { get; set; } = "Call the 0x8000 instruction";

        public override ushort OpCode { get; set; } = 0x8000;

        public _8000()
        {
            // Get all types that are Instructions under HJM.Chip8.CPU.Instructions.Chip8.SubInstructions._8 namespace.
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(Instruction).IsAssignableFrom(p) && p.Namespace == "HJM.Chip8.CPU.Instructions.Chip8.SubInstructions._8");

            // Make instances of these types and add to the Instruction dictionary.
            foreach (Type type in types)
            {
                Instruction? instruction = (Instruction?)Activator.CreateInstance(type);

                if (instruction == null)
                {
                    throw new NullReferenceException("Instruction instance was null");
                }

                Instructions.Add(instruction.OpCode, instruction);
            }
        }

        public override CPUStateChange Execute(in CPUState state)
        {
            Instruction? instruction = Instructions.GetValueOrDefault((ushort)(state.OpCode & 0x000F));

            if (instruction == null)
            {
                throw new InvalidOpCodeException(state.OpCode);
            }

            Description = instruction.Description;

            return instruction.Execute(state);
        }
    }
}
