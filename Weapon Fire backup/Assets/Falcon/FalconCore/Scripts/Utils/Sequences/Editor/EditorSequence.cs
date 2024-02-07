using System;
using System.Collections;
using System.Collections.Generic;
using Falcon.FalconCore.Scripts.Utils.Logs;
using Falcon.FalconCore.Scripts.Utils.Sequences.Core;
using UnityEditor;

namespace Falcon.FalconCore.Scripts.Utils.Sequences.Editor
{
    internal static class EditorSequenceManager
    {
        private static List<Sequence> _editorSequences;

        internal static List<Sequence> EditorSequences
        {
            get
            {
                if (_editorSequences == null)
                {
                    _editorSequences = new List<Sequence>();
                    EditorApplication.update += () =>
                    {
                        for (var i = _editorSequences.Count - 1; i >= 0; i--)
                            if (!_editorSequences[i].MoveNext())
                                _editorSequences.RemoveAt(i);
                    };
                }

                return _editorSequences;
            }
        }
    }
    public class EditorSequence<T> : Sequence<T> where T : class
    {
        private readonly IEnumerator<T> enumerator;


        private readonly Action<Exception> ifDropOut;

        public EditorSequence(IEnumerator<T> enumerator, Action<Exception> ifDropOut = null)
        {
            this.enumerator = enumerator;
            this.ifDropOut = ifDropOut ?? (e => CoreLogger.Instance.Info("Action dropped : " + e));
        }

        protected override void OnException(Exception e)
        {
            ifDropOut.Invoke(e);
        }

        protected override IEnumerator<T> EnumeratorT()
        {
            return enumerator;
        }

        public void Start()
        {
            EditorSequenceManager.EditorSequences.Add(this);
        }
    }


    public class EditorSequence : EditorSequence<object>
    {
        public EditorSequence(IEnumerator enumerator, Action<Exception> ifDropOut = null) : base(
            (IEnumerator<object>)enumerator, ifDropOut)
        {
        }
    }
}