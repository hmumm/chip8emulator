using NUnit.Framework;
using System.Threading;

namespace HJM.Chip8.CPU.Tests
{
    public class UTChip8
    {
        // Always initialized in Setup
        private Chip8 chip8 = null!;

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
            chip8.State.Memory[0x0200] = 0x00;
            chip8.State.Memory[0x0201] = 0xE0;

            // change the graphics memory
            chip8.State.Graphics[0] = 0xF;
            chip8.EmulateCycle();

            // check that it got cleared
            Assert.AreEqual(0, chip8.State.Graphics[0]);
        }

        [Test]
        public void Op00EE()
        {
            // set the first instruction to 0x00EE
            chip8.State.Memory[0x0200] = 0x00;
            chip8.State.Memory[0x0201] = 0xEE;

            chip8.State.Stack[0] = 0x202;
            chip8.State.StackPointer = 0x1;

            // execute instruction
            chip8.EmulateCycle();

            Assert.AreEqual(0x0204, chip8.State.ProgramCounter);
        }

        [Test]
        public void Op1nnn()
        {
            // set the first instruction to 0x1111
            chip8.State.Memory[0x0200] = 0x11;
            chip8.State.Memory[0x0201] = 0x11;

            // execute instruction
            chip8.EmulateCycle();

            Assert.AreEqual(0x0111, chip8.State.ProgramCounter);
        }

        [Test]
        public void Op2nnn()
        {
            // set the first instruction to 0x2202
            chip8.State.Memory[0x0200] = 0x22;
            chip8.State.Memory[0x0201] = 0x02;

            Assert.AreEqual(0, chip8.State.StackPointer);
            Assert.AreEqual(0x0, chip8.State.Stack[0]);

            // execute instruction
            chip8.EmulateCycle();

            Assert.AreEqual(1, chip8.State.StackPointer);
            Assert.AreEqual(0x0200, chip8.State.Stack[0]);
            Assert.AreEqual(0x0202, chip8.State.ProgramCounter);
        }

        [Test]
        public void Op3xkk()
        {
            // set the first instruction to 0x3001
            chip8.State.Memory[0x0200] = 0x30;
            chip8.State.Memory[0x0201] = 0x01;

            // set the second instrunction to 0x3000
            chip8.State.Memory[0x0202] = 0x30;
            chip8.State.Memory[0x0203] = 0x00;

            Assert.AreEqual(chip8.State.Registers[0], 0);

            chip8.EmulateCycle();

            // Vx should not have been equal to kk so incremented only by 1 (2 really)
            Assert.AreEqual(0x202, chip8.State.ProgramCounter);

            chip8.EmulateCycle();

            // Vx should have been equal to kk so incremented by 2 (4 really)
            Assert.AreEqual(0x206, chip8.State.ProgramCounter);
        }

        [Test]
        public void Op4xkk()
        {
            // set the first instruction to 0x4001
            chip8.State.Memory[0x0200] = 0x40;
            chip8.State.Memory[0x0201] = 0x01;

            // set the second instrunction to 0x4000
            chip8.State.Memory[0x0204] = 0x40;
            chip8.State.Memory[0x0205] = 0x00;

            Assert.AreEqual(chip8.State.Registers[0], 0);

            chip8.EmulateCycle();

            // Vx should have been equal to kk so incremented by 2 (4 really)
            Assert.AreEqual(0x204, chip8.State.ProgramCounter);

            chip8.EmulateCycle();

            // Vx should not have been equal to kk so incremented only by 1 (2 really)
            Assert.AreEqual(0x206, chip8.State.ProgramCounter);
        }

        [Test]
        public void Op5xy0()
        {
            // set the first instruction to 0x5000
            chip8.State.Memory[0x0200] = 0x50;
            chip8.State.Memory[0x0201] = 0x00;

            // set the second instrunction to 0x5010
            chip8.State.Memory[0x0204] = 0x50;
            chip8.State.Memory[0x0205] = 0x10;

            chip8.State.Registers[1] = 0x1;

            Assert.AreEqual(chip8.State.Registers[0], 0);
            Assert.AreEqual(chip8.State.Registers[1], 1);

            chip8.EmulateCycle();

            // Vx should have been equal to Vy so incremented by 2 (4 really)
            Assert.AreEqual(0x204, chip8.State.ProgramCounter);

            chip8.EmulateCycle();

            // Vx should not have been equal to Vy so incremented only by 1 (2 really)
            Assert.AreEqual(0x206, chip8.State.ProgramCounter);
        }

        [Test]
        public void Op6xkk()
        {
            // set the first instruction to 0x60FF
            chip8.State.Memory[0x0200] = 0x60;
            chip8.State.Memory[0x0201] = 0xFF;

            chip8.EmulateCycle();

            Assert.AreEqual(0xFF, chip8.State.Registers[0]);
        }

        [Test]
        public void Op7xkk()
        {
            chip8.State.Registers[0] = 0x05;

            // set the first instruction to 0x7005
            chip8.State.Memory[0x0200] = 0x70;
            chip8.State.Memory[0x0201] = 0x05;

            chip8.EmulateCycle();

            Assert.AreEqual(0x05 + 0x05, chip8.State.Registers[0]);
        }

        [Test]
        public void Op8xy0()
        {
            chip8.State.Registers[1] = 0x05;

            // set the first instruction to 0x8010
            chip8.State.Memory[0x0200] = 0x80;
            chip8.State.Memory[0x0201] = 0x10;

            chip8.EmulateCycle();

            Assert.AreEqual(0x05, chip8.State.Registers[0]);
        }

        [Test]
        public void Op8xy1()
        {
            chip8.State.Registers[0] = 0x0;
            chip8.State.Registers[1] = 0x1;

            // set the first instruction to 0x8011
            chip8.State.Memory[0x0200] = 0x80;
            chip8.State.Memory[0x0201] = 0x11;

            chip8.EmulateCycle();

            Assert.AreEqual(0x1, chip8.State.Registers[0]);
        }

        [Test]
        public void Op8xy2()
        {
            chip8.State.Registers[0] = 0x2;
            chip8.State.Registers[1] = 0x1;

            // set the first instruction to 0x8012
            chip8.State.Memory[0x0200] = 0x80;
            chip8.State.Memory[0x0201] = 0x12;

            chip8.EmulateCycle();

            Assert.AreEqual(0x0, chip8.State.Registers[0]);
        }

        [Test]
        public void Op8xy3()
        {
            chip8.State.Registers[0] = 0x2;
            chip8.State.Registers[1] = 0x1;

            // set the first instruction to 0x8013
            chip8.State.Memory[0x0200] = 0x80;
            chip8.State.Memory[0x0201] = 0x13;

            chip8.EmulateCycle();

            Assert.AreEqual(0x3, chip8.State.Registers[0]);
        }

        [Test]
        public void Op8xy4()
        {
            chip8.State.Registers[0] = 0xFF;
            chip8.State.Registers[1] = 0xFF;

            // set the first instruction to 0x8014
            chip8.State.Memory[0x0200] = 0x80;
            chip8.State.Memory[0x0201] = 0x14;

            chip8.EmulateCycle();

            Assert.AreEqual(0xFE, chip8.State.Registers[0]);
            Assert.AreEqual(0x1, chip8.State.Registers[0xF]);
        }

        [Test]
        public void Op8xy5()
        {
            chip8.State.Registers[0] = 0xFF;
            chip8.State.Registers[1] = 0x00;

            // set the first instruction to 0x8015
            chip8.State.Memory[0x0200] = 0x80;
            chip8.State.Memory[0x0201] = 0x15;

            chip8.EmulateCycle();

            Assert.AreEqual(0xFF, chip8.State.Registers[0]);
            Assert.AreEqual(0x1, chip8.State.Registers[0xF]);
        }

        [Test]
        public void Op8xy6()
        {
            chip8.State.Registers[0] = 0xFF;

            // set the first instruction to 0x8016
            chip8.State.Memory[0x0200] = 0x80;
            chip8.State.Memory[0x0201] = 0x16;

            chip8.EmulateCycle();

            Assert.AreEqual(0xFF / 2, chip8.State.Registers[0]);
            Assert.AreEqual(0x1, chip8.State.Registers[0xF]);
        }

        [Test]
        public void Op8xy7()
        {
            chip8.State.Registers[0] = 0x00;
            chip8.State.Registers[1] = 0xFF;

            // set the first instruction to 0x8017
            chip8.State.Memory[0x0200] = 0x80;
            chip8.State.Memory[0x0201] = 0x17;

            chip8.EmulateCycle();

            Assert.AreEqual(0xFF, chip8.State.Registers[0]);
            Assert.AreEqual(0x1, chip8.State.Registers[0xF]);
        }

        [Test]
        public void Op8xyE()
        {
            chip8.State.Registers[0] = 0xFF;

            // set the first instruction to 0x801E
            chip8.State.Memory[0x0200] = 0x80;
            chip8.State.Memory[0x0201] = 0x1E;

            chip8.EmulateCycle();

            Assert.AreEqual(0xFE, chip8.State.Registers[0]);
            Assert.AreEqual(0x1, chip8.State.Registers[0xF]);
        }

        [Test]
        public void Op9xy0()
        {
            // set the first instruction to 0x9000
            chip8.State.Memory[0x0200] = 0x90;
            chip8.State.Memory[0x0201] = 0x00;

            // set the second instrunction to 0x9010
            chip8.State.Memory[0x0202] = 0x90;
            chip8.State.Memory[0x0203] = 0x10;

            chip8.State.Registers[1] = 0x1;

            Assert.AreEqual(chip8.State.Registers[0], 0);
            Assert.AreEqual(chip8.State.Registers[1], 1);

            chip8.EmulateCycle();

            // Vx should have been equal to Vy so incremented by 2 (4 really)
            Assert.AreEqual(0x202, chip8.State.ProgramCounter);

            chip8.EmulateCycle();

            // Vx should not have been equal to Vy so incremented only by 1 (2 really)
            Assert.AreEqual(0x206, chip8.State.ProgramCounter);
        }

        [Test]
        public void OpAnnn()
        {
            // set the first instruction to 0xAFFF
            chip8.State.Memory[0x0200] = 0xAF;
            chip8.State.Memory[0x0201] = 0xFF;

            chip8.EmulateCycle();

            Assert.AreEqual(0xFFF, chip8.State.IndexRegister);
        }

        [Test]
        public void OpBnnn()
        {
            // set the first instruction to 0xBFFF
            chip8.State.Memory[0x0200] = 0xBF;
            chip8.State.Memory[0x0201] = 0xFF;

            chip8.EmulateCycle();

            Assert.AreEqual(0xFFF, chip8.State.ProgramCounter);
        }

        [Test]
        public void OpCxkk()
        {
            // set the first instruction to 0xC0FF
            chip8.State.Memory[0x0200] = 0xC0;
            chip8.State.Memory[0x0201] = 0xFF;

            chip8.EmulateCycle();

            Assert.True(chip8.State.Registers[0] > 0 && chip8.State.Registers[0] < 255);
        }

        // TODO make this actually check the graphics memory
        [Test]
        public void OpDxyn()
        {
            // set the first instruction to 0xD0FF
            chip8.State.Memory[0x0200] = 0xD0;
            chip8.State.Memory[0x0201] = 0x0F;

            // set the second instruction to 0xD0FF
            chip8.State.Memory[0x0202] = 0xD0;
            chip8.State.Memory[0x0203] = 0x0F;

            chip8.EmulateCycle();

            // not going to check the actual graphics memory, just check the collision
            Assert.AreEqual(0x0, chip8.State.Registers[0xF]);

            chip8.EmulateCycle();

            // not going to check the actual graphics memory, just check the collision
            Assert.AreEqual(0x1, chip8.State.Registers[0xF]);
        }

        [Test]
        public void OpEx9E()
        {
            // set the first instruction to 0xE09E
            chip8.State.Memory[0x0200] = 0xE0;
            chip8.State.Memory[0x0201] = 0x9E;

            // set the second instruction to 0xE09E
            chip8.State.Memory[0x0202] = 0xE0;
            chip8.State.Memory[0x0203] = 0x9E;

            chip8.State.Key[0] = 0;

            chip8.EmulateCycle();

            Assert.AreEqual(0x202, chip8.State.ProgramCounter);

            chip8.State.Key[0] = 1;

            chip8.EmulateCycle();

            Assert.AreEqual(0x206, chip8.State.ProgramCounter);
        }

        [Test]
        public void OpExA1()
        {
            // set the first instruction to 0xE09E
            chip8.State.Memory[0x0200] = 0xE0;
            chip8.State.Memory[0x0201] = 0xA1;

            // set the second instruction to 0xE09E
            chip8.State.Memory[0x0202] = 0xE0;
            chip8.State.Memory[0x0203] = 0xA1;

            chip8.State.Key[0] = 1;

            chip8.EmulateCycle();

            Assert.AreEqual(0x202, chip8.State.ProgramCounter);

            chip8.State.Key[0] = 0;

            chip8.EmulateCycle();

            Assert.AreEqual(0x206, chip8.State.ProgramCounter);
        }

        [Test]
        public void OpFx07()
        {
            // set the first instruction to 0xF007
            chip8.State.Memory[0x0200] = 0xF0;
            chip8.State.Memory[0x0201] = 0x07;

            chip8.State.DelayTimer = 1;

            chip8.EmulateCycle();

            Assert.AreEqual(0x1, chip8.State.Registers[0]);
        }

        [Test]
        public void OpFx0A()
        {
            // set the first instruction to 0xF00A
            chip8.State.Memory[0x0200] = 0xF0;
            chip8.State.Memory[0x0201] = 0x0A;

            chip8.EmulateCycle();
            chip8.EmulateCycle();
            chip8.EmulateCycle();

            chip8.State.Key[1] = 1;

            chip8.EmulateCycle();

            Assert.AreEqual(0x1, chip8.State.Registers[0]);
        }

        [Test]
        public void OpFx15()
        {
            // set the first instruction to 0xF015
            chip8.State.Memory[0x0200] = 0xF0;
            chip8.State.Memory[0x0201] = 0x15;

            chip8.State.Registers[0] = 0xF;

            // Sleep so timer will update
            Thread.Sleep(50);

            chip8.EmulateCycle();

            Assert.AreEqual(0xF - 1, chip8.State.DelayTimer);
        }

        [Test]
        public void OpFx18()
        {
            // set the first instruction to 0xF018
            chip8.State.Memory[0x0200] = 0xF0;
            chip8.State.Memory[0x0201] = 0x18;

            chip8.State.Registers[0] = 0xF;

            // Sleep so timer will update
            Thread.Sleep(50);

            chip8.EmulateCycle();

            Assert.AreEqual(0xF - 1, chip8.State.SoundTimer);
        }

        [Test]
        public void OpFx1E()
        {
            // set the first instruction to 0xF01E
            chip8.State.Memory[0x0200] = 0xF0;
            chip8.State.Memory[0x0201] = 0x1E;

            chip8.State.Registers[0] = 0xF;
            chip8.State.IndexRegister = 0xF;

            chip8.EmulateCycle();

            Assert.AreEqual(0x1E, chip8.State.IndexRegister);
        }

        [Test]
        public void OpFx29()
        {
            // set the first instruction to 0xF029
            chip8.State.Memory[0x0200] = 0xF0;
            chip8.State.Memory[0x0201] = 0x29;

            chip8.State.Registers[0] = 0xF;

            chip8.EmulateCycle();

            Assert.AreEqual(0x4B, chip8.State.IndexRegister);
        }

        [Test]
        public void OpFx33()
        {
            // set the first instruction to 0xF033
            chip8.State.Memory[0x0200] = 0xF0;
            chip8.State.Memory[0x0201] = 0x33;

            chip8.State.Registers[0] = 0xFF;

            chip8.EmulateCycle();

            Assert.AreEqual(0x2, chip8.State.Memory[chip8.State.IndexRegister]);
            Assert.AreEqual(0x5, chip8.State.Memory[chip8.State.IndexRegister + 1]);
            Assert.AreEqual(0x5, chip8.State.Memory[chip8.State.IndexRegister + 2]);
        }

        [Test]
        public void OpFx55()
        {
            // set the first instruction to 0xF155
            chip8.State.Memory[0x0200] = 0xF1;
            chip8.State.Memory[0x0201] = 0x55;

            chip8.State.IndexRegister = 0;

            chip8.State.Registers[0] = 0xF;
            chip8.State.Registers[1] = 0xF;

            chip8.EmulateCycle();

            Assert.AreEqual(0xF, chip8.State.Memory[0]);
            Assert.AreEqual(0xF, chip8.State.Memory[1]);
        }

        [Test]
        public void OpFx65()
        {
            // set the first instruction to 0xF155
            chip8.State.Memory[0x0200] = 0xF1;
            chip8.State.Memory[0x0201] = 0x65;

            chip8.State.IndexRegister = 0;

            chip8.State.Memory[0] = 0xF;
            chip8.State.Memory[1] = 0xF;

            chip8.EmulateCycle();

            Assert.AreEqual(0xF, chip8.State.Registers[0]);
            Assert.AreEqual(0xF, chip8.State.Registers[1]);
        }
    }
}
