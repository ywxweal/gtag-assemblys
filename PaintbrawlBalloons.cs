using System;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020003A5 RID: 933
public class PaintbrawlBalloons : MonoBehaviour
{
	// Token: 0x060015CB RID: 5579 RVA: 0x0006A124 File Offset: 0x00068324
	protected void Awake()
	{
		this.matPropBlock = new MaterialPropertyBlock();
		this.renderers = new Renderer[this.balloons.Length];
		this.balloonsCachedActiveState = new bool[this.balloons.Length];
		for (int i = 0; i < this.balloons.Length; i++)
		{
			this.renderers[i] = this.balloons[i].GetComponentInChildren<Renderer>();
			this.balloonsCachedActiveState[i] = this.balloons[i].activeSelf;
		}
		this.colorShaderPropID = Shader.PropertyToID("_Color");
	}

	// Token: 0x060015CC RID: 5580 RVA: 0x0006A1AF File Offset: 0x000683AF
	protected void OnEnable()
	{
		this.UpdateBalloonColors();
	}

	// Token: 0x060015CD RID: 5581 RVA: 0x0006A1B8 File Offset: 0x000683B8
	protected void LateUpdate()
	{
		if (GorillaGameManager.instance != null && (this.bMgr != null || GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>() != null))
		{
			if (this.bMgr == null)
			{
				this.bMgr = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
			}
			int playerLives = this.bMgr.GetPlayerLives(this.myRig.creator);
			for (int i = 0; i < this.balloons.Length; i++)
			{
				bool flag = playerLives >= i + 1;
				if (flag != this.balloonsCachedActiveState[i])
				{
					this.balloonsCachedActiveState[i] = flag;
					this.balloons[i].SetActive(flag);
					if (!flag)
					{
						this.PopBalloon(i);
					}
				}
			}
		}
		else if (GorillaGameManager.instance != null)
		{
			base.gameObject.SetActive(false);
		}
		this.UpdateBalloonColors();
	}

	// Token: 0x060015CE RID: 5582 RVA: 0x0006A2A4 File Offset: 0x000684A4
	private void PopBalloon(int i)
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(this.balloonPopFXPrefab, true);
		gameObject.transform.position = this.balloons[i].transform.position;
		GorillaColorizableBase componentInChildren = gameObject.GetComponentInChildren<GorillaColorizableBase>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(this.teamColor);
		}
	}

	// Token: 0x060015CF RID: 5583 RVA: 0x0006A2FC File Offset: 0x000684FC
	public void UpdateBalloonColors()
	{
		if (this.bMgr != null && this.myRig.creator != null)
		{
			if (this.bMgr.OnRedTeam(this.myRig.creator))
			{
				this.teamColor = this.orangeColor;
			}
			else
			{
				this.teamColor = this.blueColor;
			}
		}
		if (this.teamColor != this.lastColor)
		{
			this.lastColor = this.teamColor;
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer)
				{
					foreach (Material material in renderer.materials)
					{
						if (!(material == null))
						{
							if (material.HasProperty("_BaseColor"))
							{
								material.SetColor("_BaseColor", this.teamColor);
							}
							if (material.HasProperty("_Color"))
							{
								material.SetColor("_Color", this.teamColor);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0400183C RID: 6204
	public VRRig myRig;

	// Token: 0x0400183D RID: 6205
	public GameObject[] balloons;

	// Token: 0x0400183E RID: 6206
	public Color orangeColor;

	// Token: 0x0400183F RID: 6207
	public Color blueColor;

	// Token: 0x04001840 RID: 6208
	public Color defaultColor;

	// Token: 0x04001841 RID: 6209
	public Color lastColor;

	// Token: 0x04001842 RID: 6210
	public GameObject balloonPopFXPrefab;

	// Token: 0x04001843 RID: 6211
	[HideInInspector]
	public GorillaPaintbrawlManager bMgr;

	// Token: 0x04001844 RID: 6212
	public Player myPlayer;

	// Token: 0x04001845 RID: 6213
	private int colorShaderPropID;

	// Token: 0x04001846 RID: 6214
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x04001847 RID: 6215
	private bool[] balloonsCachedActiveState;

	// Token: 0x04001848 RID: 6216
	private Renderer[] renderers;

	// Token: 0x04001849 RID: 6217
	private Color teamColor;
}
