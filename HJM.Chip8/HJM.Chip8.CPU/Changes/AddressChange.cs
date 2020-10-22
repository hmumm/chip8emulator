using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Changes
{
    public class AddressChange<T> : Change<T>
    {
        public T AddressChanged { get; set; }
    }
}
