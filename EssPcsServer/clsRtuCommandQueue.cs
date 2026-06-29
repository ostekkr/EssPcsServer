using EssPcsService;
using System;
using System.Collections.Generic;

namespace EssPcsServer
{
    /// <summary>
    /// Thread-safe command queue for RTU client messages.
    /// Encapsulates the List + lock that was previously scattered across clsRtuClient partial files.
    /// </summary>
    public class clsRtuCommandQueue
    {
        private readonly List<clsEssPcsMsg> _list = new List<clsEssPcsMsg>();
        private readonly object _lock = new object();

        public int Count { get { lock (_lock) return _list.Count; } }

        public void Enqueue(clsEssPcsMsg msg)
        {
            lock (_lock) _list.Add(msg);
        }

        public void DequeueFirst()
        {
            lock (_lock) { if (_list.Count > 0) _list.RemoveAt(0); }
        }

        public void Clear()
        {
            lock (_lock) _list.Clear();
        }

        /// <summary>Returns a reference to the first item, or null. Caller may modify the returned object.</summary>
        public clsEssPcsMsg PeekFirst()
        {
            lock (_lock) return _list.Count > 0 ? _list[0] : null;
        }

        public void RemoveAll(Predicate<clsEssPcsMsg> match)
        {
            lock (_lock) _list.RemoveAll(match);
        }

        public void SetFinishMark(int msgType, DateTime dt)
        {
            lock (_lock)
            {
                foreach (var msg in _list)
                    if (msg.msgType == msgType && msg.msgFinish == 0)
                    { msg.msgFinish = 1; msg.msgDateTime = dt; }
            }
        }

        public void SetFinishMark(int msgType, DateTime dt, byte[] buf, int len)
        {
            lock (_lock)
            {
                foreach (var msg in _list)
                    if (msg.msgType == msgType && msg.msgFinish == 0)
                    {
                        msg.msgFinish = 1;
                        msg.msgDateTime = dt;
                        msg.msgBuf = new byte[len];
                        Array.Copy(buf, msg.msgBuf, len);
                    }
            }
        }

        /// <summary>Marks the first item matching commandType as failed (finish=1, result=-1).</summary>
        public void SetFailByCommandType(int commandType, DateTime dt)
        {
            lock (_lock)
            {
                foreach (var msg in _list)
                    if (msg.msgCommand == commandType)
                    {
                        msg.msgResult = -1;
                        msg.msgDateTime = dt;
                        msg.msgFinish = 1;
                        break;
                    }
            }
        }

        /// <summary>Marks all items matching commandType as done (finish=2).</summary>
        public void MarkDoneByCommandType(int commandType)
        {
            lock (_lock)
            {
                foreach (var msg in _list)
                    if (msg.msgCommand == commandType)
                    { msg.msgDateTime = DateTime.Now; msg.msgFinish = 2; }
            }
        }

        /// <summary>Sets finish=2 on all items that are still pending (finish==0).</summary>
        public void MarkPendingAsDone()
        {
            lock (_lock)
            {
                foreach (var msg in _list)
                    if (msg.msgFinish == 0)
                        msg.msgFinish = 2;
            }
        }
    }
}
