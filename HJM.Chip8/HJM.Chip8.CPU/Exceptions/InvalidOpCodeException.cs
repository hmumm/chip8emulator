using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Exceptions
{
    /// <summary>
    /// Exception for when an invalid opcode is trying to be executed
    /// </summary>
    public class InvalidOpCodeException : InvalidOperationException
    {
        public InvalidOpCodeException(ushort opCode) : base(OpCodeToMessage(opCode))
        {

        }

        private static string OpCodeToMessage(ushort opCode)
        {
            return $"Unknown opCode: 0x{Convert.ToString(opCode, toBase: 16)}";
        }
    }
}
