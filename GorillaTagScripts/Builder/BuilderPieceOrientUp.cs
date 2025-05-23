using System;
using BoingKit;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B5E RID: 2910
	public class BuilderPieceOrientUp : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x060047EC RID: 18412 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x060047ED RID: 18413 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPieceDestroy()
		{
		}

		// Token: 0x060047EE RID: 18414 RVA: 0x001573F4 File Offset: 0x001555F4
		public void OnPiecePlacementDeserialized()
		{
			if (this.alwaysFaceUp != null)
			{
				Quaternion quaternion;
				Quaternion quaternion2;
				QuaternionUtil.DecomposeSwingTwist(this.alwaysFaceUp.parent.rotation, Vector3.up, out quaternion, out quaternion2);
				this.alwaysFaceUp.rotation = quaternion2;
			}
		}

		// Token: 0x060047EF RID: 18415 RVA: 0x0015743C File Offset: 0x0015563C
		public void OnPieceActivate()
		{
			if (this.alwaysFaceUp != null)
			{
				Quaternion quaternion;
				Quaternion quaternion2;
				QuaternionUtil.DecomposeSwingTwist(this.alwaysFaceUp.parent.rotation, Vector3.up, out quaternion, out quaternion2);
				this.alwaysFaceUp.rotation = quaternion2;
			}
		}

		// Token: 0x060047F0 RID: 18416 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPieceDeactivate()
		{
		}

		// Token: 0x04004A76 RID: 19062
		[SerializeField]
		private Transform alwaysFaceUp;
	}
}
