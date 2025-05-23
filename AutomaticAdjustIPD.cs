using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020004B0 RID: 1200
public class AutomaticAdjustIPD : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06001CEE RID: 7406 RVA: 0x00017251 File Offset: 0x00015451
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001CEF RID: 7407 RVA: 0x0001725A File Offset: 0x0001545A
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001CF0 RID: 7408 RVA: 0x0008C3CC File Offset: 0x0008A5CC
	public void SliceUpdate()
	{
		if (!this.headset.isValid)
		{
			this.headset = InputDevices.GetDeviceAtXRNode(XRNode.Head);
		}
		if (this.headset.isValid && this.headset.TryGetFeatureValue(CommonUsages.leftEyePosition, out this.leftEyePosition) && this.headset.TryGetFeatureValue(CommonUsages.rightEyePosition, out this.rightEyePosition))
		{
			this.currentIPD = (this.rightEyePosition - this.leftEyePosition).magnitude;
			if (Mathf.Abs(this.lastIPD - this.currentIPD) < 0.01f)
			{
				return;
			}
			this.lastIPD = this.currentIPD;
			for (int i = 0; i < this.adjustXScaleObjects.Length; i++)
			{
				Transform transform = this.adjustXScaleObjects[i];
				if (!transform)
				{
					return;
				}
				transform.localScale = new Vector3(Mathf.LerpUnclamped(1f, 1.12f, (this.currentIPD - 0.058f) / 0.0050000027f), 1f, 1f);
			}
		}
	}

	// Token: 0x06001CF2 RID: 7410 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04002022 RID: 8226
	public InputDevice headset;

	// Token: 0x04002023 RID: 8227
	public float currentIPD;

	// Token: 0x04002024 RID: 8228
	public Vector3 leftEyePosition;

	// Token: 0x04002025 RID: 8229
	public Vector3 rightEyePosition;

	// Token: 0x04002026 RID: 8230
	public bool testOverride;

	// Token: 0x04002027 RID: 8231
	public Transform[] adjustXScaleObjects;

	// Token: 0x04002028 RID: 8232
	public float sizeAt58mm = 1f;

	// Token: 0x04002029 RID: 8233
	public float sizeAt63mm = 1.12f;

	// Token: 0x0400202A RID: 8234
	public float lastIPD;
}
