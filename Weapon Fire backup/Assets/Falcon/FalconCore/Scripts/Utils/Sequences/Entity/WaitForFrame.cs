using System.Collections;
using Falcon.FalconCore.Scripts.Utils.Sequences.Core;

namespace Falcon.FalconCore.Scripts.Utils.Sequences.Entity
{
	public class WaitForFrame : Sequence
	{
		private readonly int frames;
		public WaitForFrame( int frames )
		{
			this.frames = frames;
		}
		protected override IEnumerator Enumerator()
		{
			var count = 0;
			while( count < frames )
			{
				count++;
				yield return null;
			}
		}
	}
}