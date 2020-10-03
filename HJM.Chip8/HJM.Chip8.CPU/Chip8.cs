using System;

namespace HJM.Chip8.CPU
{
    /// <summary>
    /// Emulates Chip8 Instructions.
    /// Does not do anything with graphics.
    /// </summary>
    public class Chip8
    {
        /// <summary>
        /// Whether the screen needs to be drawn or not
        /// </summary>
        public bool DrawFlag { get; set; }
        /// <summary>
        /// Whether sound should be played or not 
        /// </summary>
        public bool SoundFlag { get; set; }
        public ushort OpCode { get; set; }
        /// <summary>
        /// 0x000-0x1FF - Chip 8 interpreter(contains font set in emu)
        /// 0x050-0x0A0 - Used for the built in 4x5 pixel font set(0-F)
        /// 0x200-0xFFF - Program ROM and work RAM
        /// </summary>
        public byte[] Memory { get; set; } = new byte[4096];
        public byte[] Registers { get; set; } = new byte[16];
        public ushort IndexRegister { get; set; }
        public ushort ProgramCounter { get; set; }
        public byte[] Graphics { get; set; } = new byte[64 * 32];
        public byte DelayTimer { get; set; }
        public byte SoundTimer { get; set; }
        public ushort[] Stack { get; set; } = new ushort[16];
        public ushort StackPointer { get; set; }
        public byte[] Key { get; set; } = new byte[16];

        private readonly byte[] FONT_SET =
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
            ProgramCounter = 0x200;
            OpCode = 0;
            IndexRegister = 0;
            StackPointer = 0;
            Stack = new ushort[16];
            Registers = new byte[16];
            Memory = new byte[4096];

            // Load the font set
            for (int i = 0; i < 80; i++)
                Memory[i] = FONT_SET[i];

            DelayTimer = 0;
            SoundTimer = 0;
        }

        /// <summary>
        /// Loads the game with the specified name
        /// </summary>
        /// <param name="pathToGame">Path of the game to run</param>
        public void LoadGame(string pathToGame)
        {
            byte[] data = System.IO.File.ReadAllBytes(pathToGame);

            if (data.Length > (4096 - 512))
            {
                throw new ArgumentException("Game to load is too large for memory.", nameof(pathToGame));
            }

            for (int i = 0; i < data.Length; i++)
            {
                Memory[i + 512] = (byte)(data[i] & 0x00FF);
            }
        }

        /// <summary>
        /// Emulates one Chip8 instruction cycle
        /// </summary>
        public void EmulateCycle()
        {
            // Fetch Opcode
            // Convert 2 1 byte memory addresses to 1 2 byte op code
            OpCode = (ushort)(Memory[ProgramCounter] << 8 | Memory[ProgramCounter + 1]);

            // Decode Opcode
            switch (OpCode & 0xF000)
            {
                case 0x0000:
                    switch (OpCode & 0x00FF)
                    {
                        case 0x00E0: //  00E0 - CLS
                                     //  Clear the display.
                            Graphics = new byte[64 * 32];
                            DrawFlag = true;
                            ProgramCounter += 2;
                            break;

                        case 0x00EE: // 00EE - RET
                                     // Return from a subroutine.
                                     // The interpreter sets the program counter to the address at the top of the stack, then subtracts 1 from the stack pointer.
                            StackPointer -= 1;
                            ProgramCounter = Stack[StackPointer];
                            ProgramCounter += 2;
                            break;

                        default:
                            throw new InvalidOperationException($"Unknown opCode: 0x{Convert.ToString(OpCode, toBase: 16)}");
                    }
                    break;

                case 0x1000: // 1nnn - JP addr
                             // Jump to location nnn.
                             // The interpreter sets the program counter to nnn.
                    ProgramCounter = (ushort)(OpCode & 0x0FFF);
                    break;

                case 0x2000: // 2nnn - CALL addr
                             //  Call subroutine at nnn.
                             // The interpreter increments the stack pointer, then puts the current PC on the top of the stack. The PC is then set to nnn.
                    Stack[StackPointer] = ProgramCounter;
                    StackPointer++;
                    ProgramCounter = (ushort)(OpCode & 0x0FFF);
                    break;
                case 0x3000: // 3xkk - SE Vx, byte
                             // Skip next instruction if Vx = kk.
                             // The interpreter compares register Vx to kk, and if they are equal, increments the program counter by 2.
                    if (Registers[(ushort)((OpCode & 0x0F00) >> 8)] == (ushort)(OpCode & 0x00FF))
                    {
                        ProgramCounter += 4;
                    }
                    else
                    {
                        ProgramCounter += 2;
                    }
                    break;

                case 0x4000: // 4xkk - SNE Vx, byte
                             // Skip next instruction if Vx != kk.
                             // The interpreter compares register Vx to kk, and if they are not equal, increments the program counter by 2.
                    if (Registers[(ushort)((OpCode & 0x0F00) >> 8)] != (ushort)(OpCode & 0x00FF))
                    {
                        ProgramCounter += 4;
                    }
                    else
                    {
                        ProgramCounter += 2;
                    }
                    break;

                case 0x5000: // 5xy0 - SE Vx, Vy
                             // Skip next instruction if Vx = Vy.
                             // The interpreter compares register Vx to register Vy, and if they are equal, increments the program counter by 2.
                    if (Registers[(OpCode & 0x0F00) >> 8] == Registers[(OpCode & 0x00F0) >> 4])
                    {
                        ProgramCounter += 4;
                    }
                    else
                    {
                        ProgramCounter += 2;
                    }
                    break;
                case 0x6000: // 6xkk - LD Vx, byte
                             // Set Vx = kk.
                             // The interpreter puts the value kk into register Vx.
                    Registers[(OpCode & 0x0F00) >> 8] = (byte)(OpCode & 0x00FF);
                    ProgramCounter += 2;
                    break;

                case 0x7000: // 7xkk - ADD Vx, byte
                             // Set Vx = Vx + kk.
                             // Adds the value kk to the value of register Vx, then stores the result in Vx.
                    Registers[(OpCode & 0x0F00) >> 8] = (byte)(Registers[(OpCode & 0x0F00) >> 8] + (OpCode & 0x00FF));
                    ProgramCounter += 2;
                    break;

                case 0x8000:
                    switch (OpCode & 0x000F)
                    {
                        case 0x0000: // 8xy0 - LD Vx, Vy
                                     // Set Vx = Vy.
                                     // Stores the value of register Vy in register Vx.
                            Registers[(OpCode & 0x0F00) >> 8] = Registers[(OpCode & 0x00F0) >> 4];
                            ProgramCounter += 2;
                            break;

                        case 0x0001: // 8xy1 - OR Vx, Vy
                                     // Set Vx = Vx OR Vy.
                                     //  Performs a bitwise OR on the values of Vx and Vy, then stores the result in Vx. 
                                     // A bitwise OR compares the corrseponding bits from two values, and if either bit is 1, then the same bit in the result is also 1. Otherwise, it is 0. 
                            Registers[(OpCode & 0x0F00) >> 8] = (byte)(Registers[(OpCode & 0x0F00) >> 8] | Registers[(OpCode & 0x00F0) >> 4]);
                            ProgramCounter += 2;
                            break;

                        case 0x0002: // 8xy2 - AND Vx, Vy
                                     // Set Vx = Vx AND Vy.
                                     // Performs a bitwise AND on the values of Vx and Vy, then stores the result in Vx. 
                                     // A bitwise AND compares the corrseponding bits from two values, and if both bits are 1, then the same bit in the result is also 1. Otherwise, it is 0. 
                            Registers[(OpCode & 0x0F00) >> 8] = (byte)(Registers[(OpCode & 0x0F00) >> 8] & Registers[(OpCode & 0x00F0) >> 4]);
                            ProgramCounter += 2;
                            break;

                        case 0x0003: // 8xy3 - XOR Vx, Vy
                                     // Set Vx = Vx XOR Vy.
                                     // Performs a bitwise exclusive OR on the values of Vx and Vy, then stores the result in Vx. 
                                     // An exclusive OR compares the corrseponding bits from two values, and if the bits are not both the same, then the corresponding bit in the result is set to 1. Otherwise, it is 0. 
                            Registers[(OpCode & 0x0F00) >> 8] = (byte)(Registers[(OpCode & 0x0F00) >> 8] ^ Registers[(OpCode & 0x00F0) >> 4]);
                            ProgramCounter += 2;
                            break;

                        case 0x0004: // 8xy4 - ADD Vx, Vy
                                     // Set Vx = Vx + Vy, set VF = carry.
                                     //  The values of Vx and Vy are added together. If the result is greater than 8 bits (i.e., > 255,) VF is set to 1, otherwise 0. Only the lowest 8 bits of the result are kept, and stored in Vx.
                            if (Registers[(OpCode & 0x00F0) >> 4] > (0xFF - Registers[(OpCode & 0x0F00) >> 8]))
                                Registers[0xF] = 1; //carry
                            else
                                Registers[0xF] = 0;

                            Registers[(OpCode & 0x0F00) >> 8] += Registers[(OpCode & 0x00F0) >> 4];
                            ProgramCounter += 2;
                            break;

                        case 0x0005: // 8xy5 - SUB Vx, Vy
                                     // Set Vx = Vx - Vy, set VF = NOT borrow.
                                     // If Vx > Vy, then VF is set to 1, otherwise 0. Then Vy is subtracted from Vx, and the results stored in Vx.
                            if (Registers[(OpCode & 0x0F00) >> 8] > Registers[(OpCode & 0x00F0) >> 4])
                            {
                                Registers[0xF] = 1;
                            }
                            else
                            {
                                Registers[0xF] = 0;
                            }

                            Registers[(OpCode & 0x0F00) >> 8] = (byte)(Registers[(OpCode & 0x0F00) >> 8] - Registers[(OpCode & 0x00F0) >> 4]);
                            ProgramCounter += 2;
                            break;

                        case 0x0006: // 8xy6 - SHR Vx {, Vy}
                                     // Set Vx = Vx SHR 1.
                                     //  If the least-significant bit of Vx is 1, then VF is set to 1, otherwise 0. Then Vx is divided by 2.
                            Registers[0xF] = (byte)(Registers[(OpCode & 0x0F00) >> 8] & 0x1); // if the last bit of Vx is one set VF to 1, otherwise set to 0
                            Registers[(OpCode & 0x0F00) >> 8] = (byte)(Registers[(OpCode & 0x0F00) >> 8] / 2);
                            ProgramCounter += 2;
                            break;

                        case 0x0007: // 8xy7 - SUBN Vx, Vy
                                     // Set Vx = Vy - Vx, set VF = NOT borrow.
                                     // If Vy > Vx, then VF is set to 1, otherwise 0. Then Vx is subtracted from Vy, and the results stored in Vx.
                            if (Registers[(OpCode & 0x00F0) >> 4] > Registers[(OpCode & 0x0F00) >> 8])
                            {
                                Registers[0xF] = 1;
                            }
                            else
                            {
                                Registers[0xF] = 0;
                            }

                            Registers[(OpCode & 0x0F00) >> 8] = (byte)(Registers[(OpCode & 0x00F0) >> 4] - Registers[(OpCode & 0x0F00) >> 8]);
                            ProgramCounter += 2;
                            break;

                        case 0x000E: // 8xyE - SHL Vx {, Vy}
                                     // Set Vx = Vx SHL 1.
                                     // If the most-significant bit of Vx is 1, then VF is set to 1, otherwise to 0. Then Vx is multiplied by 2.
                            Registers[0xF] = (byte)(Registers[(OpCode & 0x0F00) >> 8] & 0x1); // if the last bit of Vx is one set VF to 1, otherwise set to 0
                            Registers[(OpCode & 0x0F00) >> 8] = (byte)(Registers[(OpCode & 0x0F00) >> 8] * 2);
                            ProgramCounter += 2;
                            break;

                        default:
                            throw new InvalidOperationException($"Unknown opCode: 0x{Convert.ToString(OpCode, toBase: 16)}");
                    }
                    break;

                case 0x9000: // 9xy0 - SNE Vx, Vy
                             // Skip next instruction if Vx != Vy.
                             // The values of Vx and Vy are compared, and if they are not equal, the program counter is increased by 2.
                    if (Registers[(OpCode & 0x0F00) >> 8] != Registers[(OpCode & 0x00F0) >> 4])
                    {
                        ProgramCounter += 4;
                    }
                    else
                    {
                        ProgramCounter += 2;
                    }
                    break;

                case 0xA000: // Annn - LD I, addr
                             //  Set I = nnn.
                             // The value of register I is set to nnn.
                    IndexRegister = (ushort)(OpCode & 0x0FFF);
                    ProgramCounter += 2;
                    break;

                case 0xB000: // Bnnn - JP V0, addr
                             // Jump to location nnn + V0.
                             // The program counter is set to nnn plus the value of V0.
                    ProgramCounter = (ushort)((OpCode & 0x0FFF) + Registers[0]);
                    break;

                case 0xC000: // Cxkk - RND Vx, byte
                             // Set Vx = random byte AND kk.
                             // The interpreter generates a random number from 0 to 255, which is then ANDed with the value kk. The results are stored in Vx. See instruction 8xy2 for more information on AND.
                    Random rand = new Random();
                    Registers[(OpCode & 0x0F00) >> 8] = (byte)(rand.Next(255) & (OpCode & 0x00FF));
                    ProgramCounter += 2;
                    break;

                case 0xD000: // Dxyn - DRW Vx, Vy, nibble
                             // Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.
                             // The interpreter reads n bytes from memory, starting at the address stored in I. These bytes are then displayed as sprites on screen at coordinates (Vx, Vy). 
                             // Sprites are XORed onto the existing screen. If this causes any pixels to be erased, VF is set to 1, otherwise it is set to 0. 
                             // If the sprite is positioned so part of it is outside the coordinates of the display, it wraps around to the opposite side of the screen. 
                             // See instruction 8xy3 for more information on XOR, and section 2.4, Display, for more information on the Chip-8 screen and sprites.
                    ushort x = Registers[(OpCode & 0x0F00) >> 8];
                    ushort y = Registers[(OpCode & 0x00F0) >> 4];
                    ushort height = (ushort)(OpCode & 0x000F);
                    ushort pixel;

                    Registers[0xF] = 0;
                    for (int yline = 0; yline < height; yline++)
                    {
                        pixel = Memory[IndexRegister + yline];
                        for (int xline = 0; xline < 8; xline++)
                        {
                            if ((pixel & (0x80 >> xline)) != 0)
                            {
                                if (Graphics[(x + xline + ((y + yline) * 64))] == 1)
                                    Registers[0xF] = 1;
                                Graphics[x + xline + ((y + yline) * 64)] ^= 1;
                            }
                        }
                    }

                    DrawFlag = true;
                    ProgramCounter += 2;
                    break;

                case 0xE000:
                    switch (OpCode & 0x00FF)
                    {
                        case 0x009E: // Ex9E - SKP Vx
                                     // Skip next instruction if key with the value of Vx is pressed.
                                     // Checks the keyboard, and if the key corresponding to the value of Vx is currently in the down position, PC is increased by 2.
                            if (Key[Registers[(OpCode & 0x0F00) >> 8]] != 0)
                                ProgramCounter += 4;
                            else
                                ProgramCounter += 2;
                            break;

                        case 0x00A1: // ExA1 - SKNP Vx
                                     // Skip next instruction if key with the value of Vx is not pressed.
                                     // Checks the keyboard, and if the key corresponding to the value of Vx is currently in the up position, PC is increased by 2.
                            if (Key[Registers[(OpCode & 0x0F00) >> 8]] == 0)
                                ProgramCounter += 4;
                            else
                                ProgramCounter += 2;
                            break;
                        default:
                            throw new InvalidOperationException($"Unknown opCode: 0x{Convert.ToString(OpCode, toBase: 16)}");
                    }
                    break;

                case 0xF000:
                    switch (OpCode & 0x00FF)
                    {
                        case 0x0007: // Fx07 - LD Vx, DT
                                     // Set Vx = delay timer value.
                                     // The value of DT is placed into Vx.
                            Registers[(OpCode & 0x0F00) >> 8] = DelayTimer;
                            ProgramCounter += 2;
                            break;

                        case 0x000A: // Fx0A - LD Vx, K
                                     // Wait for a key press, store the value of the key in Vx.
                                     // All execution stops until a key is pressed, then the value of that key is stored in Vx.

                            // loop through the keys pressed, if one is pressed capture the first one
                            // probably not the best way to do this, can't think of a better way right now
                            int keyValue = -1;
                            for (int i = 0; i < 16; i++)
                            {
                                if (Key[i] > 0)
                                {
                                    keyValue = i;
                                    break;
                                }
                            }

                            if (keyValue > -1)
                            {
                                Registers[(OpCode & 0x0F00) >> 8] = (byte)keyValue;
                                ProgramCounter += 2;
                            }
                            // if no value is found, don't update the programCounter i.e. continue executing this instruction
                            break;

                        case 0x0015: // Fx15 - LD DT, Vx
                                     // Set delay timer = Vx.
                                     // DT is set equal to the value of Vx.
                            DelayTimer = Registers[(OpCode & 0x0F00) >> 8];
                            ProgramCounter += 2;
                            break;

                        case 0x0018: // Fx18 - LD ST, Vx
                                     // Set sound timer = Vx.
                                     // ST is set equal to the value of Vx.
                            SoundTimer = Registers[(OpCode & 0x0F00) >> 8];
                            ProgramCounter += 2;
                            break;

                        case 0x001E: // Fx1E - ADD I, Vx
                                     // Set I = I + Vx.
                                     // The values of I and Vx are added, and the results are stored in I.
                            IndexRegister += Registers[(OpCode & 0x0F00) >> 8];
                            ProgramCounter += 2;
                            break;

                        case 0x0029: // Fx29 - LD F, Vx
                                     // Set I = location of sprite for digit Vx.
                                     // The value of I is set to the location for the hexadecimal sprite corresponding to the value of Vx. See section 2.4, Display, for more information on the Chip-8 hexadecimal font.
                            IndexRegister = (ushort)(Registers[(OpCode & 0x0F00) >> 8] * 0x5);
                            ProgramCounter += 2;
                            break;

                        case 0x0033: // Fx33 - LD B, Vx
                                     // Store BCD representation of Vx in memory locations I, I+1, and I+2.
                                     // The interpreter takes the decimal value of Vx, and places the hundreds digit in memory at location in I, the tens digit at location I+1, and the ones digit at location I+2.
                            Memory[IndexRegister] = (byte)(Registers[(OpCode & 0x0F00) >> 8] / 100);
                            Memory[IndexRegister + 1] = (byte)((Registers[(OpCode & 0x0F00) >> 8] / 10) % 10);
                            Memory[IndexRegister + 2] = (byte)((Registers[(OpCode & 0x0F00) >> 8] % 100) % 10);
                            ProgramCounter += 2;
                            break;

                        case 0x0055: // Fx55 - LD [I], Vx
                                     // Store registers V0 through Vx in memory starting at location I.
                                     // The interpreter copies the values of registers V0 through Vx into memory, starting at the address in I.
                            for (int i = 0; i <= (OpCode & 0x0F00) >> 8; i++)
                            {
                                Memory[IndexRegister + i] = Registers[i];
                            }
                            ProgramCounter += 2;
                            break;

                        case 0x0065: // Fx65 - LD Vx, [I]
                                     // Read registers V0 through Vx from memory starting at location I.
                                     // The interpreter reads values from memory starting at location I into registers V0 through Vx.
                            for (int i = 0; i <= (OpCode & 0x0F00) >> 8; i++)
                            {
                                Registers[i] = Memory[IndexRegister + i];
                            }
                            ProgramCounter += 2;
                            break;

                        default:
                            throw new InvalidOperationException($"Unknown opCode: 0x{Convert.ToString(OpCode, toBase: 16)}");
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Unknown opCode: 0x{Convert.ToString(OpCode, toBase: 16)}");
            }

            // Update timers
            if (DelayTimer > 0)
                DelayTimer--;

            if (SoundTimer > 0)
            {
                // play sound
                SoundTimer--;
                SoundFlag = true;
            }
            else
            {
                SoundFlag = false;
            }
        }
    }
}
