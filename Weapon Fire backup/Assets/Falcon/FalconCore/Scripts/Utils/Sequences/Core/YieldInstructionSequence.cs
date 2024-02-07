using System.Collections;
using UnityEngine;

namespace Falcon.FalconCore.Scripts.Utils.Sequences.Core
{
	public class YieldInstructionSequence : Sequence
	{
		private bool flag;
		public YieldInstructionSequence( YieldInstruction yieldInstruction )
		{
			YieldInstruction = yieldInstruction;
		}

		private YieldInstruction YieldInstruction { get; }

		private IEnumerator Coroutine()
		{
			yield return YieldInstruction;
			flag = true;
		}

		protected override IEnumerator Enumerator()
		{
			FalconMain.Instance.StartCoroutine(Coroutine());
			while (!flag)
			{
				yield return null;
			}
		}
	}
}