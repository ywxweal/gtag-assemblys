using System;
using BoingKit;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B5E RID: 2910
	public class BuilderPieceOrientUp : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x060047ED RID: 18413 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x060047EE RID: 18414 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPieceDestroy()
		{
		}

		// Token: 0x060047EF RID: 18415 RVA: 0x001574CC File Offset: 0x001556CC
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

		// Token: 0x060047F0 RID: 18416 RVA: 0x00157514 File Offset: 0x00155714
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

		// Token: 0x060047F1 RID: 18417 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPieceDeactivate()
		{
		}

		// Token: 0x04004A77 RID: 19063
		[SerializeField]
		private Transform alwaysFaceUp;
	}
}
