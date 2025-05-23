using System;
using GorillaTag.Reactions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000463 RID: 1123
public class PaperPlaneProjectile : MonoBehaviour
{
	// Token: 0x1400004B RID: 75
	// (add) Token: 0x06001B93 RID: 7059 RVA: 0x000875F4 File Offset: 0x000857F4
	// (remove) Token: 0x06001B94 RID: 7060 RVA: 0x0008762C File Offset: 0x0008582C
	public event PaperPlaneProjectile.PaperPlaneHit OnHit;

	// Token: 0x170002FD RID: 765
	// (get) Token: 0x06001B95 RID: 7061 RVA: 0x00087661 File Offset: 0x00085861
	public new Transform transform
	{
		get
		{
			return this._tCached;
		}
	}

	// Token: 0x170002FE RID: 766
	// (get) Token: 0x06001B96 RID: 7062 RVA: 0x00087669 File Offset: 0x00085869
	public VRRig MyRig
	{
		get
		{
			return this.myRig;
		}
	}

	// Token: 0x06001B97 RID: 7063 RVA: 0x00087671 File Offset: 0x00085871
	private void Awake()
	{
		this._tCached = base.transform;
		this.spawnWorldEffects = base.GetComponent<SpawnWorldEffects>();
	}

	// Token: 0x06001B98 RID: 7064 RVA: 0x0008768B File Offset: 0x0008588B
	private void Start()
	{
		this.ResetProjectile();
	}

	// Token: 0x06001B99 RID: 7065 RVA: 0x00087693 File Offset: 0x00085893
	public void ResetProjectile()
	{
		this._timeElapsed = 0f;
		this.flyingObject.SetActive(true);
		this.crashingObject.SetActive(false);
	}

	// Token: 0x06001B9A RID: 7066 RVA: 0x000876B8 File Offset: 0x000858B8
	public void Launch(Vector3 startPos, Quaternion startRot, Vector3 vel)
	{
		base.gameObject.SetActive(true);
		this.ResetProjectile();
		this.transform.position = startPos;
		if (this.enableRotation)
		{
			this.transform.rotation = startRot;
		}
		else
		{
			this.transform.LookAt(this.transform.position + vel.normalized);
		}
		this._direction = vel.normalized;
		this._speed = Mathf.Clamp(vel.sqrMagnitude / 2f, this.minSpeed, this.maxSpeed);
		this._stopped = false;
		this.scaleFactor = 0.7f * (this.transform.lossyScale.x - 1f + 1.4285715f);
	}

	// Token: 0x06001B9B RID: 7067 RVA: 0x0008777C File Offset: 0x0008597C
	private void Update()
	{
		if (this._stopped)
		{
			if (!this.crashingObject.gameObject.activeSelf)
			{
				if (ObjectPools.instance)
				{
					ObjectPools.instance.Destroy(base.gameObject);
					return;
				}
				base.gameObject.SetActive(false);
			}
			return;
		}
		this._timeElapsed += Time.deltaTime;
		this.nextPos = this.transform.position + this._direction * this._speed * Time.deltaTime * this.scaleFactor;
		if (this._timeElapsed < this.maxFlightTime && (this._timeElapsed < this.minFlightTime || Physics.RaycastNonAlloc(this.transform.position, this.nextPos - this.transform.position, this.results, Vector3.Distance(this.transform.position, this.nextPos), this.layerMask.value) == 0))
		{
			this.transform.position = this.nextPos;
			this.transform.Rotate(Mathf.Sin(this._timeElapsed) * 10f * Time.deltaTime, 0f, 0f);
			return;
		}
		if (this._timeElapsed < this.maxFlightTime)
		{
			SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
			if (this.results[0].collider.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
			{
				slingshotProjectileHitNotifier.InvokeHit(this, this.results[0].collider);
			}
			if (this.spawnWorldEffects != null)
			{
				this.spawnWorldEffects.RequestSpawn(this.nextPos);
			}
		}
		this._stopped = true;
		this._timeElapsed = 0f;
		PaperPlaneProjectile.PaperPlaneHit onHit = this.OnHit;
		if (onHit != null)
		{
			onHit(this.nextPos);
		}
		this.OnHit = null;
		this.flyingObject.SetActive(false);
		this.crashingObject.SetActive(true);
	}

	// Token: 0x06001B9C RID: 7068 RVA: 0x0008796E File Offset: 0x00085B6E
	internal void SetVRRig(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x04001E95 RID: 7829
	private const float speedScaleRatio = 0.7f;

	// Token: 0x04001E96 RID: 7830
	public Vector3 startPos;

	// Token: 0x04001E97 RID: 7831
	public Vector3 endPos;

	// Token: 0x04001E98 RID: 7832
	[FormerlySerializedAs("_flyTimeOut")]
	[Range(1f, 128f)]
	public float flyTimeOut = 32f;

	// Token: 0x04001E9A RID: 7834
	[Space]
	public float curveTime = 0.4f;

	// Token: 0x04001E9B RID: 7835
	[Space]
	public Vector3 curveDirection;

	// Token: 0x04001E9C RID: 7836
	public float curveDistance = 9.8f;

	// Token: 0x04001E9D RID: 7837
	[Space]
	[NonSerialized]
	private float _timeElapsed;

	// Token: 0x04001E9E RID: 7838
	[NonSerialized]
	private float _speed;

	// Token: 0x04001E9F RID: 7839
	[NonSerialized]
	private Vector3 _direction;

	// Token: 0x04001EA0 RID: 7840
	[NonSerialized]
	private bool _stopped;

	// Token: 0x04001EA1 RID: 7841
	private Transform _tCached;

	// Token: 0x04001EA2 RID: 7842
	private SpawnWorldEffects spawnWorldEffects;

	// Token: 0x04001EA3 RID: 7843
	private Vector3 nextPos;

	// Token: 0x04001EA4 RID: 7844
	private RaycastHit[] results = new RaycastHit[1];

	// Token: 0x04001EA5 RID: 7845
	[SerializeField]
	private float maxFlightTime = 7.5f;

	// Token: 0x04001EA6 RID: 7846
	[SerializeField]
	private float minFlightTime = 0.5f;

	// Token: 0x04001EA7 RID: 7847
	[SerializeField]
	private float maxSpeed = 10f;

	// Token: 0x04001EA8 RID: 7848
	[SerializeField]
	private float minSpeed = 1f;

	// Token: 0x04001EA9 RID: 7849
	[SerializeField]
	private bool enableRotation;

	// Token: 0x04001EAA RID: 7850
	[SerializeField]
	private GameObject flyingObject;

	// Token: 0x04001EAB RID: 7851
	[SerializeField]
	private GameObject crashingObject;

	// Token: 0x04001EAC RID: 7852
	[SerializeField]
	private LayerMask layerMask;

	// Token: 0x04001EAD RID: 7853
	private VRRig myRig;

	// Token: 0x04001EAE RID: 7854
	private float scaleFactor;

	// Token: 0x02000464 RID: 1124
	// (Invoke) Token: 0x06001B9F RID: 7071
	public delegate void PaperPlaneHit(Vector3 endPoint);
}
