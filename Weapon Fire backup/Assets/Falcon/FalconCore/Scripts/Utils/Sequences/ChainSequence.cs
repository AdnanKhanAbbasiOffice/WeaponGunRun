using System.Collections;
using Falcon.FalconCore.Scripts.Utils.Sequences.Core;

namespace Falcon.FalconCore.Scripts.Utils.Sequences
{
	public class ChainSequence : Sequence
	{
		private readonly Sequence[] sequences;
		private Sequence runningSequence;

		public ChainSequence( params Sequence[] sequences )
		{
			this.sequences = sequences;
		}

		protected override void OnException(System.Exception e)
		{
			runningSequence?.Cancel();
		}

		protected override IEnumerator Enumerator()
		{
			foreach (Sequence sequence in sequences)
			{
				yield return runningSequence = sequence;
				if (runningSequence.Failed)
				{
					throw runningSequence.Exception;
				}
			}
		}
	}
}