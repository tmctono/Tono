// Copyright (c) Manabu Tonosaki All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Tono;
using Tono.Jit;

namespace UnitTestProject1
{
    [TestClass]
    public class TonoJit_JaC_Template
    {
        [TestMethod]
        public void Test01()
        {
            var c = @"
                te = new Template
                    Block
                        add 'st = new Stage'
                    Name = 'MyTemp'
            ";
            var jac = new JacInterpreter();
            jac.Exec(c);
            Assert.AreEqual(jac["te"].GetType(), typeof(JitTemplate));
            Assert.IsNotNull(jac.Template("te"));
            Assert.AreEqual(jac.Template("te").Name, "MyTemp");
        }


        [TestMethod]
        public void Test02()
        {
            var c = @"
                te = new Template
                    Block
                        add 'st = new Stage'
                        add 'p1 = new Process'
                        add 'w1 = new Work'
                        add 'k1 = new Kanban'
            ";
            var jac = new JacInterpreter();
            jac.Exec(c);
            var te = jac.Template("te");
            Assert.IsNotNull(te);
            Assert.AreEqual(te.Count, 4);

            c = @"
                'te'
                    Block
                        add 'w2 = new Work'
                        add 'w3 = new Work'
                        add 'w4 = new Work'
            ";
            jac.Exec(c);
            Assert.AreEqual(te.Count, 7);

            c = @"
                'te'
                    Block
                        remove '::LAST::'
            ";
            jac.Exec(c);
            Assert.AreEqual(te.Count, 6);
        }

        [TestMethod]
        public void Test03()
        {
            var c = @"
                te = new Template
                    Block
                        add 'st = new Stage'
                        add 'p1 = new Process'
                        add 'w1 = new Work'
                        add 'k1 = new Kanban'
            ";
            var jac = new JacInterpreter();
            jac.Exec(c);

            var jac2 = JacInterpreter.From(jac.Template("te"));
            Assert.IsNotNull(jac2.Stage("st"));
            Assert.IsNotNull(jac2.Process("p1"));
            Assert.IsNotNull(jac2.Work("w1"));
            Assert.IsNotNull(jac2.Kanban("k1"));
            Assert.IsNull(jac2.Template("te")); // child JacInterpreter should has NOT the template instance
        }
    }
}
