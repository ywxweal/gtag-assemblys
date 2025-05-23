using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004CD RID: 1229
public class Breakable : MonoBehaviour
{
	// Token: 0x06001DCB RID: 7627 RVA: 0x00090D70 File Offset: 0x0008EF70
	private void Awake()
	{
		this._breakSignal.OnSignal += this.BreakRPC;
	}

	// Token: 0x06001DCC RID: 7628 RVA: 0x00090D8C File Offset: 0x0008EF8C
	private void BreakRPC(int owner, PhotonSignalInfo info)
	{
		VRRig vrrig = base.GetComponent<OwnerRig>();
		if (vrrig == null)
		{
			return;
		}
		if (vrrig.OwningNetPlayer.ActorNumber != owner)
		{
			return;
		}
		this.OnBreak(true, false);
	}

	// Token: 0x06001DCD RID: 7629 RVA: 0x00090DC8 File Offset: 0x0008EFC8
	private void Setup()
	{
		if (this._collider == null)
		{
			SphereCollider sphereCollider;
			this.GetOrAddComponent(out sphereCollider);
			this._collider = sphereCollider;
		}
		this._collider.enabled = true;
		if (this._rigidbody == null)
		{
			this.GetOrAddComponent(out this._rigidbody);
		}
		this._rigidbody.isKinematic = false;
		this._rigidbody.useGravity = false;
		this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		this.UpdatePhysMasks();
		if (this.rendererRoot == null)
		{
			this._renderers = base.GetComponentsInChildren<Renderer>();
			return;
		}
		this._renderers = this.rendererRoot.GetComponentsInChildren<Renderer>();
	}

	// Token: 0x06001DCE RID: 7630 RVA: 0x00090E6F File Offset: 0x0008F06F
	private void OnCollisionEnter(Collision col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x06001DCF RID: 7631 RVA: 0x00090E6F File Offset: 0x0008F06F
	private void OnCollisionStay(Collision col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x06001DD0 RID: 7632 RVA: 0x00090E6F File Offset: 0x0008F06F
	private void OnTriggerEnter(Collider col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x06001DD1 RID: 7633 RVA: 0x00090E6F File Offset: 0x0008F06F
	private void OnTriggerStay(Collider col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x06001DD2 RID: 7634 RVA: 0x00090E79 File Offset: 0x0008F079
	private void OnEnable()
	{
		this._breakSignal.Enable();
		this._broken = false;
		this.OnSpawn(true);
	}

	// Token: 0x06001DD3 RID: 7635 RVA: 0x00090E94 File Offset: 0x0008F094
	private void OnDisable()
	{
		this._breakSignal.Disable();
		this._broken = false;
		this.OnReset(false);
		this.ShowRenderers(false);
	}

	// Token: 0x06001DD4 RID: 7636 RVA: 0x00090E6F File Offset: 0x0008F06F
	public void Break()
	{
		this.OnBreak(true, true);
	}

	// Token: 0x06001DD5 RID: 7637 RVA: 0x00090EB6 File Offset: 0x0008F0B6
	public void Reset()
	{
		this.OnReset(true);
	}

	// Token: 0x06001DD6 RID: 7638 RVA: 0x00090EC0 File Offset: 0x0008F0C0
	protected virtual void ShowRenderers(bool visible)
	{
		if (this._renderers.IsNullOrEmpty<Renderer>())
		{
			return;
		}
		for (int i = 0; i < this._renderers.Length; i++)
		{
			Renderer renderer = this._renderers[i];
			if (renderer)
			{
				renderer.forceRenderingOff = !visible;
			}
		}
	}

	// Token: 0x06001DD7 RID: 7639 RVA: 0x00090F0C File Offset: 0x0008F10C
	protected virtual void OnReset(bool callback = true)
	{
		if (this._breakEffect && this._breakEffect.isPlaying)
		{
			this._breakEffect.Stop();
		}
		this.ShowRenderers(true);
		this._broken = false;
		if (callback)
		{
			UnityEvent<Breakable> unityEvent = this.onReset;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	// Token: 0x06001DD8 RID: 7640 RVA: 0x00090F60 File Offset: 0x0008F160
	protected virtual void OnSpawn(bool callback = true)
	{
		this.startTime = Time.time;
		this.endTime = this.startTime + this.canBreakDelay;
		this.ShowRenderers(true);
		if (callback)
		{
			UnityEvent<Breakable> unityEvent = this.onSpawn;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	// Token: 0x06001DD9 RID: 7641 RVA: 0x00090F9C File Offset: 0x0008F19C
	protected virtual void OnBreak(bool callback = true, bool signal = true)
	{
		if (this._broken)
		{
			return;
		}
		if (Time.time < this.endTime)
		{
			return;
		}
		if (this._breakEffect)
		{
			if (this._breakEffect.isPlaying)
			{
				this._breakEffect.Stop();
			}
			this._breakEffect.Play();
		}
		if (signal && PhotonNetwork.InRoom)
		{
			VRRig vrrig = base.GetComponent<OwnerRig>();
			if (vrrig != null)
			{
				this._breakSignal.Raise(vrrig.OwningNetPlayer.ActorNumber);
			}
		}
		this.ShowRenderers(false);
		this._broken = true;
		if (callback)
		{
			UnityEvent<Breakable> unityEvent = this.onBreak;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	// Token: 0x06001DDA RID: 7642 RVA: 0x00091048 File Offset: 0x0008F248
	private void UpdatePhysMasks()
	{
		int physicsMask = (int)this._physicsMask;
		if (this._collider)
		{
			this._collider.includeLayers = physicsMask;
			this._collider.excludeLayers = ~physicsMask;
		}
		if (this._rigidbody)
		{
			this._rigidbody.includeLayers = physicsMask;
			this._rigidbody.excludeLayers = ~physicsMask;
		}
	}

	// Token: 0x040020E9 RID: 8425
	[SerializeField]
	private Collider _collider;

	// Token: 0x040020EA RID: 8426
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x040020EB RID: 8427
	[SerializeField]
	private GameObject rendererRoot;

	// Token: 0x040020EC RID: 8428
	[SerializeField]
	private Renderer[] _renderers = new Renderer[0];

	// Token: 0x040020ED RID: 8429
	[Space]
	[SerializeField]
	private ParticleSystem _breakEffect;

	// Token: 0x040020EE RID: 8430
	[SerializeField]
	private UnityLayerMask _physicsMask = UnityLayerMask.GorillaHand;

	// Token: 0x040020EF RID: 8431
	public UnityEvent<Breakable> onSpawn;

	// Token: 0x040020F0 RID: 8432
	public UnityEvent<Breakable> onBreak;

	// Token: 0x040020F1 RID: 8433
	public UnityEvent<Breakable> onReset;

	// Token: 0x040020F2 RID: 8434
	public float canBreakDelay = 1f;

	// Token: 0x040020F3 RID: 8435
	[SerializeField]
	private PhotonSignal<int> _breakSignal = "_breakSignal";

	// Token: 0x040020F4 RID: 8436
	[Space]
	[NonSerialized]
	private bool _broken;

	// Token: 0x040020F5 RID: 8437
	private float startTime;

	// Token: 0x040020F6 RID: 8438
	private float endTime;
}
