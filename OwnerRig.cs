using System;
using UnityEngine;

// Token: 0x0200069E RID: 1694
public class OwnerRig : MonoBehaviour, IVariable<VRRig>, IVariable, IRigAware
{
	// Token: 0x06002A66 RID: 10854 RVA: 0x000D11CF File Offset: 0x000CF3CF
	public void TryFindRig()
	{
		this._rig = base.GetComponentInParent<VRRig>();
		if (this._rig != null)
		{
			return;
		}
		this._rig = base.GetComponentInChildren<VRRig>();
	}

	// Token: 0x06002A67 RID: 10855 RVA: 0x000D11F8 File Offset: 0x000CF3F8
	public VRRig Get()
	{
		return this._rig;
	}

	// Token: 0x06002A68 RID: 10856 RVA: 0x000D1200 File Offset: 0x000CF400
	public void Set(VRRig value)
	{
		this._rig = value;
	}

	// Token: 0x06002A69 RID: 10857 RVA: 0x000D1209 File Offset: 0x000CF409
	public void Set(GameObject obj)
	{
		this._rig = ((obj != null) ? obj.GetComponentInParent<VRRig>() : null);
	}

	// Token: 0x06002A6A RID: 10858 RVA: 0x000D1200 File Offset: 0x000CF400
	void IRigAware.SetRig(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x06002A6B RID: 10859 RVA: 0x000D1223 File Offset: 0x000CF423
	public static implicit operator bool(OwnerRig or)
	{
		return or != null && !(or == null) && or._rig != null && !(or._rig == null);
	}

	// Token: 0x06002A6C RID: 10860 RVA: 0x000D1250 File Offset: 0x000CF450
	public static implicit operator VRRig(OwnerRig or)
	{
		if (!or)
		{
			return null;
		}
		return or._rig;
	}

	// Token: 0x04002F52 RID: 12114
	[SerializeField]
	private VRRig _rig;
}
