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

                case 0xA000: // Annn - LD I, addr
                             //  Set I = nnn.
                             // The value of register I is set to nnn.
                    indexRegister = (ushort)(opCode & 0x0FFF);
                    programCounter += 2;
                    break;

                case 0x2000: // 2nnn - CALL addr
                             //  Call subroutine at nnn.
                             // The interpreter increments the stack pointer, then puts the current PC on the top of the stack. The PC is then set to nnn.
                    stack[stackPointer] = programCounter;
                    stackPointer++;
                    programCounter = (ushort)(opCode & 0x0FFF);
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

                case 0x0033: // Fx33 - LD B, Vx
                             // Store BCD representation of Vx in memory locations I, I+1, and I+2.
                             // The interpreter takes the decimal value of Vx, and places the hundreds digit in memory at location in I, the tens digit at location I+1, and the ones digit at location I+2.
                    memory[indexRegister] = (byte)(registers[(opCode & 0x0F00) >> 8] / 100);
                    memory[indexRegister + 1] = (byte)((registers[(opCode & 0x0F00) >> 8] / 10) % 10);
                    memory[indexRegister + 2] = (byte)((registers[(opCode & 0x0F00) >> 8] % 100) % 10);
                    programCounter += 2;
                    break;

                default:
                    throw new Exception("Unknown opCode: 0x" + (char)opCode);
            }

            // Update timers
            if (delayTimer > 0)
                delayTimer--;

            if (soundTimer > 0)
            {
                if (soundTimer == 1)
                    throw new NotImplementedException("Sound not implemented yet");
                soundTimer--;
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
