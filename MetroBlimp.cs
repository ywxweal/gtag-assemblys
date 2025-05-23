using System;
using UnityEngine;

// Token: 0x020000C0 RID: 192
public class MetroBlimp : MonoBehaviour
{
	// Token: 0x060004D5 RID: 1237 RVA: 0x0001C258 File Offset: 0x0001A458
	private void Awake()
	{
		this._startLocalHeight = base.transform.localPosition.y;
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x0001C270 File Offset: 0x0001A470
	public void Tick()
	{
		bool flag = Mathf.Sin(Time.time * 2f) * 0.5f + 0.5f < 0.0001f;
		int num = Mathf.CeilToInt(this._numHandsOnBlimp / 2f);
		if (this._numHandsOnBlimp == 0f)
		{
			this._topStayTime = 0f;
			if (flag)
			{
				this.blimpRenderer.material.DisableKeyword("_INNER_GLOW");
			}
		}
		else
		{
			this._topStayTime += Time.deltaTime;
			if (flag)
			{
				this.blimpRenderer.material.EnableKeyword("_INNER_GLOW");
			}
		}
		Vector3 localPosition = base.transform.localPosition;
		Vector3 vector = localPosition;
		float y = vector.y;
		float num2 = this._startLocalHeight + this.descendOffset;
		float deltaTime = Time.deltaTime;
		if (num > 0)
		{
			if (y > num2)
			{
				vector += Vector3.down * (this.descendSpeed * (float)num * deltaTime);
			}
		}
		else if (y < this._startLocalHeight)
		{
			vector += Vector3.up * (this.ascendSpeed * deltaTime);
		}
		base.transform.localPosition = Vector3.Slerp(localPosition, vector, 0.5f);
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x0001C39F File Offset: 0x0001A59F
	private static bool IsPlayerHand(Collider c)
	{
		return c.gameObject.IsOnLayer(UnityLayer.GorillaHand);
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x0001C3AE File Offset: 0x0001A5AE
	private void OnTriggerEnter(Collider other)
	{
		if (MetroBlimp.IsPlayerHand(other))
		{
			this._numHandsOnBlimp += 1f;
		}
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x0001C3CA File Offset: 0x0001A5CA
	private void OnTriggerExit(Collider other)
	{
		if (MetroBlimp.IsPlayerHand(other))
		{
			this._numHandsOnBlimp -= 1f;
		}
	}

	// Token: 0x040005A4 RID: 1444
	public MetroSpotlight spotLightLeft;

	// Token: 0x040005A5 RID: 1445
	public MetroSpotlight spotLightRight;

	// Token: 0x040005A6 RID: 1446
	[Space]
	public BoxCollider topCollider;

	// Token: 0x040005A7 RID: 1447
	public Material blimpMaterial;

	// Token: 0x040005A8 RID: 1448
	public Renderer blimpRenderer;

	// Token: 0x040005A9 RID: 1449
	[Space]
	public float ascendSpeed = 1f;

	// Token: 0x040005AA RID: 1450
	public float descendSpeed = 0.5f;

	// Token: 0x040005AB RID: 1451
	public float descendOffset = -24.1f;

	// Token: 0x040005AC RID: 1452
	public float descendReactionTime = 3f;

	// Token: 0x040005AD RID: 1453
	[Space]
	[NonSerialized]
	private float _startLocalHeight;

	// Token: 0x040005AE RID: 1454
	[NonSerialized]
	private float _topStayTime;

	// Token: 0x040005AF RID: 1455
	[NonSerialized]
	private float _numHandsOnBlimp;

	// Token: 0x040005B0 RID: 1456
	[NonSerialized]
	private bool _lowering;

	// Token: 0x040005B1 RID: 1457
	private const string _INNER_GLOW = "_INNER_GLOW";
}
