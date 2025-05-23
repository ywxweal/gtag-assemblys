using System;
using System.Collections;
using GorillaTag;
using UnityEngine;

// Token: 0x0200039D RID: 925
public class HitTargetScoreDisplay : MonoBehaviour
{
	// Token: 0x06001595 RID: 5525 RVA: 0x00069494 File Offset: 0x00067694
	protected void Awake()
	{
		this.rotateTimeTotal = 180f / (float)this.rotateSpeed;
		this.matPropBlock = new MaterialPropertyBlock();
		this.networkedScore.AddCallback(new Action<int>(this.OnScoreChanged), true);
		this.ResetRotation();
		this.tensOld = 0;
		this.hundredsOld = 0;
		this.matPropBlock.SetVector(this.shaderPropID_MainTex_ST, this.numberSheet[0]);
		this.singlesRend.SetPropertyBlock(this.matPropBlock);
		this.tensRend.SetPropertyBlock(this.matPropBlock);
		this.hundredsRend.SetPropertyBlock(this.matPropBlock);
	}

	// Token: 0x06001596 RID: 5526 RVA: 0x0006953B File Offset: 0x0006773B
	private void OnDestroy()
	{
		this.networkedScore.RemoveCallback(new Action<int>(this.OnScoreChanged));
	}

	// Token: 0x06001597 RID: 5527 RVA: 0x00069554 File Offset: 0x00067754
	private void ResetRotation()
	{
		Quaternion rotation = base.transform.rotation;
		this.singlesCard.rotation = rotation;
		this.tensCard.rotation = rotation;
		this.hundredsCard.rotation = rotation;
	}

	// Token: 0x06001598 RID: 5528 RVA: 0x00069591 File Offset: 0x00067791
	private IEnumerator RotatingCo()
	{
		float timeElapsedSinceHit = 0f;
		int singlesPlace = this.currentScore % 10;
		int tensPlace = this.currentScore / 10 % 10;
		bool tensChange = this.tensOld != tensPlace;
		this.tensOld = tensPlace;
		int hundredsPlace = this.currentScore / 100 % 10;
		bool hundredsChange = this.hundredsOld != hundredsPlace;
		this.hundredsOld = hundredsPlace;
		bool digitsChange = true;
		while (timeElapsedSinceHit < this.rotateTimeTotal)
		{
			this.singlesCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
			Vector3 localEulerAngles = this.singlesCard.localEulerAngles;
			localEulerAngles.x = Mathf.Clamp(localEulerAngles.x, 0f, 180f);
			this.singlesCard.localEulerAngles = localEulerAngles;
			if (tensChange)
			{
				this.tensCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
				Vector3 localEulerAngles2 = this.tensCard.localEulerAngles;
				localEulerAngles2.x = Mathf.Clamp(localEulerAngles2.x, 0f, 180f);
				this.tensCard.localEulerAngles = localEulerAngles2;
			}
			if (hundredsChange)
			{
				this.hundredsCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
				Vector3 localEulerAngles3 = this.hundredsCard.localEulerAngles;
				localEulerAngles3.x = Mathf.Clamp(localEulerAngles3.x, 0f, 180f);
				this.hundredsCard.localEulerAngles = localEulerAngles3;
			}
			if (digitsChange && timeElapsedSinceHit >= this.rotateTimeTotal / 2f)
			{
				this.matPropBlock.SetVector(this.shaderPropID_MainTex_ST, this.numberSheet[singlesPlace]);
				this.singlesRend.SetPropertyBlock(this.matPropBlock);
				if (tensChange)
				{
					this.matPropBlock.SetVector(this.shaderPropID_MainTex_ST, this.numberSheet[tensPlace]);
					this.tensRend.SetPropertyBlock(this.matPropBlock);
				}
				if (hundredsChange)
				{
					this.matPropBlock.SetVector(this.shaderPropID_MainTex_ST, this.numberSheet[hundredsPlace]);
					this.hundredsRend.SetPropertyBlock(this.matPropBlock);
				}
				digitsChange = false;
			}
			yield return null;
			timeElapsedSinceHit += Time.deltaTime;
		}
		this.ResetRotation();
		yield break;
		yield break;
	}

	// Token: 0x06001599 RID: 5529 RVA: 0x000695A0 File Offset: 0x000677A0
	private void OnScoreChanged(int newScore)
	{
		if (newScore == this.currentScore)
		{
			return;
		}
		if (this.currentRotationCoroutine != null)
		{
			base.StopCoroutine(this.currentRotationCoroutine);
		}
		this.currentScore = newScore;
		this.currentRotationCoroutine = base.StartCoroutine(this.RotatingCo());
	}

	// Token: 0x04001802 RID: 6146
	[SerializeField]
	private WatchableIntSO networkedScore;

	// Token: 0x04001803 RID: 6147
	private int currentScore;

	// Token: 0x04001804 RID: 6148
	private int tensOld;

	// Token: 0x04001805 RID: 6149
	private int hundredsOld;

	// Token: 0x04001806 RID: 6150
	private float rotateTimeTotal;

	// Token: 0x04001807 RID: 6151
	private int shaderPropID_MainTex_ST = Shader.PropertyToID("_BaseMap_ST");

	// Token: 0x04001808 RID: 6152
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x04001809 RID: 6153
	private readonly Vector4[] numberSheet = new Vector4[]
	{
		new Vector4(1f, 1f, 0.8f, -0.5f),
		new Vector4(1f, 1f, 0f, 0f),
		new Vector4(1f, 1f, 0.2f, 0f),
		new Vector4(1f, 1f, 0.4f, 0f),
		new Vector4(1f, 1f, 0.6f, 0f),
		new Vector4(1f, 1f, 0.8f, 0f),
		new Vector4(1f, 1f, 0f, -0.5f),
		new Vector4(1f, 1f, 0.2f, -0.5f),
		new Vector4(1f, 1f, 0.4f, -0.5f),
		new Vector4(1f, 1f, 0.6f, -0.5f)
	};

	// Token: 0x0400180A RID: 6154
	public int rotateSpeed = 180;

	// Token: 0x0400180B RID: 6155
	public Transform singlesCard;

	// Token: 0x0400180C RID: 6156
	public Transform tensCard;

	// Token: 0x0400180D RID: 6157
	public Transform hundredsCard;

	// Token: 0x0400180E RID: 6158
	public Renderer singlesRend;

	// Token: 0x0400180F RID: 6159
	public Renderer tensRend;

	// Token: 0x04001810 RID: 6160
	public Renderer hundredsRend;

	// Token: 0x04001811 RID: 6161
	private Coroutine currentRotationCoroutine;
}
