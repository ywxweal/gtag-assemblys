using System;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200050A RID: 1290
public class BuilderResourceMeter : MonoBehaviour
{
	// Token: 0x06001F4F RID: 8015 RVA: 0x0009D070 File Offset: 0x0009B270
	private void Awake()
	{
		this.fillColor = this.resourceColors.GetColorForType(this._resourceType);
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		this.fillCube.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetColor("_BaseColor", this.fillColor);
		this.fillCube.SetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetColor("_BaseColor", this.emptyColor);
		this.emptyCube.SetPropertyBlock(materialPropertyBlock);
		this.fillAmount = this.fillTarget;
	}

	// Token: 0x06001F50 RID: 8016 RVA: 0x0009D0EC File Offset: 0x0009B2EC
	private void Start()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.OnZoneChanged();
	}

	// Token: 0x06001F51 RID: 8017 RVA: 0x0009D11A File Offset: 0x0009B31A
	private void OnDestroy()
	{
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x06001F52 RID: 8018 RVA: 0x0009D150 File Offset: 0x0009B350
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
		if (flag != this.inBuilderZone)
		{
			this.inBuilderZone = flag;
			if (!flag)
			{
				this.fillCube.enabled = false;
				this.emptyCube.enabled = false;
				return;
			}
			this.fillCube.enabled = true;
			this.emptyCube.enabled = true;
			this.OnAvailableResourcesChange();
		}
	}

	// Token: 0x06001F53 RID: 8019 RVA: 0x0009D1B4 File Offset: 0x0009B3B4
	public void OnAvailableResourcesChange()
	{
		if (this.table == null || this.table.maxResources == null)
		{
			return;
		}
		this.resourceMax = this.table.maxResources[(int)this._resourceType];
		int num = this.table.usedResources[(int)this._resourceType];
		if (num != this.usedResource)
		{
			this.usedResource = num;
			this.SetNormalizedFillTarget((float)(this.resourceMax - this.usedResource) / (float)this.resourceMax);
		}
	}

	// Token: 0x06001F54 RID: 8020 RVA: 0x0009D234 File Offset: 0x0009B434
	public void UpdateMeterFill()
	{
		if (this.animatingMeter)
		{
			float num = Mathf.MoveTowards(this.fillAmount, this.fillTarget, this.lerpSpeed * Time.deltaTime);
			this.UpdateFill(num);
		}
	}

	// Token: 0x06001F55 RID: 8021 RVA: 0x0009D270 File Offset: 0x0009B470
	private void UpdateFill(float newFill)
	{
		this.fillAmount = newFill;
		if (Mathf.Approximately(this.fillAmount, this.fillTarget))
		{
			this.fillAmount = this.fillTarget;
			this.animatingMeter = false;
		}
		if (!this.inBuilderZone)
		{
			return;
		}
		if (this.fillAmount <= 1E-45f)
		{
			this.fillCube.enabled = false;
			float num = this.meterHeight / this.meshHeight;
			Vector3 vector = new Vector3(this.emptyCube.transform.localScale.x, num, this.emptyCube.transform.localScale.z);
			Vector3 vector2 = new Vector3(0f, this.meterHeight / 2f, 0f);
			this.emptyCube.transform.localScale = vector;
			this.emptyCube.transform.localPosition = vector2;
			this.emptyCube.enabled = true;
			return;
		}
		if (this.fillAmount >= 1f)
		{
			float num2 = this.meterHeight / this.meshHeight;
			Vector3 vector3 = new Vector3(this.fillCube.transform.localScale.x, num2, this.fillCube.transform.localScale.z);
			Vector3 vector4 = new Vector3(0f, this.meterHeight / 2f, 0f);
			this.fillCube.transform.localScale = vector3;
			this.fillCube.transform.localPosition = vector4;
			this.fillCube.enabled = true;
			this.emptyCube.enabled = false;
			return;
		}
		float num3 = this.meterHeight / this.meshHeight * this.fillAmount;
		Vector3 vector5 = new Vector3(this.fillCube.transform.localScale.x, num3, this.fillCube.transform.localScale.z);
		Vector3 vector6 = new Vector3(0f, num3 * this.meshHeight / 2f, 0f);
		this.fillCube.transform.localScale = vector5;
		this.fillCube.transform.localPosition = vector6;
		this.fillCube.enabled = true;
		float num4 = this.meterHeight / this.meshHeight * (1f - this.fillAmount);
		Vector3 vector7 = new Vector3(this.emptyCube.transform.localScale.x, num4, this.emptyCube.transform.localScale.z);
		Vector3 vector8 = new Vector3(0f, this.meterHeight - num4 * this.meshHeight / 2f, 0f);
		this.emptyCube.transform.localScale = vector7;
		this.emptyCube.transform.localPosition = vector8;
		this.emptyCube.enabled = true;
	}

	// Token: 0x06001F56 RID: 8022 RVA: 0x0009D544 File Offset: 0x0009B744
	public void SetNormalizedFillTarget(float fill)
	{
		this.fillTarget = Mathf.Clamp(fill, 0f, 1f);
		this.animatingMeter = true;
	}

	// Token: 0x04002316 RID: 8982
	public BuilderResourceColors resourceColors;

	// Token: 0x04002317 RID: 8983
	public MeshRenderer fillCube;

	// Token: 0x04002318 RID: 8984
	public MeshRenderer emptyCube;

	// Token: 0x04002319 RID: 8985
	private Color fillColor = Color.white;

	// Token: 0x0400231A RID: 8986
	public Color emptyColor = Color.black;

	// Token: 0x0400231B RID: 8987
	[FormerlySerializedAs("MeterHeight")]
	public float meterHeight = 2f;

	// Token: 0x0400231C RID: 8988
	public float meshHeight = 1f;

	// Token: 0x0400231D RID: 8989
	public BuilderResourceType _resourceType;

	// Token: 0x0400231E RID: 8990
	private float fillAmount;

	// Token: 0x0400231F RID: 8991
	[Range(0f, 1f)]
	[SerializeField]
	private float fillTarget;

	// Token: 0x04002320 RID: 8992
	public float lerpSpeed = 0.5f;

	// Token: 0x04002321 RID: 8993
	private bool animatingMeter;

	// Token: 0x04002322 RID: 8994
	private int resourceMax = -1;

	// Token: 0x04002323 RID: 8995
	private int usedResource = -1;

	// Token: 0x04002324 RID: 8996
	private bool inBuilderZone;

	// Token: 0x04002325 RID: 8997
	internal BuilderTable table;
}
