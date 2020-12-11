using HJM.Chip8.CPU.Changes;
using HJM.Chip8.CPU.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HJM.Chip8.CPU.Instructions.Chip8
{
    /// <summary>
    /// Call the correct instruction that starts with F in hex.
    /// </summary>
    public class InsF000 : Instruction
    {
        private readonly Dictionary<ushort, Instruction> instructions = new Dictionary<ushort, Instruction>();

        public override string Description { get; set; } = "Call the 0xF000 instruction";

        public override ushort OpCode { get; set; } = 0xF000;

        public InsF000()
        {
            // Get all types that are Instructions under HJM.Chip8.CPU.Instructions.Chip8.SubInstructions._F namespace.
            IEnumerable<Type>? types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(Instruction).IsAssignableFrom(p) && p.Namespace == "HJM.Chip8.CPU.Instructions.Chip8.SubInstructions.SubF");

            // Make instances of these types and add to the Instruction dictionary.
            foreach (Type type in types)
            {
                Instruction? instruction = (Instruction?)Activator.CreateInstance(type);

                if (instruction == null)
                {
                    throw new NullReferenceException("Instruction instance was null");
                }

                instructions.Add(instruction.OpCode, instruction);
            }
        }

        public override CPUStateChange Execute(in CPUState state)
        {
            Instruction? instruction = instructions.GetValueOrDefault((ushort)(state.OpCode & 0x00FF));

            if (instruction == null)
            {
                throw new InvalidOpCodeException(state.OpCode);
            }

            Description = instruction.Description;

            return instruction.Execute(state);
        }
    }
}
