using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020003B3 RID: 947
public class SlingshotProjectileTrail : MonoBehaviour
{
	// Token: 0x06001630 RID: 5680 RVA: 0x0006BAF8 File Offset: 0x00069CF8
	private void Awake()
	{
		this.initialWidthMultiplier = this.trailRenderer.widthMultiplier;
	}

	// Token: 0x06001631 RID: 5681 RVA: 0x0006BB0C File Offset: 0x00069D0C
	public void AttachTrail(GameObject obj, bool blueTeam, bool redTeam)
	{
		this.followObject = obj;
		this.followXform = this.followObject.transform;
		Transform transform = base.transform;
		transform.position = this.followXform.position;
		this.initialScale = transform.localScale.x;
		transform.localScale = this.followXform.localScale;
		this.trailRenderer.widthMultiplier = this.initialWidthMultiplier * this.followXform.localScale.x;
		this.trailRenderer.Clear();
		if (blueTeam)
		{
			this.SetColor(this.blueColor);
		}
		else if (redTeam)
		{
			this.SetColor(this.orangeColor);
		}
		else
		{
			this.SetColor(this.defaultColor);
		}
		this.timeToDie = -1f;
	}

	// Token: 0x06001632 RID: 5682 RVA: 0x0006BBD4 File Offset: 0x00069DD4
	protected void LateUpdate()
	{
		if (this.followObject.IsNull())
		{
			ObjectPools.instance.Destroy(base.gameObject);
			return;
		}
		base.gameObject.transform.position = this.followXform.position;
		if (!this.followObject.activeSelf && this.timeToDie < 0f)
		{
			this.timeToDie = Time.time + this.trailRenderer.time;
		}
		if (this.timeToDie > 0f && Time.time > this.timeToDie)
		{
			base.transform.localScale = Vector3.one * this.initialScale;
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x0006BC90 File Offset: 0x00069E90
	public void SetColor(Color color)
	{
		TrailRenderer trailRenderer = this.trailRenderer;
		this.trailRenderer.endColor = color;
		trailRenderer.startColor = color;
	}

	// Token: 0x040018A5 RID: 6309
	public TrailRenderer trailRenderer;

	// Token: 0x040018A6 RID: 6310
	public Color defaultColor = Color.white;

	// Token: 0x040018A7 RID: 6311
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

	// Token: 0x040018A8 RID: 6312
	public Color blueColor = new Color(0f, 0.72f, 1f, 1f);

	// Token: 0x040018A9 RID: 6313
	private GameObject followObject;

	// Token: 0x040018AA RID: 6314
	private Transform followXform;

	// Token: 0x040018AB RID: 6315
	private float timeToDie = -1f;

	// Token: 0x040018AC RID: 6316
	private float initialScale;

	// Token: 0x040018AD RID: 6317
	private float initialWidthMultiplier;
}
