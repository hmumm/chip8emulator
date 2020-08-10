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

        private byte[] fontSet = new byte[80];

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

            // TODO implement font set
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
            
            // Decode Opcode
            // Execute Opcode

            // Update timers
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the state of the keys
        /// </summary>
        public void SetKeys()
        {

        }
    }
}
