using System;
using UnityEngine;

// Token: 0x020005F6 RID: 1526
public class GorillaCaveCrystal : Tappable
{
	// Token: 0x060025A8 RID: 9640 RVA: 0x000BB6B2 File Offset: 0x000B98B2
	private void Awake()
	{
		if (this.tapScript == null)
		{
			this.tapScript = base.GetComponent<TapInnerGlow>();
		}
	}

	// Token: 0x060025A9 RID: 9641 RVA: 0x000BB6CE File Offset: 0x000B98CE
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		this._tapStrength = tapStrength;
		this.AnimateCrystal();
	}

	// Token: 0x060025AA RID: 9642 RVA: 0x000BB6DD File Offset: 0x000B98DD
	private void AnimateCrystal()
	{
		if (this.tapScript)
		{
			this.tapScript.Tap();
		}
	}

	// Token: 0x04002A34 RID: 10804
	public bool overrideSoundAndMaterial;

	// Token: 0x04002A35 RID: 10805
	public CrystalOctave octave;

	// Token: 0x04002A36 RID: 10806
	public CrystalNote note;

	// Token: 0x04002A37 RID: 10807
	[SerializeField]
	private MeshRenderer _crystalRenderer;

	// Token: 0x04002A38 RID: 10808
	public TapInnerGlow tapScript;

	// Token: 0x04002A39 RID: 10809
	[HideInInspector]
	public GorillaCaveCrystalVisuals visuals;

	// Token: 0x04002A3A RID: 10810
	[HideInInspector]
	[SerializeField]
	private AnimationCurve _lerpInCurve = AnimationCurve.Constant(0f, 1f, 1f);

	// Token: 0x04002A3B RID: 10811
	[HideInInspector]
	[SerializeField]
	private AnimationCurve _lerpOutCurve = AnimationCurve.Constant(0f, 1f, 1f);

	// Token: 0x04002A3C RID: 10812
	[HideInInspector]
	[SerializeField]
	private bool _animating;

	// Token: 0x04002A3D RID: 10813
	[HideInInspector]
	[SerializeField]
	[Range(0f, 1f)]
	private float _tapStrength = 1f;

	// Token: 0x04002A3E RID: 10814
	[NonSerialized]
	private TimeSince _timeSinceLastTap;
}
