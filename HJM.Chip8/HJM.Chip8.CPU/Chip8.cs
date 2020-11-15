using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HJM.Chip8.CPU.Exceptions;
using HJM.Chip8.CPU.Instructions;
using HJM.Chip8.CPU.Instructions.Chip8;
using Serilog;

namespace HJM.Chip8.CPU
{
    /// <summary>
    /// Emulates Chip8 Instructions.
    /// Does not do anything with graphics.
    /// </summary>
    public class Chip8
    {
        private Dictionary<ushort, Instruction> Instructions = new Dictionary<ushort, Instruction>();
        public CPUState State { get; set; } = new CPUState();

        private Stopwatch timerStopWatch = new Stopwatch();
        private const double TIMER_INTERVAL = (1 / 60) * 1000;

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

        public Chip8()
        {
            // Get all types that are Instructions under HJM.Chip8.CPU.Instructions.Chip8 namespace.
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(Instruction).IsAssignableFrom(p) && p.Namespace == "HJM.Chip8.CPU.Instructions.Chip8");

            // Make instances of these types and add to the Instruction dictionary.
            foreach (Type type in types)
            {
                Instruction? instruction = (Instruction?)Activator.CreateInstance(type);

                if (instruction == null)
                {
                    throw new NullReferenceException("Instruction instance was null");
                }

                Instructions.Add(instruction.OpCode, instruction);
            }
        }

        /// <summary>
        /// Initalizes memory for emulation.
        /// </summary>
        public void Initalize()
        {
            State.ProgramCounter = 0x200;
            State.OpCode = 0;
            State.IndexRegister = 0;
            State.StackPointer = 0;
            State.Stack = new ushort[16];
            State.Registers = new byte[16];
            State.Memory = new byte[4096];

            // Load the font set
            for (int i = 0; i < 80; i++)
                State.Memory[i] = FONT_SET[i];

            State.DelayTimer = 0;
            State.SoundTimer = 0;

            timerStopWatch.Restart();
        }

        /// <summary>
        /// Loads the game with the specified name
        /// </summary>
        /// <param name="pathToGame">Path of the game to run</param>
        public void LoadGame(string pathToGame)
        {
            Log.Information($"Loading game from \"{pathToGame}\".");

            byte[] data = System.IO.File.ReadAllBytes(pathToGame);

            Log.Information($"Read {data.Length} bytes.");

            if (data.Length > (4096 - 512))
            {
                throw new ArgumentException("Game to load is too large for memory.", nameof(pathToGame));
            }

            for (int i = 0; i < data.Length; i++)
            {
                State.Memory[i + 512] = (byte)(data[i] & 0x00FF);
            }
        }

        /// <summary>
        /// Emulates one Chip8 instruction cycle
        /// </summary>
        public void EmulateCycle()
        {
            fetchOpCode();

            executeInstruction();

            updateTimers();
        }

        private void fetchOpCode()
        {
            // Convert 2 1 byte memory addresses to 1 2 byte op code
            State.OpCode = (ushort)(State.Memory[State.ProgramCounter] << 8 | State.Memory[State.ProgramCounter + 1]);
        }

        private void updateTimers()
        {
            if (timerStopWatch.ElapsedMilliseconds > TIMER_INTERVAL)
            {
                // Update timers
                if (State.DelayTimer > 0)
                    State.DelayTimer--;

                if (State.SoundTimer > 0)
                {
                    // play sound
                    State.SoundTimer--;
                    State.SoundFlag = true;
                }
                else
                {
                    State.SoundFlag = false;
                }

                timerStopWatch.Restart();
            }
        }

        private void executeInstruction()
        {
            Log.Debug($"Executing OpCode {State.OpCode}");

            // Execute OpCode
            Instruction? instruction = Instructions.GetValueOrDefault((ushort)(State.OpCode & 0xF000));

            if (instruction == null)
            {
                throw new InvalidOpCodeException(State.OpCode);
            }

            State.ApplyStateChange(instruction.Execute(State));
        }
    }
}
