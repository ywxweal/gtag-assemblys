using System;
using UnityEngine;

// Token: 0x0200069E RID: 1694
public class OwnerRig : MonoBehaviour, IVariable<VRRig>, IVariable, IRigAware
{
	// Token: 0x06002A67 RID: 10855 RVA: 0x000D1273 File Offset: 0x000CF473
	public void TryFindRig()
	{
		this._rig = base.GetComponentInParent<VRRig>();
		if (this._rig != null)
		{
			return;
		}
		this._rig = base.GetComponentInChildren<VRRig>();
	}

	// Token: 0x06002A68 RID: 10856 RVA: 0x000D129C File Offset: 0x000CF49C
	public VRRig Get()
	{
		return this._rig;
	}

	// Token: 0x06002A69 RID: 10857 RVA: 0x000D12A4 File Offset: 0x000CF4A4
	public void Set(VRRig value)
	{
		this._rig = value;
	}

	// Token: 0x06002A6A RID: 10858 RVA: 0x000D12AD File Offset: 0x000CF4AD
	public void Set(GameObject obj)
	{
		this._rig = ((obj != null) ? obj.GetComponentInParent<VRRig>() : null);
	}

	// Token: 0x06002A6B RID: 10859 RVA: 0x000D12A4 File Offset: 0x000CF4A4
	void IRigAware.SetRig(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x06002A6C RID: 10860 RVA: 0x000D12C7 File Offset: 0x000CF4C7
	public static implicit operator bool(OwnerRig or)
	{
		return or != null && !(or == null) && or._rig != null && !(or._rig == null);
	}

	// Token: 0x06002A6D RID: 10861 RVA: 0x000D12F4 File Offset: 0x000CF4F4
	public static implicit operator VRRig(OwnerRig or)
	{
		if (!or)
		{
			return null;
		}
		return or._rig;
	}

	// Token: 0x04002F54 RID: 12116
	[SerializeField]
	private VRRig _rig;
}
