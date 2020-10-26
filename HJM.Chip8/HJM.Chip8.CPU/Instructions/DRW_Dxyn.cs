using HJM.Chip8.CPU.Changes;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace HJM.Chip8.CPU.Instructions
{
    /// <summary>
    /// Dxyn - DRW Vx, Vy, nibble
    /// Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.
    /// The interpreter reads n bytes from memory, starting at the address stored in I. These bytes are then displayed as sprites on screen at coordinates (Vx, Vy). 
    /// Sprites are XORed onto the existing screen. If this causes any pixels to be erased, VF is set to 1, otherwise it is set to 0. 
    /// If the sprite is positioned so part of it is outside the coordinates of the display, it wraps around to the opposite side of the screen. 
    /// See instruction 8xy3 for more information on XOR, and section 2.4, Display, for more information on the Chip-8 screen and sprites.
    /// </summary>
    public class DRW_Dxyn : Instruction
    {
        public override CPUStateChange Execute(in CPUState state)
        {
            CPUStateChange stateChange = new CPUStateChange();

            ushort x = state.Registers[(state.OpCode & 0x0F00) >> 8];
            ushort y = state.Registers[(state.OpCode & 0x00F0) >> 4];
            ushort height = (ushort)(state.OpCode & 0x000F);
            ushort pixel;

            AddressChange<byte> collisionChange = new AddressChange<byte>()
            {
                AddressChanged = 0xF,
                OldValue = state.Registers[0xF],
                NewValue = 0x0
            };

            for (int yline = 0; yline < height; yline++)
            {
                pixel = state.Memory[state.IndexRegister + yline];
                for (int xline = 0; xline < 8; xline++)
                {
                    if ((pixel & (0x80 >> xline)) != 0)
                    {
                        int currentAddress = x + xline + ((y + yline) * 64);

                        if (state.Graphics[currentAddress] == 1)
                            collisionChange.NewValue = 0x1;

                        stateChange.GraphicsChanges.Add(new AddressChange<byte>()
                        {
                            AddressChanged = currentAddress,
                            OldValue = state.Graphics[currentAddress],
                            NewValue = (byte)(state.Graphics[currentAddress] ^ 1)
                        });
                    }
                }
            }

            stateChange.RegisterChanges.Add(collisionChange);
            stateChange.IncrementProgramCounter(state.ProgramCounter);

            return stateChange;
        }
    }
}
