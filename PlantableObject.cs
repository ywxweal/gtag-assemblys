using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000417 RID: 1047
public class PlantableObject : TransferrableObject
{
	// Token: 0x06001992 RID: 6546 RVA: 0x0007C188 File Offset: 0x0007A388
	protected override void Awake()
	{
		base.Awake();
		this.colorRShaderPropID = Shader.PropertyToID("_ColorR");
		this.colorGShaderPropID = Shader.PropertyToID("_ColorG");
		this.colorBShaderPropID = Shader.PropertyToID("_ColorB");
		this.materialPropertyBlock = new MaterialPropertyBlock();
	}

	// Token: 0x06001993 RID: 6547 RVA: 0x0007C1D8 File Offset: 0x0007A3D8
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.materialPropertyBlock.SetColor(this.colorRShaderPropID, this._colorR);
		this.flagRenderer.material = this.flagRenderer.sharedMaterial;
		this.flagRenderer.SetPropertyBlock(this.materialPropertyBlock);
		this.dippedColors = new PlantableObject.AppliedColors[20];
	}

	// Token: 0x06001994 RID: 6548 RVA: 0x0007C238 File Offset: 0x0007A438
	private void AssureShaderStuff()
	{
		if (!this.flagRenderer)
		{
			return;
		}
		if (this.colorRShaderPropID == 0)
		{
			this.colorRShaderPropID = Shader.PropertyToID("_ColorR");
			this.colorGShaderPropID = Shader.PropertyToID("_ColorG");
			this.colorBShaderPropID = Shader.PropertyToID("_ColorB");
		}
		if (this.materialPropertyBlock == null)
		{
			this.materialPropertyBlock = new MaterialPropertyBlock();
		}
		try
		{
			this.materialPropertyBlock.SetColor(this.colorRShaderPropID, this._colorR);
			this.materialPropertyBlock.SetColor(this.colorGShaderPropID, this._colorG);
		}
		catch
		{
			this.materialPropertyBlock = new MaterialPropertyBlock();
			this.materialPropertyBlock.SetColor(this.colorRShaderPropID, this._colorR);
			this.materialPropertyBlock.SetColor(this.colorGShaderPropID, this._colorG);
		}
		this.flagRenderer.material = this.flagRenderer.sharedMaterial;
		this.flagRenderer.SetPropertyBlock(this.materialPropertyBlock);
	}

	// Token: 0x170002CA RID: 714
	// (get) Token: 0x06001995 RID: 6549 RVA: 0x0007C344 File Offset: 0x0007A544
	// (set) Token: 0x06001996 RID: 6550 RVA: 0x0007C34C File Offset: 0x0007A54C
	public Color colorR
	{
		get
		{
			return this._colorR;
		}
		set
		{
			this._colorR = value;
			this.AssureShaderStuff();
		}
	}

	// Token: 0x170002CB RID: 715
	// (get) Token: 0x06001997 RID: 6551 RVA: 0x0007C35B File Offset: 0x0007A55B
	// (set) Token: 0x06001998 RID: 6552 RVA: 0x0007C363 File Offset: 0x0007A563
	public Color colorG
	{
		get
		{
			return this._colorG;
		}
		set
		{
			this._colorG = value;
			this.AssureShaderStuff();
		}
	}

	// Token: 0x170002CC RID: 716
	// (get) Token: 0x06001999 RID: 6553 RVA: 0x0007C372 File Offset: 0x0007A572
	// (set) Token: 0x0600199A RID: 6554 RVA: 0x0007C37A File Offset: 0x0007A57A
	public bool planted { get; private set; }

	// Token: 0x0600199B RID: 6555 RVA: 0x0007C384 File Offset: 0x0007A584
	public void SetPlanted(bool newPlanted)
	{
		if (this.planted != newPlanted)
		{
			if (newPlanted)
			{
				if (!this.rigidbodyInstance.isKinematic)
				{
					this.rigidbodyInstance.isKinematic = true;
				}
				this.respawnAtTimestamp = Time.time + this.respawnAfterDuration;
			}
			else
			{
				this.respawnAtTimestamp = 0f;
			}
			this.planted = newPlanted;
		}
	}

	// Token: 0x0600199C RID: 6556 RVA: 0x0007C3DC File Offset: 0x0007A5DC
	private void AddRed()
	{
		this.AddColor(PlantableObject.AppliedColors.Red);
	}

	// Token: 0x0600199D RID: 6557 RVA: 0x0007C3E5 File Offset: 0x0007A5E5
	private void AddGreen()
	{
		this.AddColor(PlantableObject.AppliedColors.Blue);
	}

	// Token: 0x0600199E RID: 6558 RVA: 0x0007C3EE File Offset: 0x0007A5EE
	private void AddBlue()
	{
		this.AddColor(PlantableObject.AppliedColors.Green);
	}

	// Token: 0x0600199F RID: 6559 RVA: 0x0007C3F7 File Offset: 0x0007A5F7
	private void AddBlack()
	{
		this.AddColor(PlantableObject.AppliedColors.Black);
	}

	// Token: 0x060019A0 RID: 6560 RVA: 0x0007C400 File Offset: 0x0007A600
	public void AddColor(PlantableObject.AppliedColors color)
	{
		this.dippedColors[this.currentDipIndex] = color;
		this.currentDipIndex++;
		if (this.currentDipIndex >= this.dippedColors.Length)
		{
			this.currentDipIndex = 0;
		}
		this.UpdateDisplayedDippedColor();
	}

	// Token: 0x060019A1 RID: 6561 RVA: 0x0007C43C File Offset: 0x0007A63C
	public void ClearColors()
	{
		for (int i = 0; i < this.dippedColors.Length; i++)
		{
			this.dippedColors[i] = PlantableObject.AppliedColors.None;
		}
		this.currentDipIndex = 0;
		this.UpdateDisplayedDippedColor();
	}

	// Token: 0x060019A2 RID: 6562 RVA: 0x0007C474 File Offset: 0x0007A674
	public Color CalculateOutputColor()
	{
		Color color = Color.black;
		int num = 0;
		int num2 = 0;
		foreach (PlantableObject.AppliedColors appliedColors in this.dippedColors)
		{
			if (appliedColors == PlantableObject.AppliedColors.None)
			{
				break;
			}
			switch (appliedColors)
			{
			case PlantableObject.AppliedColors.Red:
				color += Color.red;
				num2++;
				break;
			case PlantableObject.AppliedColors.Green:
				color += Color.green;
				num2++;
				break;
			case PlantableObject.AppliedColors.Blue:
				color += Color.blue;
				num2++;
				break;
			case PlantableObject.AppliedColors.Black:
				num++;
				num2++;
				break;
			}
		}
		if (color == Color.black && num == 0)
		{
			return Color.white;
		}
		float num3 = Mathf.Max(new float[] { color.r, color.g, color.b });
		if (num3 == 0f)
		{
			return Color.black;
		}
		color /= num3;
		float num4 = (float)num / (float)num2;
		if (num4 > 0f)
		{
			color *= 1f - num4;
		}
		return color;
	}

	// Token: 0x060019A3 RID: 6563 RVA: 0x0007C57D File Offset: 0x0007A77D
	public void UpdateDisplayedDippedColor()
	{
		this.colorR = this.CalculateOutputColor();
	}

	// Token: 0x060019A4 RID: 6564 RVA: 0x0007C58B File Offset: 0x0007A78B
	public override void DropItem()
	{
		base.DropItem();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x060019A5 RID: 6565 RVA: 0x0007C5B8 File Offset: 0x0007A7B8
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		this.itemState = (this.planted ? TransferrableObject.ItemStates.State1 : TransferrableObject.ItemStates.State0);
		if (this.respawnAtTimestamp != 0f && Time.time > this.respawnAtTimestamp)
		{
			this.respawnAtTimestamp = 0f;
			this.ResetToHome();
		}
	}

	// Token: 0x060019A6 RID: 6566 RVA: 0x0007C608 File Offset: 0x0007A808
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x060019A7 RID: 6567 RVA: 0x0007C632 File Offset: 0x0007A832
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
	}

	// Token: 0x060019A8 RID: 6568 RVA: 0x0007C63C File Offset: 0x0007A83C
	public override bool ShouldBeKinematic()
	{
		return base.ShouldBeKinematic() || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x060019A9 RID: 6569 RVA: 0x0007C654 File Offset: 0x0007A854
	public override void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		base.OnOwnershipTransferred(toPlayer, fromPlayer);
		if (toPlayer == null)
		{
			return;
		}
		if (toPlayer.IsLocal && this.itemState == TransferrableObject.ItemStates.State1)
		{
			this.respawnAtTimestamp = Time.time + this.respawnAfterDuration;
		}
		Action<Color> <>9__1;
		GorillaGameManager.OnInstanceReady(delegate
		{
			VRRig vrrig = GorillaGameManager.instance.FindPlayerVRRig(toPlayer);
			if (vrrig == null)
			{
				return;
			}
			VRRig vrrig2 = vrrig;
			Action<Color> action;
			if ((action = <>9__1) == null)
			{
				action = (<>9__1 = delegate(Color color1)
				{
					this.colorG = color1;
				});
			}
			vrrig2.OnColorInitialized(action);
		});
	}

	// Token: 0x04001C82 RID: 7298
	public PlantablePoint point;

	// Token: 0x04001C83 RID: 7299
	public float respawnAfterDuration;

	// Token: 0x04001C84 RID: 7300
	private float respawnAtTimestamp;

	// Token: 0x04001C85 RID: 7301
	public SkinnedMeshRenderer flagRenderer;

	// Token: 0x04001C86 RID: 7302
	[FormerlySerializedAs("colorShaderPropID")]
	[SerializeReference]
	private int colorRShaderPropID;

	// Token: 0x04001C87 RID: 7303
	[SerializeReference]
	private int colorGShaderPropID;

	// Token: 0x04001C88 RID: 7304
	[SerializeReference]
	private int colorBShaderPropID;

	// Token: 0x04001C89 RID: 7305
	private MaterialPropertyBlock materialPropertyBlock;

	// Token: 0x04001C8A RID: 7306
	[HideInInspector]
	[SerializeReference]
	private Color _colorR;

	// Token: 0x04001C8B RID: 7307
	[HideInInspector]
	[SerializeReference]
	private Color _colorG;

	// Token: 0x04001C8D RID: 7309
	public Transform flagTip;

	// Token: 0x04001C8E RID: 7310
	public PlantableObject.AppliedColors[] dippedColors = new PlantableObject.AppliedColors[20];

	// Token: 0x04001C8F RID: 7311
	public int currentDipIndex;

	// Token: 0x02000418 RID: 1048
	public enum AppliedColors
	{
		// Token: 0x04001C91 RID: 7313
		None,
		// Token: 0x04001C92 RID: 7314
		Red,
		// Token: 0x04001C93 RID: 7315
		Green,
		// Token: 0x04001C94 RID: 7316
		Blue,
		// Token: 0x04001C95 RID: 7317
		Black
	}
}
