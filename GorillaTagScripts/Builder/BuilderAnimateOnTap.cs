using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B4E RID: 2894
	public class BuilderAnimateOnTap : BuilderPieceTappable
	{
		// Token: 0x0600475D RID: 18269 RVA: 0x001532FF File Offset: 0x001514FF
		public override void OnTapReplicated()
		{
			this.anim.Rewind();
			this.anim.Play();
		}

		// Token: 0x040049AF RID: 18863
		[SerializeField]
		private Animation anim;
	}
}
