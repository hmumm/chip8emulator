using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    public class LD_6xkk : Instruction
    {
        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            byte address = (byte)((state.OpCode & 0x0F00) >> 8);

            AddressChange<byte> registerChange = new AddressChange<byte>()
            {
                AddressChanged = address,
                OldValue = state.Registers[address],
                NewValue = (byte)(state.OpCode & 0x00FF)
            };

            stateChange.RegisterChanges.Add(registerChange);
            
            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
