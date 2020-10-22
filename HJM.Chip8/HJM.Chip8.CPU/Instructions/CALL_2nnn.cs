using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    public class CALL_2nnn : Instruction
    {
        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            StackChange stackChange = new StackChange();

            AddressChange<ushort> stackAddressChange = new AddressChange<ushort>();

            stackAddressChange.AddressChanged = state.StackPointer;
            stackAddressChange.OldValue = state.Stack[state.StackPointer];
            stackAddressChange.NewValue = state.ProgramCounter;

            stackChange.AddressStackChange = stackAddressChange;

            Change<ushort> stackPointerChange = new Change<ushort>();

            stackPointerChange.OldValue = state.StackPointer;
            stackPointerChange.NewValue = (ushort)(state.StackPointer + 1);

            stackChange.StackPointerChange = stackPointerChange;

            stateChange.StackChange = stackChange;

            Change<ushort> programCounterChange = new Change<ushort>();

            programCounterChange.OldValue = state.ProgramCounter;
            programCounterChange.NewValue = (ushort)(state.OpCode & 0x0FFF);

            stateChange.ProgramCounterChange = programCounterChange;

            return stateChange;
        }
    }
}
