using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200068D RID: 1677
public class SizeManager : MonoBehaviour
{
	// Token: 0x1700040E RID: 1038
	// (get) Token: 0x060029EE RID: 10734 RVA: 0x000CF604 File Offset: 0x000CD804
	public float currentScale
	{
		get
		{
			if (this.targetRig != null)
			{
				return this.targetRig.ScaleMultiplier;
			}
			if (this.targetPlayer != null)
			{
				return this.targetPlayer.ScaleMultiplier;
			}
			return 1f;
		}
	}

	// Token: 0x1700040F RID: 1039
	// (get) Token: 0x060029EF RID: 10735 RVA: 0x000CF63F File Offset: 0x000CD83F
	// (set) Token: 0x060029F0 RID: 10736 RVA: 0x000CF674 File Offset: 0x000CD874
	public int currentSizeLayerMaskValue
	{
		get
		{
			if (this.targetPlayer)
			{
				return this.targetPlayer.sizeLayerMask;
			}
			if (this.targetRig)
			{
				return this.targetRig.SizeLayerMask;
			}
			return 1;
		}
		set
		{
			if (this.targetPlayer)
			{
				this.targetPlayer.sizeLayerMask = value;
				if (this.targetRig != null)
				{
					this.targetRig.SizeLayerMask = value;
					return;
				}
			}
			else if (this.targetRig)
			{
				this.targetRig.SizeLayerMask = value;
			}
		}
	}

	// Token: 0x060029F1 RID: 10737 RVA: 0x000CF6CE File Offset: 0x000CD8CE
	private void OnDisable()
	{
		this.touchingChangers.Clear();
		this.currentSizeLayerMaskValue = 1;
		SizeManagerManager.UnregisterSM(this);
	}

	// Token: 0x060029F2 RID: 10738 RVA: 0x000CF6E8 File Offset: 0x000CD8E8
	private void OnEnable()
	{
		SizeManagerManager.RegisterSM(this);
	}

	// Token: 0x060029F3 RID: 10739 RVA: 0x000CF6F0 File Offset: 0x000CD8F0
	private void CollectLineRenderers(GameObject obj)
	{
		this.lineRenderers = obj.GetComponentsInChildren<LineRenderer>(true);
		int num = this.lineRenderers.Length;
		foreach (LineRenderer lineRenderer in this.lineRenderers)
		{
			this.initLineScalar.Add(lineRenderer.widthMultiplier);
		}
	}

	// Token: 0x060029F4 RID: 10740 RVA: 0x000CF740 File Offset: 0x000CD940
	public void BuildInitialize()
	{
		this.rate = 650f;
		if (this.targetRig != null)
		{
			this.CollectLineRenderers(this.targetRig.gameObject);
		}
		else if (this.targetPlayer != null)
		{
			this.CollectLineRenderers(GorillaTagger.Instance.offlineVRRig.gameObject);
		}
		this.mainCameraTransform = Camera.main.transform;
		if (this.targetPlayer != null)
		{
			this.myType = SizeManager.SizeChangerType.LocalOffline;
		}
		else if (this.targetRig != null && !this.targetRig.isOfflineVRRig && this.targetRig.netView != null && this.targetRig.netView.Owner != NetworkSystem.Instance.LocalPlayer)
		{
			this.myType = SizeManager.SizeChangerType.OtherOnline;
		}
		else
		{
			this.myType = SizeManager.SizeChangerType.LocalOnline;
		}
		this.buildInitialized = true;
	}

	// Token: 0x060029F5 RID: 10741 RVA: 0x000CF824 File Offset: 0x000CDA24
	private void Awake()
	{
		if (!this.buildInitialized)
		{
			this.BuildInitialize();
		}
		SizeManagerManager.RegisterSM(this);
	}

	// Token: 0x060029F6 RID: 10742 RVA: 0x000CF83C File Offset: 0x000CDA3C
	public void InvokeFixedUpdate()
	{
		float num = 1f;
		SizeChanger sizeChanger = this.ControllingChanger(this.targetRig.transform);
		switch (this.myType)
		{
		case SizeManager.SizeChangerType.LocalOffline:
			num = this.ScaleFromChanger(sizeChanger, this.mainCameraTransform, Time.fixedDeltaTime);
			this.targetPlayer.SetScaleMultiplier((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
			break;
		case SizeManager.SizeChangerType.LocalOnline:
			num = this.ScaleFromChanger(sizeChanger, this.targetRig.transform, Time.fixedDeltaTime);
			this.targetRig.ScaleMultiplier = ((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
			break;
		case SizeManager.SizeChangerType.OtherOnline:
			num = this.ScaleFromChanger(sizeChanger, this.targetRig.transform, Time.fixedDeltaTime);
			this.targetRig.ScaleMultiplier = ((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
			break;
		}
		if (num != this.lastScale)
		{
			for (int i = 0; i < this.lineRenderers.Length; i++)
			{
				this.lineRenderers[i].widthMultiplier = num * this.initLineScalar[i];
			}
			Vector3 vector;
			if (sizeChanger != null && sizeChanger.TryGetScaleCenterPoint(out vector))
			{
				if (this.myType == SizeManager.SizeChangerType.LocalOffline)
				{
					this.targetPlayer.ScaleAwayFromPoint(this.lastScale, num, vector);
				}
				else if (this.myType == SizeManager.SizeChangerType.LocalOnline)
				{
					GTPlayer.Instance.ScaleAwayFromPoint(this.lastScale, num, vector);
				}
			}
			if (this.myType == SizeManager.SizeChangerType.LocalOffline)
			{
				this.CheckSizeChangeEvents(num);
			}
		}
		this.lastScale = num;
	}

	// Token: 0x060029F7 RID: 10743 RVA: 0x000CF9DC File Offset: 0x000CDBDC
	private SizeChanger ControllingChanger(Transform t)
	{
		for (int i = this.touchingChangers.Count - 1; i >= 0; i--)
		{
			SizeChanger sizeChanger = this.touchingChangers[i];
			if (!(sizeChanger == null) && sizeChanger.gameObject.activeInHierarchy && (sizeChanger.SizeLayerMask & this.currentSizeLayerMaskValue) != 0 && (sizeChanger.alwaysControlWhenEntered || (sizeChanger.ClosestPoint(t.position) - t.position).magnitude < this.magnitudeThreshold))
			{
				return sizeChanger;
			}
		}
		return null;
	}

	// Token: 0x060029F8 RID: 10744 RVA: 0x000CFA68 File Offset: 0x000CDC68
	private float ScaleFromChanger(SizeChanger sC, Transform t, float deltaTime)
	{
		if (sC == null)
		{
			return 1f;
		}
		SizeChanger.ChangerType changerType = sC.MyType;
		if (changerType == SizeChanger.ChangerType.Static)
		{
			return this.SizeOverTime(sC.MinScale, sC.StaticEasing, deltaTime);
		}
		if (changerType == SizeChanger.ChangerType.Continuous)
		{
			Vector3 vector = Vector3.Project(t.position - sC.StartPos.position, sC.EndPos.position - sC.StartPos.position);
			return Mathf.Clamp(sC.MaxScale - vector.magnitude / (sC.StartPos.position - sC.EndPos.position).magnitude * (sC.MaxScale - sC.MinScale), sC.MinScale, sC.MaxScale);
		}
		return 1f;
	}

	// Token: 0x060029F9 RID: 10745 RVA: 0x000CFB3A File Offset: 0x000CDD3A
	private float SizeOverTime(float targetSize, float easing, float deltaTime)
	{
		if (easing <= 0f || Mathf.Abs(this.targetRig.ScaleMultiplier - targetSize) < 0.05f)
		{
			return targetSize;
		}
		return Mathf.MoveTowards(this.targetRig.ScaleMultiplier, targetSize, deltaTime / easing);
	}

	// Token: 0x060029FA RID: 10746 RVA: 0x000CFB74 File Offset: 0x000CDD74
	private void CheckSizeChangeEvents(float newSize)
	{
		if (newSize < this.smallThreshold)
		{
			if (!this.isSmall)
			{
				this.isSmall = true;
				this.isLarge = false;
				PlayerGameEvents.MiscEvent("SizeSmall");
				return;
			}
		}
		else if (newSize > this.largeThreshold)
		{
			if (!this.isLarge)
			{
				this.isLarge = true;
				this.isSmall = false;
				PlayerGameEvents.MiscEvent("SizeLarge");
				return;
			}
		}
		else
		{
			this.isLarge = false;
			this.isSmall = false;
		}
	}

	// Token: 0x04002F0C RID: 12044
	public List<SizeChanger> touchingChangers;

	// Token: 0x04002F0D RID: 12045
	private LineRenderer[] lineRenderers;

	// Token: 0x04002F0E RID: 12046
	private List<float> initLineScalar = new List<float>();

	// Token: 0x04002F0F RID: 12047
	public VRRig targetRig;

	// Token: 0x04002F10 RID: 12048
	public GTPlayer targetPlayer;

	// Token: 0x04002F11 RID: 12049
	public float magnitudeThreshold = 0.01f;

	// Token: 0x04002F12 RID: 12050
	public float rate = 650f;

	// Token: 0x04002F13 RID: 12051
	public Transform mainCameraTransform;

	// Token: 0x04002F14 RID: 12052
	public SizeManager.SizeChangerType myType;

	// Token: 0x04002F15 RID: 12053
	public float lastScale;

	// Token: 0x04002F16 RID: 12054
	private bool buildInitialized;

	// Token: 0x04002F17 RID: 12055
	private const float returnToNormalEasing = 0.33f;

	// Token: 0x04002F18 RID: 12056
	private float smallThreshold = 0.6f;

	// Token: 0x04002F19 RID: 12057
	private float largeThreshold = 1.5f;

	// Token: 0x04002F1A RID: 12058
	private bool isSmall;

	// Token: 0x04002F1B RID: 12059
	private bool isLarge;

	// Token: 0x0200068E RID: 1678
	public enum SizeChangerType
	{
		// Token: 0x04002F1D RID: 12061
		LocalOffline,
		// Token: 0x04002F1E RID: 12062
		LocalOnline,
		// Token: 0x04002F1F RID: 12063
		OtherOnline
	}
}
