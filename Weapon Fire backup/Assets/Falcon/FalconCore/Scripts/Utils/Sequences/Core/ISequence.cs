using System;
using System.Collections;

namespace Falcon.FalconCore.Scripts.Utils.Sequences.Core
{
    public interface ISequence : IEnumerator
    {
        Exception Exception { get; }
        bool Failed { get; }

        bool Done { get; }
        void Cancel();
        bool TryContinue();
        IEnumerator Wait();
    }
}