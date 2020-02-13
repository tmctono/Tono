﻿// (c) 2019 Manabu Tonosaki
// Licensed under the MIT license.

using System;
using System.Linq;
using ProcessKey = System.String;

namespace Tono.Jit
{
    /// <summary>
    /// in-command : request to push children works into other process
    /// 子ワークを指定工程にPUSH(要求) 
    /// </summary>
    [JacTarget(Name = "CiPickTo")]
    public class CiPickTo : CiBase
    {
        public static readonly Type Type = typeof(CiPickTo);

        /// <summary>
        /// work filter classes
        /// フィルターするクラス（デフォルト :Workは全種類の子ワークを対象）
        /// </summary>
        /// <example>
        /// TargetWorkClass = ":Sumaho"  ---  pick works that have :Sumaho class only
        /// TargetWorkClass = ":iOS:Sumaho"  --- pick works that have both :iOS and :Sumaho classes only.
        /// </example>
        public string TargetWorkClass { get; set; } = ":Work";

        /// <summary>
        /// delay time to complete push work
        /// </summary>
        public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(0);

        /// <summary>
        /// destination prosess of push operation
        /// </summary>
        public ProcessKey DestProcessKey { get; set; }

        /// <summary>
        /// in-command execute
        /// </summary>
        /// <param name="work">target work</param>
        /// <param name="now">simulation time</param>
        public override void Exec(JitWork work, DateTime now)
        {
            var childworkNames =
                from cw in work.ChildWorks
                where cw.Value.Is(TargetWorkClass)
                select cw.Key;

            foreach (string childWorkName in childworkNames.ToArray())
            {
                var childWork = work.ChildWorks[childWorkName];
                var destProc = work.Current.Subset.FindChildProcess(DestProcessKey);
                childWork.Next = new JitLocation
                {
                    Subset = work.Current.Subset,
                    Process = destProc,
                };
                childWork.Current = new JitLocation    // 子Workであった事を Process = null とする。
                {
                    Subset = work.Current.Subset,
                    Process = null,
                };
                work.ChildWorks.Remove(childWorkName);  // Remove work from child works.  子ワークから外す
                work.Engine.Events.Enqueue(now + Delay, EventTypes.Out, childWork);   // Reserve destination of push move. 次工程にPUSH予約
            }
        }
    }
}
