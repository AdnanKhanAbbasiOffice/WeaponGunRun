using System;
using System.Collections;
using System.Collections.Generic;
using Object = System.Object;

namespace Falcon.FalconCore.Scripts.Utils.Sequences.Core
{
    public class SeqWrap<T> : Sequence<T> where T : class
    {
        private readonly Action<Exception> ifDropOut;
        private readonly IEnumerator<T> enumerator;

        public SeqWrap(IEnumerator<T> enumerator, Action<Exception> ifDropOut = null)
        {
            this.enumerator = enumerator;
            this.ifDropOut = ifDropOut ?? (e => base.OnException(e));
        }

        protected override IEnumerator<T> EnumeratorT()
        {
            return enumerator;
        }

        protected override void OnException(Exception e)
        {
            ifDropOut.Invoke(e);
        }
    }
    
    public class SequenceWrap : SeqWrap<Object>
    {
        public SequenceWrap(IEnumerator enumerator, Action<Exception> ifDropOut = null) : base((IEnumerator<object>)enumerator, ifDropOut)
        {
        }
    }
}