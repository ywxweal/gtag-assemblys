using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000175 RID: 373
public class EqualizerAnim : MonoBehaviour
{
	// Token: 0x0600095C RID: 2396 RVA: 0x00032755 File Offset: 0x00030955
	private void Start()
	{
		this.inputColorHash = this.inputColorProperty;
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x00032768 File Offset: 0x00030968
	private void Update()
	{
		if (EqualizerAnim.thisFrame == Time.frameCount)
		{
			if (EqualizerAnim.materialsUpdatedThisFrame.Contains(this.material))
			{
				return;
			}
		}
		else
		{
			EqualizerAnim.thisFrame = Time.frameCount;
			EqualizerAnim.materialsUpdatedThisFrame.Clear();
		}
		float num = Time.time % this.loopDuration;
		this.material.SetColor(this.inputColorHash, new Color(this.redCurve.Evaluate(num), this.greenCurve.Evaluate(num), this.blueCurve.Evaluate(num)));
		EqualizerAnim.materialsUpdatedThisFrame.Add(this.material);
	}

	// Token: 0x04000B3D RID: 2877
	[SerializeField]
	private AnimationCurve redCurve;

	// Token: 0x04000B3E RID: 2878
	[SerializeField]
	private AnimationCurve greenCurve;

	// Token: 0x04000B3F RID: 2879
	[SerializeField]
	private AnimationCurve blueCurve;

	// Token: 0x04000B40 RID: 2880
	[SerializeField]
	private float loopDuration;

	// Token: 0x04000B41 RID: 2881
	[SerializeField]
	private Material material;

	// Token: 0x04000B42 RID: 2882
	[SerializeField]
	private string inputColorProperty;

	// Token: 0x04000B43 RID: 2883
	private ShaderHashId inputColorHash;

	// Token: 0x04000B44 RID: 2884
	private static int thisFrame;

	// Token: 0x04000B45 RID: 2885
	private static HashSet<Material> materialsUpdatedThisFrame = new HashSet<Material>();
}
