﻿// (c) 2019 Manabu Tonosaki
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Tono.Jit
{
    public abstract class JitProcessGroup : JitProcess
    {
        private readonly LinkedList<Func<JitProcess>> procSeq = new LinkedList<Func<JitProcess>>(); // TODO: Support lazy by Process key.
        private readonly Dictionary<string, JitProcess> nameProcMap = new Dictionary<string, JitProcess>();

        /// <summary>
        /// add child process as top priority
        /// </summary>
        /// <param name="procFunc"></param>
        public virtual void Add(Func<JitProcess> procFunc)
        {
            procSeq.AddFirst(procFunc);
        }

        /// <summary>
        /// query child processes order by priority
        /// </summary>
        public IEnumerable<JitProcess> ChildProcs
        {
            get
            {
                for (var node = procSeq.First; node != null; node = node.Next)
                {
                    yield return node.Value();
                }
            }
        }

        /// <summary>
        /// process count in this group
        /// </summary>
        public int Count => procSeq.Count;

        /// <summary>
        /// access to child process by name
        /// </summary>
        /// <param name="procName"></param>
        /// <returns></returns>
        public JitProcess this[string procName]
        {
            get
            {
                if (nameProcMap.TryGetValue(procName, out JitProcess proc))
                {
                    return proc;
                }
                else
                {
                    var ps =
                        from pf in procSeq
                        let p0 = pf()
                        where p0.Name == procName
                        select p0;
                    var p = ps.FirstOrDefault();
                    if (p != null)
                    {
                        nameProcMap[procName] = p;
                    }
                    return p;
                }
            }
        }
    }
}
