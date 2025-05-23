using System;
using UnityEngine;

// Token: 0x02000521 RID: 1313
public class Campfire : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06001FC6 RID: 8134 RVA: 0x000A0800 File Offset: 0x0009EA00
	private void Start()
	{
		this.lastAngleBottom = 0f;
		this.lastAngleMiddle = 0f;
		this.lastAngleTop = 0f;
		this.perlinBottom = (float)Random.Range(0, 100);
		this.perlinMiddle = (float)Random.Range(200, 300);
		this.perlinTop = (float)Random.Range(400, 500);
		this.startingRotationBottom = this.baseFire.localEulerAngles.x;
		this.startingRotationMiddle = this.middleFire.localEulerAngles.x;
		this.startingRotationTop = this.topFire.localEulerAngles.x;
		this.tempVec = new Vector3(0f, 0f, 0f);
		this.mergedBottom = false;
		this.mergedMiddle = false;
		this.mergedTop = false;
		this.wasActive = false;
		this.lastTime = Time.time;
	}

	// Token: 0x06001FC7 RID: 8135 RVA: 0x00010F2B File Offset: 0x0000F12B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06001FC8 RID: 8136 RVA: 0x00010F34 File Offset: 0x0000F134
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06001FC9 RID: 8137 RVA: 0x000A08EC File Offset: 0x0009EAEC
	public void SliceUpdate()
	{
		if (BetterDayNightManager.instance == null)
		{
			return;
		}
		if ((this.isActive[BetterDayNightManager.instance.currentTimeIndex] && BetterDayNightManager.instance.CurrentWeather() != BetterDayNightManager.WeatherType.Raining) || this.overrideDayNight == 1)
		{
			if (!this.wasActive)
			{
				this.wasActive = true;
				this.mergedBottom = false;
				this.mergedMiddle = false;
				this.mergedTop = false;
				Color.RGBToHSV(this.mat.color, out this.h, out this.s, out this.v);
				this.mat.color = Color.HSVToRGB(this.h, this.s, 1f);
			}
			this.Flap(ref this.perlinBottom, this.perlinStepBottom, ref this.lastAngleBottom, ref this.baseFire, this.bottomRange, this.baseMultiplier, ref this.mergedBottom);
			this.Flap(ref this.perlinMiddle, this.perlinStepMiddle, ref this.lastAngleMiddle, ref this.middleFire, this.middleRange, this.middleMultiplier, ref this.mergedMiddle);
			this.Flap(ref this.perlinTop, this.perlinStepTop, ref this.lastAngleTop, ref this.topFire, this.topRange, this.topMultiplier, ref this.mergedTop);
		}
		else
		{
			if (this.wasActive)
			{
				this.wasActive = false;
				this.mergedBottom = false;
				this.mergedMiddle = false;
				this.mergedTop = false;
				Color.RGBToHSV(this.mat.color, out this.h, out this.s, out this.v);
				this.mat.color = Color.HSVToRGB(this.h, this.s, 0.25f);
			}
			this.ReturnToOff(ref this.baseFire, this.startingRotationBottom, ref this.mergedBottom);
			this.ReturnToOff(ref this.middleFire, this.startingRotationMiddle, ref this.mergedMiddle);
			this.ReturnToOff(ref this.topFire, this.startingRotationTop, ref this.mergedTop);
		}
		this.lastTime = Time.time;
	}

	// Token: 0x06001FCA RID: 8138 RVA: 0x000A0AF0 File Offset: 0x0009ECF0
	private void Flap(ref float perlinValue, float perlinStep, ref float lastAngle, ref Transform flameTransform, float range, float multiplier, ref bool isMerged)
	{
		perlinValue += perlinStep;
		lastAngle += (Time.time - this.lastTime) * Mathf.PerlinNoise(perlinValue, 0f);
		this.tempVec.x = range * Mathf.Sin(lastAngle * multiplier);
		if (Mathf.Abs(this.tempVec.x - flameTransform.localEulerAngles.x) > 180f)
		{
			if (this.tempVec.x > flameTransform.localEulerAngles.x)
			{
				this.tempVec.x = this.tempVec.x - 360f;
			}
			else
			{
				this.tempVec.x = this.tempVec.x + 360f;
			}
		}
		if (isMerged)
		{
			flameTransform.localEulerAngles = this.tempVec;
			return;
		}
		if (Mathf.Abs(flameTransform.localEulerAngles.x - this.tempVec.x) < 1f)
		{
			isMerged = true;
			flameTransform.localEulerAngles = this.tempVec;
			return;
		}
		this.tempVec.x = (this.tempVec.x - flameTransform.localEulerAngles.x) * this.slerp + flameTransform.localEulerAngles.x;
		flameTransform.localEulerAngles = this.tempVec;
	}

	// Token: 0x06001FCB RID: 8139 RVA: 0x000A0C38 File Offset: 0x0009EE38
	private void ReturnToOff(ref Transform startTransform, float targetAngle, ref bool isMerged)
	{
		this.tempVec.x = targetAngle;
		if (Mathf.Abs(this.tempVec.x - startTransform.localEulerAngles.x) > 180f)
		{
			if (this.tempVec.x > startTransform.localEulerAngles.x)
			{
				this.tempVec.x = this.tempVec.x - 360f;
			}
			else
			{
				this.tempVec.x = this.tempVec.x + 360f;
			}
		}
		if (!isMerged)
		{
			if (Mathf.Abs(startTransform.localEulerAngles.x - targetAngle) < 1f)
			{
				isMerged = true;
				return;
			}
			this.tempVec.x = (this.tempVec.x - startTransform.localEulerAngles.x) * this.slerp + startTransform.localEulerAngles.x;
			startTransform.localEulerAngles = this.tempVec;
		}
	}

	// Token: 0x06001FCD RID: 8141 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x040023AD RID: 9133
	public Transform baseFire;

	// Token: 0x040023AE RID: 9134
	public Transform middleFire;

	// Token: 0x040023AF RID: 9135
	public Transform topFire;

	// Token: 0x040023B0 RID: 9136
	public float baseMultiplier;

	// Token: 0x040023B1 RID: 9137
	public float middleMultiplier;

	// Token: 0x040023B2 RID: 9138
	public float topMultiplier;

	// Token: 0x040023B3 RID: 9139
	public float bottomRange;

	// Token: 0x040023B4 RID: 9140
	public float middleRange;

	// Token: 0x040023B5 RID: 9141
	public float topRange;

	// Token: 0x040023B6 RID: 9142
	private float lastAngleBottom;

	// Token: 0x040023B7 RID: 9143
	private float lastAngleMiddle;

	// Token: 0x040023B8 RID: 9144
	private float lastAngleTop;

	// Token: 0x040023B9 RID: 9145
	public float perlinStepBottom;

	// Token: 0x040023BA RID: 9146
	public float perlinStepMiddle;

	// Token: 0x040023BB RID: 9147
	public float perlinStepTop;

	// Token: 0x040023BC RID: 9148
	private float perlinBottom;

	// Token: 0x040023BD RID: 9149
	private float perlinMiddle;

	// Token: 0x040023BE RID: 9150
	private float perlinTop;

	// Token: 0x040023BF RID: 9151
	public float startingRotationBottom;

	// Token: 0x040023C0 RID: 9152
	public float startingRotationMiddle;

	// Token: 0x040023C1 RID: 9153
	public float startingRotationTop;

	// Token: 0x040023C2 RID: 9154
	public float slerp = 0.01f;

	// Token: 0x040023C3 RID: 9155
	private bool mergedBottom;

	// Token: 0x040023C4 RID: 9156
	private bool mergedMiddle;

	// Token: 0x040023C5 RID: 9157
	private bool mergedTop;

	// Token: 0x040023C6 RID: 9158
	public string lastTimeOfDay;

	// Token: 0x040023C7 RID: 9159
	public Material mat;

	// Token: 0x040023C8 RID: 9160
	private float h;

	// Token: 0x040023C9 RID: 9161
	private float s;

	// Token: 0x040023CA RID: 9162
	private float v;

	// Token: 0x040023CB RID: 9163
	public int overrideDayNight;

	// Token: 0x040023CC RID: 9164
	private Vector3 tempVec;

	// Token: 0x040023CD RID: 9165
	public bool[] isActive;

	// Token: 0x040023CE RID: 9166
	public bool wasActive;

	// Token: 0x040023CF RID: 9167
	private float lastTime;
}
