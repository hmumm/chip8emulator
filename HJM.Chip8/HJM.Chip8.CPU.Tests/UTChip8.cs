using NUnit.Framework;

namespace HJM.Chip8.CPU.Tests
{
    public class Tests
    {
        Chip8 chip8;

        [SetUp]
        public void Setup()
        {
            chip8 = new Chip8();
            chip8.Initalize();
        }

        [Test]
        public void Op00E0()
        {
            // set the first instruction to 0x00E0
            chip8.Memory[0x0200] = 0x00;
            chip8.Memory[0x0201] = 0xE0;
            // change the graphics memory
            chip8.Graphics[0] = 0xF;
            chip8.EmulateCycle();

            // check that it got cleared
            Assert.AreEqual(0,chip8.Graphics[0]);
        }

        [Test]
        public void Op00EE()
        {
            // set the first instruction to 0x2202
            chip8.Memory[0x0200] = 0x22;
            chip8.Memory[0x0201] = 0x02;
            // set the second instruction to 0x00EE
            chip8.Memory[0x0202] = 0x00;
            chip8.Memory[0x0203] = 0xEE;

            // execute both instructions
            chip8.EmulateCycle();
            chip8.EmulateCycle();

            Assert.AreEqual(0x0200, chip8.ProgramCounter);
        }

        [Test]
        public void Op1nnn()
        {
            // set the first instruction to 0x1111
            chip8.Memory[0x0200] = 0x11;
            chip8.Memory[0x0201] = 0x11;

            // execute instruction
            chip8.EmulateCycle();

            Assert.AreEqual(0x0111, chip8.ProgramCounter);
        }

        [Test]
        public void Op2nnn()
        {
            // set the first instruction to 0x2202
            chip8.Memory[0x0200] = 0x22;
            chip8.Memory[0x0201] = 0x02;

            Assert.AreEqual(0, chip8.StackPointer);
            Assert.AreEqual(0x0, chip8.Stack[0]);

            // execute instruction
            chip8.EmulateCycle();

            Assert.AreEqual(1, chip8.StackPointer);
            Assert.AreEqual(0x0200, chip8.Stack[0]);
            Assert.AreEqual(0x0202, chip8.ProgramCounter);
        }

        [Test]
        public void Op3xkk()
        {
            // set the first instruction to 0x3001
            chip8.Memory[0x0200] = 0x30;
            chip8.Memory[0x0201] = 0x01;

            // set the second instrunction to 0x3000
            chip8.Memory[0x0202] = 0x30;
            chip8.Memory[0x0203] = 0x00;

            Assert.AreEqual(chip8.Registers[0], 0);

            chip8.EmulateCycle();

            // Vx should not have been equal to kk so incremented only by 1 (2 really)
            Assert.AreEqual(0x202, chip8.ProgramCounter);

            chip8.EmulateCycle();

            // Vx should have been equal to kk so incremented by 2 (4 really)
            Assert.AreEqual(0x206, chip8.ProgramCounter);
        }

        [Test]
        public void Op4xkk()
        {
            // set the first instruction to 0x4001
            chip8.Memory[0x0200] = 0x40;
            chip8.Memory[0x0201] = 0x01;

            // set the second instrunction to 0x4000
            chip8.Memory[0x0204] = 0x40;
            chip8.Memory[0x0205] = 0x00;

            Assert.AreEqual(chip8.Registers[0], 0);

            chip8.EmulateCycle();

            // Vx should have been equal to kk so incremented by 2 (4 really)
            Assert.AreEqual(0x204, chip8.ProgramCounter);

            chip8.EmulateCycle();

            // Vx should not have been equal to kk so incremented only by 1 (2 really)
            Assert.AreEqual(0x206, chip8.ProgramCounter);
        }

        [Test]
        public void Op5xy0()
        {
            // set the first instruction to 0x5000
            chip8.Memory[0x0200] = 0x50;
            chip8.Memory[0x0201] = 0x00;

            // set the second instrunction to 0x5010
            chip8.Memory[0x0204] = 0x50;
            chip8.Memory[0x0205] = 0x10;

            chip8.Registers[1] = 0x1;

            Assert.AreEqual(chip8.Registers[0], 0);
            Assert.AreEqual(chip8.Registers[1], 1);

            chip8.EmulateCycle();

            // Vx should have been equal to Vy so incremented by 2 (4 really)
            Assert.AreEqual(0x204, chip8.ProgramCounter);

            chip8.EmulateCycle();

            // Vx should not have been equal to Vy so incremented only by 1 (2 really)
            Assert.AreEqual(0x206, chip8.ProgramCounter);
        }

        [Test]
        public void Op6xkk()
        {
            // set the first instruction to 0x60FF
            chip8.Memory[0x0200] = 0x60;
            chip8.Memory[0x0201] = 0xFF;

            chip8.EmulateCycle();

            Assert.AreEqual(0xFF, chip8.Registers[0]);
        }

        [Test]
        public void Op7xkk()
        {
            chip8.Registers[0] = 0x05;

            // set the first instruction to 0x7005
            chip8.Memory[0x0200] = 0x70;
            chip8.Memory[0x0201] = 0x05;

            chip8.EmulateCycle();

            Assert.AreEqual(0x05 + 0x05, chip8.Registers[0]);
        }

        [Test]
        public void Op8xy0()
        {
            chip8.Registers[1] = 0x05;

            // set the first instruction to 0x8010
            chip8.Memory[0x0200] = 0x80;
            chip8.Memory[0x0201] = 0x10;

            chip8.EmulateCycle();

            Assert.AreEqual(0x05, chip8.Registers[0]);
        }
    }
}