using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200021C RID: 540
public class SpinParametricAnimation : MonoBehaviour
{
	// Token: 0x06000C8C RID: 3212 RVA: 0x00041B0D File Offset: 0x0003FD0D
	protected void OnEnable()
	{
		this.axis = this.axis.normalized;
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x00041B20 File Offset: 0x0003FD20
	protected void LateUpdate()
	{
		Transform transform = base.transform;
		this._animationProgress = (this._animationProgress + Time.deltaTime * this.revolutionsPerSecond) % 1f;
		float num = this.timeCurve.Evaluate(this._animationProgress) * 360f;
		float num2 = num - this._oldAngle;
		this._oldAngle = num;
		if (this.WorldSpaceRotation)
		{
			transform.rotation = Quaternion.AngleAxis(num2, this.axis) * transform.rotation;
			return;
		}
		transform.localRotation = Quaternion.AngleAxis(num2, this.axis) * transform.localRotation;
	}

	// Token: 0x04000F18 RID: 3864
	[Tooltip("Axis to rotate around.")]
	public Vector3 axis = Vector3.up;

	// Token: 0x04000F19 RID: 3865
	[Tooltip("Whether rotation is in World Space or Local Space")]
	public bool WorldSpaceRotation = true;

	// Token: 0x04000F1A RID: 3866
	[FormerlySerializedAs("speed")]
	[Tooltip("Speed of rotation.")]
	public float revolutionsPerSecond = 0.25f;

	// Token: 0x04000F1B RID: 3867
	[Tooltip("Affects the progress of the animation over time.")]
	public AnimationCurve timeCurve;

	// Token: 0x04000F1C RID: 3868
	private float _animationProgress;

	// Token: 0x04000F1D RID: 3869
	private float _oldAngle;
}
