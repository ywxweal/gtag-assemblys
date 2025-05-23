using System;
using UnityEngine;

// Token: 0x02000633 RID: 1587
public class GorillaPawn : MonoBehaviour
{
	// Token: 0x170003BA RID: 954
	// (get) Token: 0x0600277C RID: 10108 RVA: 0x000C3AA5 File Offset: 0x000C1CA5
	public VRRig rig
	{
		get
		{
			return this._rig;
		}
	}

	// Token: 0x170003BB RID: 955
	// (get) Token: 0x0600277D RID: 10109 RVA: 0x000C3AAD File Offset: 0x000C1CAD
	public ZoneEntity zoneEntity
	{
		get
		{
			return this._zoneEntity;
		}
	}

	// Token: 0x170003BC RID: 956
	// (get) Token: 0x0600277E RID: 10110 RVA: 0x000C3AB5 File Offset: 0x000C1CB5
	public new Transform transform
	{
		get
		{
			return this._transform;
		}
	}

	// Token: 0x170003BD RID: 957
	// (get) Token: 0x0600277F RID: 10111 RVA: 0x000C3ABD File Offset: 0x000C1CBD
	public XformNode handLeft
	{
		get
		{
			return this._handLeftXform;
		}
	}

	// Token: 0x170003BE RID: 958
	// (get) Token: 0x06002780 RID: 10112 RVA: 0x000C3AC5 File Offset: 0x000C1CC5
	public XformNode handRight
	{
		get
		{
			return this._handRightXform;
		}
	}

	// Token: 0x170003BF RID: 959
	// (get) Token: 0x06002781 RID: 10113 RVA: 0x000C3ACD File Offset: 0x000C1CCD
	public XformNode body
	{
		get
		{
			return this._bodyXform;
		}
	}

	// Token: 0x170003C0 RID: 960
	// (get) Token: 0x06002782 RID: 10114 RVA: 0x000C3AD5 File Offset: 0x000C1CD5
	public XformNode head
	{
		get
		{
			return this._headXform;
		}
	}

	// Token: 0x06002783 RID: 10115 RVA: 0x000C3ADD File Offset: 0x000C1CDD
	private void Awake()
	{
		this.Setup(false);
	}

	// Token: 0x06002784 RID: 10116 RVA: 0x000C3AE8 File Offset: 0x000C1CE8
	private void Setup(bool force)
	{
		this._transform = base.transform;
		this._rig = base.GetComponentInChildren<VRRig>();
		if (!this._rig)
		{
			return;
		}
		this._zoneEntity = this._rig.zoneEntity;
		if (this._zoneEntity)
		{
			if (this._bodyXform == null)
			{
				this._bodyXform = new XformNode();
			}
			this._bodyXform.localPosition = this._zoneEntity.collider.center;
			this._bodyXform.radius = this._zoneEntity.collider.radius;
			this._bodyXform.parent = this._transform;
		}
		bool flag = force || this._handLeft.AsNull<Transform>() == null;
		bool flag2 = force || this._handRight.AsNull<Transform>() == null;
		bool flag3 = force || this._head.AsNull<Transform>() == null;
		if (!flag && !flag2 && !flag3)
		{
			return;
		}
		foreach (Transform transform in this._rig.mainSkin.bones)
		{
			string name = transform.name;
			if (flag3 && name.StartsWith("head", StringComparison.OrdinalIgnoreCase))
			{
				this._head = transform;
				this._headXform = new XformNode();
				this._headXform.localPosition = new Vector3(0f, 0.13f, 0.015f);
				this._headXform.radius = 0.12f;
				this._headXform.parent = transform;
			}
			else if (flag && name.StartsWith("hand.L", StringComparison.OrdinalIgnoreCase))
			{
				this._handLeft = transform;
				this._handLeftXform = new XformNode();
				this._handLeftXform.localPosition = new Vector3(-0.014f, 0.034f, 0f);
				this._handLeftXform.radius = 0.044f;
				this._handLeftXform.parent = transform;
			}
			else if (flag2 && name.StartsWith("hand.R", StringComparison.OrdinalIgnoreCase))
			{
				this._handRight = transform;
				this._handRightXform = new XformNode();
				this._handRightXform.localPosition = new Vector3(0.014f, 0.034f, 0f);
				this._handRightXform.radius = 0.044f;
				this._handRightXform.parent = transform;
			}
		}
	}

	// Token: 0x06002785 RID: 10117 RVA: 0x000C3D5B File Offset: 0x000C1F5B
	private bool CanRun()
	{
		if (GorillaPawn._gPawnActiveCount > 10)
		{
			Debug.LogError(string.Format("Cannot register more than {0} pawns.", 10));
			return false;
		}
		return true;
	}

	// Token: 0x06002786 RID: 10118 RVA: 0x000C3D80 File Offset: 0x000C1F80
	private void OnEnable()
	{
		if (!this.CanRun())
		{
			return;
		}
		this._id = -1;
		if (this._rig && this._rig.OwningNetPlayer != null)
		{
			this._id = this._rig.OwningNetPlayer.ActorNumber;
		}
		this._index = GorillaPawn._gPawnActiveCount++;
		GorillaPawn._gPawns[this._index] = this;
	}

	// Token: 0x06002787 RID: 10119 RVA: 0x000C3DF0 File Offset: 0x000C1FF0
	private void OnDisable()
	{
		this._id = -1;
		if (!this.CanRun())
		{
			return;
		}
		if (this._index < 0 || this._index >= GorillaPawn._gPawnActiveCount - 1)
		{
			return;
		}
		int num = --GorillaPawn._gPawnActiveCount;
		GorillaPawn._gPawns.Swap(this._index, num);
		this._index = num;
	}

	// Token: 0x06002788 RID: 10120 RVA: 0x000C3E4C File Offset: 0x000C204C
	private void OnDestroy()
	{
		int num = GorillaPawn._gPawns.IndexOfRef(this);
		GorillaPawn._gPawns[num] = null;
		Array.Sort<GorillaPawn>(GorillaPawn._gPawns, new Comparison<GorillaPawn>(GorillaPawn.ComparePawns));
		int num2 = 0;
		while (num2 < GorillaPawn._gPawns.Length && GorillaPawn._gPawns[num2])
		{
			num2++;
		}
		GorillaPawn._gPawnActiveCount = num2;
	}

	// Token: 0x06002789 RID: 10121 RVA: 0x000C3EAC File Offset: 0x000C20AC
	private static int ComparePawns(GorillaPawn x, GorillaPawn y)
	{
		bool flag = x.AsNull<GorillaPawn>() == null;
		bool flag2 = y.AsNull<GorillaPawn>() == null;
		if (flag && flag2)
		{
			return 0;
		}
		if (flag)
		{
			return 1;
		}
		if (flag2)
		{
			return -1;
		}
		return x._index.CompareTo(y._index);
	}

	// Token: 0x170003C1 RID: 961
	// (get) Token: 0x0600278A RID: 10122 RVA: 0x000C3EF5 File Offset: 0x000C20F5
	public static GorillaPawn[] AllPawns
	{
		get
		{
			return GorillaPawn._gPawns;
		}
	}

	// Token: 0x170003C2 RID: 962
	// (get) Token: 0x0600278B RID: 10123 RVA: 0x000C3EFC File Offset: 0x000C20FC
	public static int ActiveCount
	{
		get
		{
			return GorillaPawn._gPawnActiveCount;
		}
	}

	// Token: 0x170003C3 RID: 963
	// (get) Token: 0x0600278C RID: 10124 RVA: 0x000C3F03 File Offset: 0x000C2103
	public static Matrix4x4[] ShaderData
	{
		get
		{
			return GorillaPawn._gShaderData;
		}
	}

	// Token: 0x0600278D RID: 10125 RVA: 0x000C3F0C File Offset: 0x000C210C
	public static void SyncPawnData()
	{
		Matrix4x4[] gShaderData = GorillaPawn._gShaderData;
		m4x4 m4x = default(m4x4);
		for (int i = 0; i < GorillaPawn._gPawnActiveCount; i++)
		{
			GorillaPawn gorillaPawn = GorillaPawn._gPawns[i];
			Vector4 worldPosition = gorillaPawn._headXform.worldPosition;
			Vector4 worldPosition2 = gorillaPawn._bodyXform.worldPosition;
			Vector4 worldPosition3 = gorillaPawn._handLeftXform.worldPosition;
			Vector4 worldPosition4 = gorillaPawn._handRightXform.worldPosition;
			m4x.SetRow0(ref worldPosition);
			m4x.SetRow1(ref worldPosition2);
			m4x.SetRow2(ref worldPosition3);
			m4x.SetRow3(ref worldPosition4);
			m4x.Push(ref gShaderData[i]);
		}
		for (int j = GorillaPawn._gPawnActiveCount; j < 10; j++)
		{
			MatrixUtils.Clear(ref gShaderData[j]);
		}
	}

	// Token: 0x04002BF8 RID: 11256
	[SerializeField]
	private Transform _transform;

	// Token: 0x04002BF9 RID: 11257
	[SerializeField]
	private Transform _handLeft;

	// Token: 0x04002BFA RID: 11258
	[SerializeField]
	private Transform _handRight;

	// Token: 0x04002BFB RID: 11259
	[SerializeField]
	private Transform _head;

	// Token: 0x04002BFC RID: 11260
	[Space]
	[SerializeField]
	private VRRig _rig;

	// Token: 0x04002BFD RID: 11261
	[SerializeField]
	private ZoneEntity _zoneEntity;

	// Token: 0x04002BFE RID: 11262
	[Space]
	[SerializeField]
	private XformNode _handLeftXform;

	// Token: 0x04002BFF RID: 11263
	[SerializeField]
	private XformNode _handRightXform;

	// Token: 0x04002C00 RID: 11264
	[SerializeField]
	private XformNode _bodyXform;

	// Token: 0x04002C01 RID: 11265
	[SerializeField]
	private XformNode _headXform;

	// Token: 0x04002C02 RID: 11266
	[Space]
	private int _id;

	// Token: 0x04002C03 RID: 11267
	private int _index;

	// Token: 0x04002C04 RID: 11268
	private bool _invalid;

	// Token: 0x04002C05 RID: 11269
	public const int MAX_PAWNS = 10;

	// Token: 0x04002C06 RID: 11270
	private static GorillaPawn[] _gPawns = new GorillaPawn[10];

	// Token: 0x04002C07 RID: 11271
	private static int _gPawnActiveCount = 0;

	// Token: 0x04002C08 RID: 11272
	private static Matrix4x4[] _gShaderData = new Matrix4x4[10];
}
