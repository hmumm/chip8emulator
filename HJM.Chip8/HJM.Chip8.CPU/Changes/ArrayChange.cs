﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Changes
{
    /// <summary>
    /// Represents a change at a specific address
    /// </summary>
    /// <typeparam name="T">Type of the change</typeparam>
    public class ArrayChange<T> : Change<T>
    {
        /// <summary>
        /// The address at which the value is changed
        /// </summary>
        public int IndexChanged { get; set; } = -1;
    }
}
