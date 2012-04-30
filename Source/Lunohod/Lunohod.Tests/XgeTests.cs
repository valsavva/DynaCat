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
        private XNumAnimation numAnimation;
        private XBoolTrigger boolTrigger;

        [SetUp]
        public void Init()
        {
            Compiler.Instance.VariableStorage.Clear();

            level = new XLevel() { Id = "lvlTest", Subcomponents = new XObjectCollection() };
            system = new XSystem { Id="system" };
            block = new XBlock() { Id = "blkBlock01", Bounds = new RectangleF(10, 10, 20, 20) };
            numAnimation = new XNumAnimation() { Id = "anmNum01", Subcomponents = new XObjectCollection() };
            boolTrigger = new XBoolTrigger();

            numAnimation.Subcomponents.Add(boolTrigger);

            level.Subcomponents.Add(system);
            level.Subcomponents.Add(block);
            level.Subcomponents.Add(numAnimation);
        }

        [Test]
        public void SimpleExpressions()
        {
            Assert.AreEqual(3.7, Compiler.CompileExpression<double>(level, "4.2 - 0.5").GetValue());
            Assert.AreEqual(5, Compiler.CompileExpression<double>(level, "1 + 2 * 2").GetValue());
            Assert.AreEqual(5, Compiler.CompileExpression<double>(level, "10 % 3 + 2 * 2 + (10 - 2 * 5)").GetValue());

            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "6 > 5").GetValue());
            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "6 >= 6").GetValue());
            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "6 == 6").GetValue());
            Assert.IsFalse(Compiler.CompileExpression<bool>(level, "5 == 6").GetValue());
            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "5 != 6").GetValue());
            Assert.IsFalse(Compiler.CompileExpression<bool>(level, "6 != 6").GetValue());
            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "6 <= 6").GetValue());
            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "6 < 7").GetValue());

            Assert.AreEqual("abc123", Compiler.CompileExpression<string>(level, "'abc' + '123'").GetValue());
        }

        [Test]
        public void BooleanLogic()
        {
            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "true and true").GetValue());
            Assert.IsFalse(Compiler.CompileExpression<bool>(level, "true and false").GetValue());
            Assert.IsFalse(Compiler.CompileExpression<bool>(level, "false and true").GetValue());
            Assert.IsFalse(Compiler.CompileExpression<bool>(level, "false and false").GetValue());

            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "true or true").GetValue());
            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "true or false").GetValue());
            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "false or true").GetValue());
            Assert.IsFalse(Compiler.CompileExpression<bool>(level, "false or false").GetValue());

            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "5 < 6 and not (5 == 6)").GetValue());

            Assert.IsFalse(Compiler.CompileExpression<bool>(level, "anmNum01.InProgress").GetValue());
        }

        [Test]
        [ExpectedException]
        public void UnaryNotAndWrongOperand()
        {
            Compiler.CompileExpression<bool>(level, "5 < 6 and not 5 == 6").GetValue();
        }

        [Test]
        public void Variables()
        {
            Compiler.CompileStatements(boolTrigger, "Start()");

            Compiler.CompileStatements(block, "@X1 = 60; @X2 = 120; X = 84;").ForEach(a => a.Call());

            Compiler.CompileStatements(level, "@a=10").ForEach(a => a.Call());
            Assert.IsTrue(Compiler.Instance.VariableStorage.ContainsKey("a"));
            Assert.AreEqual(10, Compiler.CompileExpression<double>(level, "@a").GetValue());

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
            Compiler.CompileExpression<double>(level, "@a").GetValue();
        }

        [Test]
        public void AssignStatements()
        {
            Compiler.CompileStatements(level, block.Id + ".X = 777;");
            Compiler.CompileStatements(level, "@a=10;@b='123' + system.Str(456);@c=@a>3;" + block.Id + ".X = 777;").ForEach(a => a.Call());

            Assert.AreEqual(10, Compiler.CompileExpression<double>(level, "@a").GetValue());
            Assert.AreEqual("123456", Compiler.CompileExpression<string>(level, "@b").GetValue());
            Assert.IsTrue(Compiler.CompileExpression<bool>(level, "@c").GetValue());
            Assert.AreEqual(777, block.X);
        }

        [Test]
        public void Flags()
        {
            GameEngine game = new GameEngine();
            game.CurrentUpdateTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero);
            game.ScreenEngines.Add(new ScreenEngine(game, "", null));

            Compiler.CompileStatements(level, block.Id + ":broke").ForEach(a => a.Call());
            Assert.AreEqual(1, game.EventQueue.Count);

            game.ProcessQueue(new GameTime(TimeSpan.Zero, TimeSpan.Zero));

            Assert.AreEqual(1, game.ScreenEngine.CurrentEvents.Count);
        }
    }
}
