using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B4E RID: 2894
	public class BuilderAnimateOnTap : BuilderPieceTappable
	{
		// Token: 0x0600475E RID: 18270 RVA: 0x001533D7 File Offset: 0x001515D7
		public override void OnTapReplicated()
		{
			this.anim.Rewind();
			this.anim.Play();
		}

		// Token: 0x040049B0 RID: 18864
		[SerializeField]
		private Animation anim;
	}
}
