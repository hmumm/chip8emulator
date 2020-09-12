using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU
{
    /// <summary>
    /// Emulates Chip8 Instructions.
    /// Does not do anything with graphics.
    /// </summary>
    public class Chip8
    {
        public bool DrawFlag { get; set; }
        private ushort opCode;
        //0x000-0x1FF - Chip 8 interpreter(contains font set in emu)
        //0x050-0x0A0 - Used for the built in 4x5 pixel font set(0-F)
        //0x200-0xFFF - Program ROM and work RAM
        private byte[] memory = new byte[4096];
        private byte[] registers = new byte[16];
        private ushort indexRegister;
        private ushort programCounter;
        private byte[] graphics = new byte[64 * 32];
        private byte delayTimer;
        private byte soundTimer;
        private ushort[] stack = new ushort[16];
        private ushort stackPointer;
        private byte[] key = new byte[16];

        private byte[] fontSet =
        {
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        };

        /// <summary>
        /// Initalizes memory for emulation.
        /// </summary>
        public void Initalize()
        {
            programCounter = 0x200;
            opCode = 0;
            indexRegister = 0;
            stackPointer = 0;
            stack = new ushort[16];
            registers = new byte[16];
            memory = new byte[4096];

            // Load the font set
            for (int i = 0; i < 80; i++)
                memory[i] = fontSet[i];

            delayTimer = 0;
            soundTimer = 0;
        }

        /// <summary>
        /// Loads the game with the specified name
        /// </summary>
        /// <param name="Game">Which game to run</param>
        public void LoadGame(String Game)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Emulates one Chip8 instruction cycle
        /// </summary>
        public void EmulateCycle()
        {
            // Fetch Opcode
            // Convert 2 1 byte memory addresses to 1 2 byte op code
            opCode = (ushort)(memory[programCounter] << 8 | memory[programCounter + 1]);

            // Decode Opcode
            switch (opCode & 0xF000)
            {
                case 0x0000:
                    switch (opCode & 0x000F)
                    {
                        case 0x00E0: //  00E0 - CLS
                                     //  Clear the display.
                            graphics = new byte[64 * 32];
                            programCounter += 2;
                            break;

                        case 0x000E: // 00EE - RET
                                     // Return from a subroutine.
                                     // The interpreter sets the program counter to the address at the top of the stack, then subtracts 1 from the stack pointer.
                            programCounter = stack[stackPointer];
                            stackPointer -= 1;
                            break;

                        default:
                            throw new Exception("Unknown opcode [0x0000]: 0x" + opCode);
                    }
                    break;

                case 0x1000: // 1nnn - JP addr
                             // Jump to location nnn.
                             // The interpreter sets the program counter to nnn.
                    programCounter = (ushort)(opCode & 0x0FFF);
                    break;

                case 0x2000: // 2nnn - CALL addr
                             //  Call subroutine at nnn.
                             // The interpreter increments the stack pointer, then puts the current PC on the top of the stack. The PC is then set to nnn.
                    stack[stackPointer] = programCounter;
                    stackPointer++;
                    programCounter = (ushort)(opCode & 0x0FFF);
                    break;
                case 0x3000: // 3xkk - SE Vx, byte
                             // Skip next instruction if Vx = kk.
                             // The interpreter compares register Vx to kk, and if they are equal, increments the program counter by 2.
                    if (registers[(ushort)((opCode & 0x0F00) >> 8)] == (ushort)(opCode & 0x00FF))
                    {
                        programCounter += 2;
                    } else
                    {
                        programCounter += 2;
                    }
                    break;

                case 0x4000: // 4xkk - SNE Vx, byte
                             // Skip next instruction if Vx != kk.
                             // The interpreter compares register Vx to kk, and if they are not equal, increments the program counter by 2.
                    if (registers[(ushort)((opCode & 0x0F00) >> 8)] != (ushort)(opCode & 0x00FF))
                    {
                        programCounter += 2;
                    }
                    else
                    {
                        programCounter += 2;
                    }
                    break;

                case 0x5000: // 5xy0 - SE Vx, Vy
                             // Skip next instruction if Vx = Vy.
                             // The interpreter compares register Vx to register Vy, and if they are equal, increments the program counter by 2.
                    if (registers[(opCode & 0x0F00) >> 8] == registers[(opCode & 0x00F0) >> 4])
                    {
                        programCounter += 2;
                    }
                    else
                    {
                        programCounter += 2;
                    }
                    break;
                case 0x6000: // 6xkk - LD Vx, byte
                             // Set Vx = kk.
                             // The interpreter puts the value kk into register Vx.
                    registers[(opCode & 0x0F00) >> 8] = (byte)(opCode & 0x00FF);
                    programCounter += 2;
                    break;

                case 0x7000: // 7xkk - ADD Vx, byte
                             // Set Vx = Vx + kk.
                             // Adds the value kk to the value of register Vx, then stores the result in Vx.
                    registers[(opCode & 0x0F00) >> 8] = (byte)(registers[(opCode & 0x0F00)] + (opCode & 0x00FF));
                    programCounter += 2;
                    break;

                case 0x8000:
                    switch (opCode & 0x000F)
                    {
                        case 0x0000: // 8xy0 - LD Vx, Vy
                                     // Set Vx = Vy.
                                     // Stores the value of register Vy in register Vx.
                            registers[(opCode & 0x0F00) >> 8] = registers[(opCode & 0x00F0) >> 4];
                            programCounter += 2;
                            break;

                        case 0x0001: // 8xy1 - OR Vx, Vy
                                     // Set Vx = Vx OR Vy.
                                     //  Performs a bitwise OR on the values of Vx and Vy, then stores the result in Vx. 
                                     // A bitwise OR compares the corrseponding bits from two values, and if either bit is 1, then the same bit in the result is also 1. Otherwise, it is 0. 
                            registers[(opCode & 0x0F00) >> 8] = (byte)(registers[(opCode & 0x0F00) >> 8] | registers[(opCode & 0x00F0) >> 4]);
                            programCounter += 2;
                            break;

                        case 0x0002: // 8xy2 - AND Vx, Vy
                                     // Set Vx = Vx AND Vy.
                                     // Performs a bitwise AND on the values of Vx and Vy, then stores the result in Vx. 
                                     // A bitwise AND compares the corrseponding bits from two values, and if both bits are 1, then the same bit in the result is also 1. Otherwise, it is 0. 
                            registers[(opCode & 0x0F00) >> 8] = (byte)(registers[(opCode & 0x0F00) >> 8] & registers[(opCode & 0x00F0) >> 4]);
                            programCounter += 2;
                            break;

                        case 0x0003: // 8xy3 - XOR Vx, Vy
                                     // Set Vx = Vx XOR Vy.
                                     // Performs a bitwise exclusive OR on the values of Vx and Vy, then stores the result in Vx. 
                                     // An exclusive OR compares the corrseponding bits from two values, and if the bits are not both the same, then the corresponding bit in the result is set to 1. Otherwise, it is 0. 
                            registers[(opCode & 0x0F00) >> 8] = (byte)(registers[(opCode & 0x0F00) >> 8] ^ registers[(opCode & 0x00F0) >> 4]);
                            programCounter += 2;
                            break;

                        case 0x0004: // 8xy4 - ADD Vx, Vy
                                     // Set Vx = Vx + Vy, set VF = carry.
                                     //  The values of Vx and Vy are added together. If the result is greater than 8 bits (i.e., > 255,) VF is set to 1, otherwise 0. Only the lowest 8 bits of the result are kept, and stored in Vx.
                            if (registers[(opCode & 0x00F0) >> 4] > (0xFF - registers[(opCode & 0x0F00) >> 8]))
                                registers[0xF] = 1; //carry
                            else
                                registers[0xF] = 0;

                            registers[(opCode & 0x0F00) >> 8] += registers[(opCode & 0x00F0) >> 4];
                            programCounter += 2;
                            break;

                        case 0x0005: // 8xy5 - SUB Vx, Vy
                                     // Set Vx = Vx - Vy, set VF = NOT borrow.
                                     // If Vx > Vy, then VF is set to 1, otherwise 0. Then Vy is subtracted from Vx, and the results stored in Vx.
                            if (registers[(opCode & 0x0F00) >> 8] > registers[(opCode & 0x00F0) >> 4])
                            {
                                registers[0xF] = 1;
                            } else
                            {
                                registers[0xF] = 0;
                            }

                            registers[(opCode & 0x0F00) >> 8] = (byte)(registers[(opCode & 0x0F00) >> 8] - registers[(opCode & 0x00F0) >> 4]);
                            programCounter += 2;
                            break;

                        case 0x0006: // 8xy6 - SHR Vx {, Vy}
                                     // Set Vx = Vx SHR 1.
                                     //  If the least-significant bit of Vx is 1, then VF is set to 1, otherwise 0. Then Vx is divided by 2.
                            registers[0xF] = (byte)(registers[(opCode & 0x0F00) >> 8] & 0x1); // if the last bit of Vx is one set VF to 1, otherwise set to 0
                            registers[(opCode & 0x0F00) >> 8] = (byte)(registers[(opCode & 0x0F00) >> 8] / 2);
                            programCounter += 2;
                            break;

                        case 0x0007: // 8xy7 - SUBN Vx, Vy
                                     // Set Vx = Vy - Vx, set VF = NOT borrow.
                                     // If Vy > Vx, then VF is set to 1, otherwise 0. Then Vx is subtracted from Vy, and the results stored in Vx.
                            if (registers[(opCode & 0x0F00) >> 8] > registers[(opCode & 0x00F0) >> 4])
                            {
                                registers[0xF] = 1;
                            }
                            else
                            {
                                registers[0xF] = 0;
                            }

                            registers[(opCode & 0x0F00) >> 8] = (byte)(registers[(opCode & 0x00F0) >> 4] - registers[(opCode & 0x0F00) >> 8]);
                            programCounter += 2;
                            break;

                        case 0x000E: // 8xyE - SHL Vx {, Vy}
                                     // Set Vx = Vx SHL 1.
                                     // If the most-significant bit of Vx is 1, then VF is set to 1, otherwise to 0. Then Vx is multiplied by 2.
                            registers[0xF] = (byte)(registers[(opCode & 0x0F00) >> 8] & 0x1); // if the last bit of Vx is one set VF to 1, otherwise set to 0
                            registers[(opCode & 0x0F00) >> 8] = (byte)(registers[(opCode & 0x0F00) >> 8] * 2);
                            programCounter += 2;
                            break;

                        default:
                            throw new Exception("Unknown opcode [0x0000]: 0x" + opCode);
                    }
                    break;

                case 0x9000: // 9xy0 - SNE Vx, Vy
                             // Skip next instruction if Vx != Vy.
                             // The values of Vx and Vy are compared, and if they are not equal, the program counter is increased by 2.
                    if (registers[(opCode & 0x0F00) >> 8] != registers[(opCode & 0x00F0) >> 4])
                    {
                        programCounter += 4;
                    } else
                    {
                        programCounter += 2;
                    }
                    break;

                case 0xA000: // Annn - LD I, addr
                             //  Set I = nnn.
                             // The value of register I is set to nnn.
                    indexRegister = (ushort)(opCode & 0x0FFF);
                    programCounter += 2;
                    break;

                case 0xB000: // Bnnn - JP V0, addr
                             // Jump to location nnn + V0.
                             // The program counter is set to nnn plus the value of V0.
                    programCounter = (ushort)((opCode & 0x0FFF) + registers[0]);
                    break;

                case 0xC000: // Cxkk - RND Vx, byte
                             // Set Vx = random byte AND kk.
                             // The interpreter generates a random number from 0 to 255, which is then ANDed with the value kk. The results are stored in Vx. See instruction 8xy2 for more information on AND.
                    Random rand = new Random();
                    registers[(opCode & 0x0F00) >> 8] = (byte)(rand.Next(255) & (opCode & 0x00FF));
                    programCounter += 2;
                    break;

                case 0xD000: // Dxyn - DRW Vx, Vy, nibble
                             // Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.
                             // The interpreter reads n bytes from memory, starting at the address stored in I. These bytes are then displayed as sprites on screen at coordinates (Vx, Vy). 
                             // Sprites are XORed onto the existing screen. If this causes any pixels to be erased, VF is set to 1, otherwise it is set to 0. 
                             // If the sprite is positioned so part of it is outside the coordinates of the display, it wraps around to the opposite side of the screen. 
                             // See instruction 8xy3 for more information on XOR, and section 2.4, Display, for more information on the Chip-8 screen and sprites.
                    ushort x = registers[(opCode & 0x0F00) >> 8];
                    ushort y = registers[(opCode & 0x00F0) >> 4];
                    ushort height = (ushort)(opCode & 0x000F);
                    ushort pixel;

                    registers[0xF] = 0;
                    for (int yline = 0; yline < height; yline++)
                    {
                        pixel = memory[indexRegister + yline];
                        for (int xline = 0; xline < 8; xline++)
                        {
                            if ((pixel & (0x80 >> xline)) != 0)
                            {
                                if (graphics[(x + xline + ((y + yline) * 64))] == 1)
                                    registers[0xF] = 1;
                                graphics[x + xline + ((y + yline) * 64)] ^= 1;
                            }
                        }
                    }

                    DrawFlag = true;
                    programCounter += 2;
                    break;

                case 0xE000:
                    switch(opCode & 0x00FF)
                    {
                        case 0x009E: // Ex9E - SKP Vx
                                     // Skip next instruction if key with the value of Vx is pressed.
                                     // Checks the keyboard, and if the key corresponding to the value of Vx is currently in the down position, PC is increased by 2.
                            if (key[registers[(opCode & 0x0F00) >> 8]] != 0)
                                programCounter += 4;
                            else
                                programCounter += 2;
                            break;

                        case 0x00A1: // ExA1 - SKNP Vx
                                     // Skip next instruction if key with the value of Vx is not pressed.
                                     // Checks the keyboard, and if the key corresponding to the value of Vx is currently in the up position, PC is increased by 2.
                            if (key[registers[(opCode & 0x0F00) >> 8]] == 0)
                                programCounter += 4;
                            else
                                programCounter += 2;
                            break;
                    }
                    break;

                case 0xF000:
                    switch (opCode & 0x00FF)
                    {
                        case 0x0007: // Fx07 - LD Vx, DT
                                     // Set Vx = delay timer value.
                                     // The value of DT is placed into Vx.
                            registers[(opCode & 0x0F00) >> 8] = delayTimer;
                            programCounter += 2;
                            break;

                        case 0x000A: // Fx0A - LD Vx, K
                                     // Wait for a key press, store the value of the key in Vx.
                                     // All execution stops until a key is pressed, then the value of that key is stored in Vx.

                            // loop through the keys pressed, if one is pressed capture the first one
                            // probably not the best way to do this, can't think of a better way right now
                            int keyValue = -1;
                            for(int i = 0; i < 16; i++)
                            {
                                if(key[i] > 0)
                                {
                                    keyValue = i;
                                    break;
                                }
                            }

                            if(keyValue > -1)
                            {
                                registers[(opCode & 0x0F00) >> 8] = (byte)keyValue;
                                programCounter += 2;
                            }
                            // if no value is found, don't update the programCounter i.e. continue executing this instruction
                            break;

                        case 0x0015: // Fx15 - LD DT, Vx
                                     // Set delay timer = Vx.
                                     // DT is set equal to the value of Vx.
                            delayTimer = (byte)((opCode & 0x0F00) >> 8);
                            programCounter += 2;
                            break;

                        case 0x0018: // Fx18 - LD ST, Vx
                                     // Set sound timer = Vx.
                                     // ST is set equal to the value of Vx.
                            soundTimer = (byte)((opCode & 0x0F00) >> 8);
                            programCounter += 2;
                            break;

                        case 0x001E: // Fx1E - ADD I, Vx
                                     // Set I = I + Vx.
                                     // The values of I and Vx are added, and the results are stored in I.
                            indexRegister += (byte)((opCode & 0x0F00) >> 8);
                            programCounter += 2;
                            break;

                        case 0x0029: // Fx29 - LD F, Vx
                                     // Set I = location of sprite for digit Vx.
                                     // The value of I is set to the location for the hexadecimal sprite corresponding to the value of Vx. See section 2.4, Display, for more information on the Chip-8 hexadecimal font.
                            indexRegister = (ushort)(registers[(opCode & 0x0F00) >> 8] * 0x5);
                            break;

                        case 0x0033: // Fx33 - LD B, Vx
                                     // Store BCD representation of Vx in memory locations I, I+1, and I+2.
                                     // The interpreter takes the decimal value of Vx, and places the hundreds digit in memory at location in I, the tens digit at location I+1, and the ones digit at location I+2.
                            memory[indexRegister] = (byte)(registers[(opCode & 0x0F00) >> 8] / 100);
                            memory[indexRegister + 1] = (byte)((registers[(opCode & 0x0F00) >> 8] / 10) % 10);
                            memory[indexRegister + 2] = (byte)((registers[(opCode & 0x0F00) >> 8] % 100) % 10);
                            programCounter += 2;
                            break;

                        case 0x0055: // Fx55 - LD [I], Vx
                                     // Store registers V0 through Vx in memory starting at location I.
                                     // The interpreter copies the values of registers V0 through Vx into memory, starting at the address in I.
                            for (int i = 0; i <= (opCode & 0x0F00) >> 8; i++)
                            {
                                memory[indexRegister + i] = registers[i];
                            }
                            programCounter += 2;
                            break;

                        case 0x0065: // Fx65 - LD Vx, [I]
                                     // Read registers V0 through Vx from memory starting at location I.
                                     // The interpreter reads values from memory starting at location I into registers V0 through Vx.
                            for (int i = 0; i <= (opCode & 0x0F00) >> 8; i++)
                            {
                                registers[i] = memory[indexRegister + i];
                            }
                            programCounter += 2;
                            break;

                        default:
                            throw new Exception("Unknown opcode [0x0000]: 0x" + opCode);
                    }
                    break;

                default:
                    throw new Exception("Unknown opCode: 0x" + Convert.ToString(opCode,toBase: 16));
            }

            // Update timers
            if (delayTimer > 0)
                delayTimer--;

            if (soundTimer > 0)
            {
                // play sound
                soundTimer--;
                throw new NotImplementedException("Sound not supported yet");
            }
        }

        /// <summary>
        /// Sets the state of the keys
        /// </summary>
        public void SetKeys()
        {
            throw new NotImplementedException();
        }
    }
}
