using System;
using UnityEngine;

// Token: 0x02000452 RID: 1106
public class PushableSlider : MonoBehaviour
{
	// Token: 0x06001B49 RID: 6985 RVA: 0x00086770 File Offset: 0x00084970
	public void Awake()
	{
		this.localSpace = base.transform.worldToLocalMatrix;
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x06001B4A RID: 6986 RVA: 0x00086794 File Offset: 0x00084994
	private void OnTriggerStay(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent == null)
		{
			return;
		}
		Vector3 vector = this.localSpace.MultiplyPoint3x4(other.transform.position);
		Vector3 vector2 = base.transform.localPosition - this.startingPos - vector;
		float num = Mathf.Abs(vector2.x);
		if (num < this.farPushDist)
		{
			Vector3 currentVelocity = componentInParent.currentVelocity;
			if (Mathf.Sign(vector2.x) != Mathf.Sign((this.localSpace.rotation * currentVelocity).x))
			{
				return;
			}
			vector2.x = Mathf.Sign(vector2.x) * (this.farPushDist - num);
			vector2.y = 0f;
			vector2.z = 0f;
			Vector3 vector3 = base.transform.localPosition - this.startingPos + vector2;
			vector3.x = Mathf.Clamp(vector3.x, this.minXOffset, this.maxXOffset);
			base.transform.localPosition = vector3 + this.startingPos;
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x06001B4B RID: 6987 RVA: 0x000868EC File Offset: 0x00084AEC
	public void SetProgress(float value)
	{
		value = Mathf.Clamp(value, 0f, 1f);
		Vector3 vector = this.startingPos;
		vector.x += Mathf.Lerp(this.minXOffset, this.maxXOffset, value);
		base.transform.localPosition = vector;
		this._previousLocalPosition = vector - this.startingPos;
		this._cachedProgress = value;
	}

	// Token: 0x06001B4C RID: 6988 RVA: 0x00086954 File Offset: 0x00084B54
	public float GetProgress()
	{
		Vector3 vector = base.transform.localPosition - this.startingPos;
		if (vector == this._previousLocalPosition)
		{
			return this._cachedProgress;
		}
		this._previousLocalPosition = vector;
		this._cachedProgress = (vector.x - this.minXOffset) / (this.maxXOffset - this.minXOffset);
		return this._cachedProgress;
	}

	// Token: 0x04001E44 RID: 7748
	[SerializeField]
	private float farPushDist = 0.015f;

	// Token: 0x04001E45 RID: 7749
	[SerializeField]
	private float maxXOffset;

	// Token: 0x04001E46 RID: 7750
	[SerializeField]
	private float minXOffset;

	// Token: 0x04001E47 RID: 7751
	private Matrix4x4 localSpace;

	// Token: 0x04001E48 RID: 7752
	private Vector3 startingPos;

	// Token: 0x04001E49 RID: 7753
	private Vector3 _previousLocalPosition;

	// Token: 0x04001E4A RID: 7754
	private float _cachedProgress;
}
