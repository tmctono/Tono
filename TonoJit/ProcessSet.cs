﻿// (c) 2019 Manabu Tonosaki
// Licensed under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProcessKey = System.String;

namespace Tono.Jit
{
    /// <summary>
    /// Process Collection
    /// </summary>
    public class ProcessSet : IEnumerable<JitProcess>
    {
        private Dictionary<string, JitProcess> _idProcs = new Dictionary<string, JitProcess>();
        private readonly List<JitProcess> _procs = new List<JitProcess>();

        /// <summary>
        /// process count
        /// </summary>
        public int Count => _procs.Count;

        /// <summary>
        /// Add Process
        /// </summary>
        /// <param name="proc"></param>
        public void Add(JitProcess proc)
        {
            _idProcs[proc.ID] = proc;
            _procs.Add(proc);
        }

        /// <summary>
        /// Add Range
        /// </summary>
        /// <param name="procs"></param>
        public void Add(IEnumerable<JitProcess> procs)
        {
            foreach (var proc in procs)
            {
                Add(proc);
            }
        }

        /// <summary>
        /// Remove a process
        /// </summary>
        /// <param name="proc"></param>
        public void Remove(JitProcess proc)
        {
            _idProcs.Remove(proc.ID);
            _procs.Remove(proc);
        }

        /// <summary>
        /// Find Process Key(ID or Name of lazy connection)
        /// </summary>
        /// <param name="procKey"></param>
        /// <returns></returns>
        public JitProcess Find(ProcessKey procKey)
        {
            if (_idProcs.TryGetValue(procKey, out var ret))
            {
                return ret;
            }
            else
            {
                var ret1 = _procs.Where(a => a.Name?.Equals(procKey) ?? false).FirstOrDefault();    // Concidering Process.Name is change/set later
                if (ret1 == null)
                {
                    var ret2 = _procs.Where(a => a.ID?.Equals(procKey) ?? false).FirstOrDefault();
                    return ret2;
                }
                else
                {
                    return ret1;
                }
            }
        }

        /// <summary>
        /// IEnumerable＜JitProcess＞
        /// </summary>
        /// <returns></returns>
        public IEnumerator<JitProcess> GetEnumerator()
        {
            return _procs.GetEnumerator();
        }

        /// <summary>
        /// IEnumerable＜JitProcess＞
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _procs.GetEnumerator();
        }

        /// <summary>
        /// get process by Name/ID
        /// 名前で子プロセスを検索。遅延評価はこのタイミングで行ったものを覚えておく
        /// </summary>
        /// <param name="procKey"></param>
        /// <returns></returns>
        public JitProcess this[ProcessKey procKey]
        {
            get => Find(procKey);
        }

        public JitProcess this[int index] => _procs[index];
    }
}