using System;
using System.Collections;
using System.Collections.Generic;
using Falcon.FalconCore.Scripts.Utils.Logs;
using UnityEngine;

namespace Falcon.FalconCore.Scripts.Utils.Sequences.Core
{
    public abstract class Sequence<T> : Sequence, IEnumerator<T> where T : class
    {
        /// <summary>
        /// If not gotten anything, throw current Exception; if current Exception is null, throw Exception
        /// </summary>
        public new T Current
        {
            get
            {
                CheckException();
                
                T result = null;
                
                if (base.Current is T) {
                    result = (T)base.Current;
                }
                else if (base.Current != null)
                {
                    result = (T)Convert.ChangeType(base.Current, typeof(T),
                        System.Globalization.CultureInfo.InstalledUICulture.NumberFormat);
                }
                
                return result;
            }
        }

        private void CheckException()
        {
            if (Exception != null) throw Exception;
        }

        protected sealed override IEnumerator Enumerator() => EnumeratorT();

        protected abstract IEnumerator<T> EnumeratorT();


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            Reset();
        }
    }

    public abstract class Sequence : ISequence
    {

        #region Execution

        private Stack<IEnumerator> enumerators;

        private Stack<IEnumerator> Enumerators
        {
            get { return enumerators = enumerators ?? new Stack<IEnumerator>(new[] { Enumerator() }); }
        }

        protected abstract IEnumerator Enumerator();

        public virtual bool MoveNext()
        {
            if (Done) return false;
            try
            {
                var peek = Enumerators.Peek();
                if (peek != null)
                {
                    if (peek.MoveNext())
                    {
                        CheckCurrent(peek.Current);
                    }
                    else
                    {
                        Sequence subSequence = peek as Sequence;
                        if (subSequence != null && subSequence.Failed)
                        {
                            OnException(subSequence.Exception);
                        } 
                        Enumerators.Pop();
                    }
                }
                else
                {
                    Enumerators.Pop();
                }
            }
            catch (Exception e)
            {
                OnException(e);
            }
            return true;
        }

        private void CheckCurrent(object current)
        {
            if (Enumerators.Count == 1) Current = current;
            var enumerator = current as IEnumerator;
            if (enumerator != null)
                Enumerators.Push(enumerator);
            
            else
            {
                var yieldInstruction = current as YieldInstruction;
                if (yieldInstruction != null)
                    Enumerators.Push(new YieldInstructionSequence(yieldInstruction));
            }
        }

        public object Current { get; private set; }
        
        public void Reset()
        {
            enumerators = null;
        }

        #endregion

        #region Expand Methods

        public Exception Exception { get; private set; }

        public bool Failed => Exception != null;

        public bool Done => Exception != null || Enumerators.Count == 0;

        public void Cancel()
        {
            if (!Done)
            {
                OnException(new SequenceCancelException());
            }
        }

        protected virtual void OnException(Exception e)
        {
            Exception = new SequenceCancelException();
            CoreLogger.Instance.Warning("Action cancelled : " + e);
        }

        public bool TryContinue()
        {
            if (Exception is SequenceCancelException)
            {
                Exception = null;
                return true;
            }

            return false;
        }

        public IEnumerator Wait()
        {
            while (!Done) yield return null;
        }

        #endregion

        #region EqualAndHashCode

        private readonly Guid guid = Guid.NewGuid();

        private bool Equals(Sequence other)
        {
            return guid.Equals(other.guid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Sequence)obj);
        }

        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }

        #endregion
    }
}