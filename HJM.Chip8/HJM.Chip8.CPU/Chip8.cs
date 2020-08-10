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

        /// <summary>
        /// Initalizes memory and everything for emulation.
        /// </summary>
        public void Initalize()
        {
            throw new NotImplementedException();
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
