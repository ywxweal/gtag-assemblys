using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x0200009C RID: 156
public class EyeScannableMono : MonoBehaviour, IEyeScannable
{
	// Token: 0x1400000B RID: 11
	// (add) Token: 0x060003D7 RID: 983 RVA: 0x00017394 File Offset: 0x00015594
	// (remove) Token: 0x060003D8 RID: 984 RVA: 0x000173CC File Offset: 0x000155CC
	public event Action OnDataChange;

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x060003D9 RID: 985 RVA: 0x00017401 File Offset: 0x00015601
	int IEyeScannable.scannableId
	{
		get
		{
			return base.GetInstanceID();
		}
	}

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x060003DA RID: 986 RVA: 0x00017409 File Offset: 0x00015609
	Vector3 IEyeScannable.Position
	{
		get
		{
			return base.transform.position - this._initialPosition + this._bounds.center;
		}
	}

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x060003DB RID: 987 RVA: 0x00017431 File Offset: 0x00015631
	Bounds IEyeScannable.Bounds
	{
		get
		{
			return this._bounds;
		}
	}

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x060003DC RID: 988 RVA: 0x00017439 File Offset: 0x00015639
	IList<KeyValueStringPair> IEyeScannable.Entries
	{
		get
		{
			return this.data.Entries;
		}
	}

	// Token: 0x060003DD RID: 989 RVA: 0x00017446 File Offset: 0x00015646
	private void Awake()
	{
		this.RecalculateBounds();
	}

	// Token: 0x060003DE RID: 990 RVA: 0x0001744E File Offset: 0x0001564E
	public void OnEnable()
	{
		this.RecalculateBoundsLater();
		EyeScannerMono.Register(this);
	}

	// Token: 0x060003DF RID: 991 RVA: 0x000137A9 File Offset: 0x000119A9
	public void OnDisable()
	{
		EyeScannerMono.Unregister(this);
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x0001745C File Offset: 0x0001565C
	private async void RecalculateBoundsLater()
	{
		await Task.Delay(100);
		this.RecalculateBounds();
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x00017494 File Offset: 0x00015694
	private void RecalculateBounds()
	{
		this._initialPosition = base.transform.position;
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		this._bounds = default(Bounds);
		if (componentsInChildren.Length == 0)
		{
			this._bounds.center = base.transform.position;
			this._bounds.Expand(1f);
			return;
		}
		this._bounds = componentsInChildren[0].bounds;
		for (int i = 1; i < componentsInChildren.Length; i++)
		{
			this._bounds.Encapsulate(componentsInChildren[i].bounds);
		}
	}

	// Token: 0x0400045A RID: 1114
	[SerializeField]
	private KeyValuePairSet data;

	// Token: 0x0400045B RID: 1115
	private Bounds _bounds;

	// Token: 0x0400045C RID: 1116
	private Vector3 _initialPosition;
}
