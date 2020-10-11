using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU
{
    public class CPUState
    {
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
    }
}
