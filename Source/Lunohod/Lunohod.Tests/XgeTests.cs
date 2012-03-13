using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Lunohod.Objects;
using System.Drawing;
using Lunohod.Xge;
using Microsoft.Xna.Framework;

namespace Lunohod.Tests
{
    [TestFixture]
    public class XgeTests
    {
        private XLevel level;
        private XBlock block;
        private XSystem system;

        [SetUp]
        public void Init()
        {
            Compiler.Instance.VariableStorage.Clear();

            level = new XLevel() { Id = "lvlTest" };
            system = new XSystem { Id="system" };
            block = new XBlock() { Id = "blkBlock01", Bounds = new RectangleF(10, 10, 20, 20) };


            level.Subcomponents = new XObjectCollection();
            level.Subcomponents.Add(system);
            level.Subcomponents.Add(block);
        }

        [Test]
        public void SimpleExpressions()
        {
            Assert.AreEqual(5, Compiler.CompileExpression<float>(level, "1 + 2 * 2").GetValue());
            Assert.AreEqual(5, Compiler.CompileExpression<float>(level, "10 % 3 + 2 * 2 + (10 - 2 * 5)").GetValue());

            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "6 > 5").GetValue());
            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "6 >= 6").GetValue());
            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "6 == 6").GetValue());
            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "6 <= 6").GetValue());
            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "6 < 7").GetValue());

            Assert.AreEqual("abc123", Compiler.CompileExpression<string>(level, "'abc' + '123'").GetValue());
        }

        [Test]
        public void Variables()
        {
            Compiler.CompileStatements(block, "@X1 = 60; @X2 = 120; X = 84;").ForEach(a => a.Call());

            Compiler.CompileStatements(level, "@a=10").ForEach(a => a.Call());
            Assert.IsTrue(Compiler.Instance.VariableStorage.ContainsKey("a"));
            Assert.AreEqual(10, Compiler.CompileExpression<float>(level, "@a").GetValue());

            Compiler.CompileStatements(level, "@b='abc'").ForEach(a => a.Call());
            Assert.IsTrue(Compiler.Instance.VariableStorage.ContainsKey("b"));
            Assert.AreEqual("abc", Compiler.CompileExpression<string>(level, "@b").GetValue());

            Compiler.CompileStatements(level, "@c=true").ForEach(a => a.Call());
            Assert.IsTrue(Compiler.Instance.VariableStorage.ContainsKey("c"));
            Assert.AreEqual(true, Compiler.CompileExpression<bool>(level, "@c").GetValue());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void VariableTypeChecking()
        {
            Compiler.CompileStatements(level, "@a=10").ForEach(a => a.Call());
            Compiler.CompileStatements(level, "@a='abc'").ForEach(a => a.Call());
            Compiler.CompileExpression<float>(level, "@a").GetValue();
        }

        [Test]
        public void AssignStatements()
        {
            Compiler.CompileStatements(level, "@a=10;@b='123' + system.Str(456);@c=@a>3;" + block.Id + ".X = 777;").ForEach(a => a.Call());

            Assert.AreEqual(10, Compiler.CompileExpression<float>(level, "@a").GetValue());
            Assert.AreEqual("123456", Compiler.CompileExpression<string>(level, "@b").GetValue());
            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "@c").GetValue());
            Assert.AreEqual(777, block.X);
        }

        [Test]
        public void Flags()
        {
            GameEngine game = new GameEngine();
            game.CurrentUpdateTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero);
            game.ScreenEngines.Add(new ScreenEngine(game, ""));

            Compiler.CompileStatements(level, block.Id + ":broke").ForEach(a => a.Call());
            Assert.AreEqual(1, game.EventQueue.Count);

            game.ProcessQueue(new GameTime(TimeSpan.Zero, TimeSpan.Zero));

            Assert.AreEqual(1, game.ScreenEngine.CurrentEvents.Count);
        }
    }
}
