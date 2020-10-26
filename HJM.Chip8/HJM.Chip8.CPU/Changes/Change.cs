using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.CPU.Changes
{
    /// <summary>
    /// Represents a change in value
    /// </summary>
    /// <typeparam name="T">Type of the value to be changed</typeparam>
    public class Change<T>
    {
        public string Description { get; set; } = string.Empty;
#pragma warning disable CS8601 // Possible null reference assignment.
        public T OldValue { get; set; } = default;
        public T NewValue { get; set; } = default;
#pragma warning restore CS8601 // Possible null reference assignment.
    }
}
