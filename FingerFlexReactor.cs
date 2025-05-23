using System;
using UnityEngine;

// Token: 0x02000542 RID: 1346
public class FingerFlexReactor : MonoBehaviour
{
	// Token: 0x06002099 RID: 8345 RVA: 0x000A3654 File Offset: 0x000A1854
	private void Setup()
	{
		this._rig = base.GetComponentInParent<VRRig>();
		if (!this._rig)
		{
			return;
		}
		this._fingers = new VRMap[]
		{
			this._rig.leftThumb,
			this._rig.leftIndex,
			this._rig.leftMiddle,
			this._rig.rightThumb,
			this._rig.rightIndex,
			this._rig.rightMiddle
		};
	}

	// Token: 0x0600209A RID: 8346 RVA: 0x000A36DB File Offset: 0x000A18DB
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x0600209B RID: 8347 RVA: 0x000A36E3 File Offset: 0x000A18E3
	private void FixedUpdate()
	{
		this.UpdateBlendShapes();
	}

	// Token: 0x0600209C RID: 8348 RVA: 0x000A36EC File Offset: 0x000A18EC
	public void UpdateBlendShapes()
	{
		if (!this._rig)
		{
			return;
		}
		if (this._blendShapeTargets == null || this._fingers == null)
		{
			return;
		}
		if (this._blendShapeTargets.Length == 0 || this._fingers.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this._blendShapeTargets.Length; i++)
		{
			FingerFlexReactor.BlendShapeTarget blendShapeTarget = this._blendShapeTargets[i];
			if (blendShapeTarget != null)
			{
				int sourceFinger = (int)blendShapeTarget.sourceFinger;
				if (sourceFinger != -1)
				{
					SkinnedMeshRenderer targetRenderer = blendShapeTarget.targetRenderer;
					if (targetRenderer)
					{
						float lerpValue = FingerFlexReactor.GetLerpValue(this._fingers[sourceFinger]);
						Vector2 inputRange = blendShapeTarget.inputRange;
						Vector2 outputRange = blendShapeTarget.outputRange;
						float num = MathUtils.Linear(lerpValue, inputRange.x, inputRange.y, outputRange.x, outputRange.y);
						blendShapeTarget.currentValue = num;
						targetRenderer.SetBlendShapeWeight(blendShapeTarget.blendShapeIndex, num);
					}
				}
			}
		}
	}

	// Token: 0x0600209D RID: 8349 RVA: 0x000A37C0 File Offset: 0x000A19C0
	private static float GetLerpValue(VRMap map)
	{
		VRMapThumb vrmapThumb = map as VRMapThumb;
		float num;
		if (vrmapThumb == null)
		{
			VRMapIndex vrmapIndex = map as VRMapIndex;
			if (vrmapIndex == null)
			{
				VRMapMiddle vrmapMiddle = map as VRMapMiddle;
				if (vrmapMiddle == null)
				{
					num = 0f;
				}
				else
				{
					num = vrmapMiddle.calcT;
				}
			}
			else
			{
				num = vrmapIndex.calcT;
			}
		}
		else
		{
			num = ((vrmapThumb.calcT > 0.1f) ? 1f : 0f);
		}
		return num;
	}

	// Token: 0x04002494 RID: 9364
	[SerializeField]
	private VRRig _rig;

	// Token: 0x04002495 RID: 9365
	[SerializeField]
	private VRMap[] _fingers = new VRMap[0];

	// Token: 0x04002496 RID: 9366
	[SerializeField]
	private FingerFlexReactor.BlendShapeTarget[] _blendShapeTargets = new FingerFlexReactor.BlendShapeTarget[0];

	// Token: 0x02000543 RID: 1347
	[Serializable]
	public class BlendShapeTarget
	{
		// Token: 0x04002497 RID: 9367
		public FingerFlexReactor.FingerMap sourceFinger;

		// Token: 0x04002498 RID: 9368
		public SkinnedMeshRenderer targetRenderer;

		// Token: 0x04002499 RID: 9369
		public int blendShapeIndex;

		// Token: 0x0400249A RID: 9370
		public Vector2 inputRange = new Vector2(0f, 1f);

		// Token: 0x0400249B RID: 9371
		public Vector2 outputRange = new Vector2(0f, 1f);

		// Token: 0x0400249C RID: 9372
		[NonSerialized]
		public float currentValue;
	}

	// Token: 0x02000544 RID: 1348
	public enum FingerMap
	{
		// Token: 0x0400249E RID: 9374
		None = -1,
		// Token: 0x0400249F RID: 9375
		LeftThumb,
		// Token: 0x040024A0 RID: 9376
		LeftIndex,
		// Token: 0x040024A1 RID: 9377
		LeftMiddle,
		// Token: 0x040024A2 RID: 9378
		RightThumb,
		// Token: 0x040024A3 RID: 9379
		RightIndex,
		// Token: 0x040024A4 RID: 9380
		RightMiddle
	}
}
