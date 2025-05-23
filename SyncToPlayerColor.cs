using System;
using UnityEngine;

// Token: 0x02000472 RID: 1138
public class SyncToPlayerColor : MonoBehaviour
{
	// Token: 0x06001BFC RID: 7164 RVA: 0x00089B35 File Offset: 0x00087D35
	protected virtual void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this._colorFunc = new Action<Color>(this.UpdateColor);
	}

	// Token: 0x06001BFD RID: 7165 RVA: 0x00089B56 File Offset: 0x00087D56
	protected virtual void Start()
	{
		this.UpdateColor(this.rig.playerColor);
		this.rig.OnColorInitialized(this._colorFunc);
	}

	// Token: 0x06001BFE RID: 7166 RVA: 0x00089B7A File Offset: 0x00087D7A
	protected virtual void OnEnable()
	{
		this.rig.OnColorChanged += this._colorFunc;
	}

	// Token: 0x06001BFF RID: 7167 RVA: 0x00089B8D File Offset: 0x00087D8D
	protected virtual void OnDisable()
	{
		this.rig.OnColorChanged -= this._colorFunc;
	}

	// Token: 0x06001C00 RID: 7168 RVA: 0x00089BA0 File Offset: 0x00087DA0
	public virtual void UpdateColor(Color color)
	{
		if (!this.target)
		{
			return;
		}
		if (this.colorPropertiesToSync == null)
		{
			return;
		}
		for (int i = 0; i < this.colorPropertiesToSync.Length; i++)
		{
			ShaderHashId shaderHashId = this.colorPropertiesToSync[i];
			this.target.SetColor(shaderHashId, color);
		}
	}

	// Token: 0x04001F1F RID: 7967
	public VRRig rig;

	// Token: 0x04001F20 RID: 7968
	public Material target;

	// Token: 0x04001F21 RID: 7969
	public ShaderHashId[] colorPropertiesToSync = new ShaderHashId[] { "_BaseColor" };

	// Token: 0x04001F22 RID: 7970
	private Action<Color> _colorFunc;
}
