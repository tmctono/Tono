// (c) 2019 Manabu Tonosaki
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Tono;
using Tono.Jit;

namespace Tono.Jit
{
    [TestClass]
    public class TonoJit_NestStage
    {
        [TestMethod]
        public void Test001()
        {
            // Unit testing to confirm a process instance used two stage can treat indipendently.
            // １つのProcessインスタンスが、２つのStageにまたがる場合でも、それぞれのStageで処理ができる事。

            var st1 = new JitStage();
            var st2 = new JitStage();
            var tp = new Dictionary<JitStage, int>
            {
                [st1] = 8,
                [st2] = 20, // start time for each stage
            };
            var t8 = new Dictionary<JitStage, string>
            {
                [st1] = "8",
                [st2] = "20",
            };
            var t9 = new Dictionary<JitStage, string>
            {
                [st1] = "9",
                [st2] = "21",
            };
            var tw = new Dictionary<(JitStage Stage, string Name), JitWork>();

            //----------------------------------------------

            var A = new JitProcess  // 前工程
            {
                Name = "A",
            };
            var B = new JitProcess  // 次工程（合流工程）
            {
                Name = "B",
            };
            var C = new JitProcess  // 分岐工程
            {
                Name = "C",
            };
            var Y = new JitProcess  // 横工程Y
            {
                Name = "Y",
            };
            var Z = new JitProcess  // 横工程Z
            {
                Name = "Z",
            };
            var SINK = new JitProcess // 消滅工程
            {
                Name = "SINK",
            };
            var D = new JitProcess // 分岐工程
            {
                Name = "D",
            };
            A.InCommands.Add(new CiDelay
            {
                Delay = TimeSpan.FromMinutes(10),
            });
            B.InCommands.Add(new CiDelay
            {
                Delay = TimeSpan.FromMinutes(20),
            });
            B.Constraints.Add(new CoMaxCost
            {
                ReferenceVarName = JitVariable.From("Count"),
                Value = 1.0,
            });
            CoJoinFrom JFY;
            B.Constraints.Add(JFY = new CoJoinFrom
            {
                PullFromProcessKey = Y.ID,
                PorlingSpan = TimeSpan.FromMinutes(10),
            });
            CoJoinFrom JFZ;
            B.Constraints.Add(JFZ = new CoJoinFrom
            {
                PullFromProcessKey = Z.ID,
                PorlingSpan = TimeSpan.FromMinutes(10),
            });
            C.InCommands.Add(new CiPickTo  // C工程で Dに分岐
            {
                DestProcessKey = "D",
                Delay = TimeSpan.FromMinutes(1),
                TargetWorkClass = ":Sumaho",
            });
            C.InCommands.Add(new CiDelay
            {
                Delay = TimeSpan.FromMinutes(8),
            });
            D.InCommands.Add(new CiDelay
            {
                Delay = TimeSpan.FromMinutes(50),
            });
            D.Constraints.Add(new CoSpan
            {
                Span = TimeSpan.FromMinutes(3),
            });
            Y.InCommands.Add(new CiDelay
            {
                Delay = TimeSpan.FromMinutes(30),
            });
            Z.InCommands.Add(new CiDelay
            {
                Delay = TimeSpan.FromMinutes(30),
            });

            foreach (var st in new[] { st1, st2 })
            {
                st.Model.Procs.Add(A);
                st.Model.Procs.Add(B);
                st.Model.Procs.Add(C);
                st.Model.Procs.Add(Y);
                st.Model.Procs.Add(Z);
                st.Model.Procs.Add(SINK);
                st.Model.Procs.Add(D);

                // 工程間リンク
                st.Model.AddProcessLink(A, B); // A→B Push。但し、B.Co.JoinFromでJoinできるまで待つ
                st.Model.AddProcessLink(B, C); // B→C Push。分岐工程への移動
                st.Model.AddProcessLink(C, SINK); // B→SINK Push
                st.Model.AddProcessLink(D, SINK); // D→SINK Push

                var today = TimeUtil.ClearTime(DateTime.Now);  // H:M:S:MSを０にする
                st.Engine().Events.Enqueue(TimeUtil.Set(today, hour: tp[st], minute: 0), EventTypes.Out, tw[(st, "w1")] = new JitWork
                {
                    Stage = st,
                    Name = $"w1",
                    NextProcess = A,
                });
                Assert.IsTrue(tw[(st, "w1")].Is(":Work"));

                st.Engine().Events.Enqueue(TimeUtil.Set(today, hour: tp[st], minute: 0), EventTypes.Out, tw[(st, "y1")] = new JitWork
                {
                    Stage = st,
                    Name = $"y1",
                    NextProcess = Y,
                    Classes = JitVariable.ClassList.From(":iOS:Sumaho"),    // :Workに、クラス「追加」
                });

                st.Engine().Events.Enqueue(TimeUtil.Set(today, hour: tp[st], minute: 2), EventTypes.Out, tw[(st, "z1")] = new JitWork
                {
                    Stage = st,
                    Name = $"z1",
                    NextProcess = Z,
                    Classes = JitVariable.ClassList.From(":Android:Sumaho"),    // :Workに、クラス「追加」
                });
            }
            var k = 0;

            // 初期状態は時間順に並んでる
            foreach (var st in new[] { st1, st2 })
            {
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, $"{t8[st]}:00"));
                Assert.IsTrue(CMP(dat[k++], "y1", EventTypes.Out, $"{t8[st]}:00"));
                Assert.IsTrue(CMP(dat[k++], "z1", EventTypes.Out, $"{t8[st]}:02"));
            }
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "y1", EventTypes.Out, $"{t8[st]}:00"));
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.In,  $"{t8[st]}:00"));
                Assert.IsTrue(CMP(dat[k++], "z1", EventTypes.Out, $"{t8[st]}:02"));
            }
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.In,  $"{t8[st]}:00"));
                Assert.IsTrue(CMP(dat[k++], "y1", EventTypes.In,  $"{t8[st]}:00"));
                Assert.IsTrue(CMP(dat[k++], "z1", EventTypes.Out, $"{t8[st]}:02"));
            }
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "y1", EventTypes.In,  $"{t8[st]}:00"));
                Assert.IsTrue(CMP(dat[k++], "z1", EventTypes.Out, $"{t8[st]}:02"));
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, $"{t8[st]}:10", "A"));

                st.Engine().DoNext();
                dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "z1", EventTypes.Out, $"{t8[st]}:02"));
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, $"{t8[st]}:10", "A"));
            }
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "z1", EventTypes.In,  $"{t8[st]}:02"));
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, $"{t8[st]}:10", "A"));
            }
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, $"{t8[st]}:10", "A"));
                Assert.AreEqual(dat.Count, 1);
            }
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, $"{t8[st]}:20", "A"));
            }
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, $"{t8[st]}:30", "A"));
            }
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, $"{t8[st]}:40", "A"));
                Assert.AreEqual(tw[(st, "w1")].ChildWorks[JFY.ChildWorkKey], tw[(st, "y1")]);
            }
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.In, $"{t8[st]}:40", "A"));
                Assert.AreEqual(tw[(st, "w1")].ChildWorks[JFY.ChildWorkKey], tw[(st, "y1")]);
                Assert.AreEqual(tw[(st, "w1")].ChildWorks[JFZ.ChildWorkKey], tw[(st, "z1")]);
            }
            // BにInOK。B.Ci.Delay=20
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, $"{t9[st]}:00", "B"));
            }
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.In, $"{t9[st]}:00", "B"));
            }
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "y1", EventTypes.Out, $"{t9[st]}:01"));
                Assert.IsTrue(CMP(dat[k++], "z1", EventTypes.Out, $"{t9[st]}:01"));
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, $"{t9[st]}:08", "C"));
            }
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "z1", EventTypes.Out, $"{t9[st]}:01"));
                Assert.IsTrue(CMP(dat[k++], "y1", EventTypes.In, $"{t9[st]}:01"));
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, $"{t9[st]}:08", "C"));
            }
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "y1", EventTypes.In, $"{t9[st]}:01"));
                Assert.IsTrue(CMP(dat[k++], "z1", EventTypes.Out, $"{t9[st]}:04"));    // D.Co.Span制約で3秒加算された
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, $"{t9[st]}:08", "C"));
            }
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "z1", EventTypes.Out, $"{t9[st]}:04"));
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, $"{t9[st]}:08", "C"));
                Assert.IsTrue(CMP(dat[k++], "y1", EventTypes.Out, $"{t9[st]}:51", "D"));   // inされた
            }
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "z1", EventTypes.In, $"{t9[st]}:04"));
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, $"{t9[st]}:08", "C"));
                Assert.IsTrue(CMP(dat[k++], "y1", EventTypes.Out, $"{t9[st]}:51", "D"));
            }
            foreach (var st in new[] { st1, st2 })
            {
                st.Engine().DoNext();
                var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
                Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, $"{t9[st]}:08", "C"));
                Assert.IsTrue(CMP(dat[k++], "y1", EventTypes.Out, $"{t9[st]}:51", "D"));
                Assert.IsTrue(CMP(dat[k++], "z1", EventTypes.Out, $"{t9[st]}:54", "D"));
            }
        }

        [TestMethod]
        public void Test002()
        {
            // Test case come from Jit Model Class Design.pptx
            var X = new JitStageSubset();
            X.Model.AddProcessLink("A", "B");

            JitProcess A, B;
            X.Model.Procs.Add(A = new JitProcess  // First Process
            {
                Name = "A",
            });
            X.Model.Procs.Add(B = new JitProcess  // 2nd Process
            {
                Name = "B",
            });
            A.InCommands.Add(new CiDelay
            {
                Delay = TimeSpan.FromSeconds(20),
            });
            B.InCommands.Add(new CiDelay
            {
                Delay = TimeSpan.FromSeconds(15),
            });

            Assert.IsNull(X.Engine);


            var st = new JitStage();
            st.ModelsAdd(X);

            JitWork w1;
            var today = TimeUtil.ClearTime(DateTime.Now);  // H:M:S:MSを０にする
            st.Engine().Events.Enqueue(TimeUtil.Set(today, hour: 9, minute: 0), EventTypes.Out, w1 = new JitWork
            {
                Stage = st,
                Name = $"w1",
                NextProcess = A,
            });

            var k = 0;

            // 初期状態は時間順に並んでる
            var dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
            Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, "9:00"));

            st.Engine().DoNext();
            dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
            Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.In, "9:00"));

            st.Engine().DoNext();
            dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
            Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.Out, "9:20", "A"));

            st.Engine().DoNext();
            dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
            Assert.IsTrue(CMP(dat[k++], "w1", EventTypes.In, "9:20", "A"));

            st.Engine().DoNext();
            dat = st.Engine().Events.Peeks(99).ToList(); k = 0;
            Assert.AreEqual(dat.Count, 0);
        }

        private bool CMP(JitStage.WorkEventQueue.Item ei, string name, EventTypes et, string time, string procName = null)
        {
            var ts = time.Split(':');
            var h = int.Parse(ts[0]);
            var m = int.Parse(ts[1]);
            var ret = ei.Type == et && ei.DT.Hour == h && ei.DT.Minute == m;
            if (ei.Work != null)
            {
                ret &= name == ei.Work.Name;
            }
            if (ei.Kanban != null)
            {
                ret &= name == ("Kanban" + ei.Kanban.TestID.ToString());
            }
            if (procName != null)
            {
                if (ei.Work is JitWork work)
                {
                    ret &= (work.CurrentProcess?.Name ?? null) == procName;
                }
                else
                {
                    ret = false;
                }
            }
            return ret;
        }
    }
}
