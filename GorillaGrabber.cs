using System;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000681 RID: 1665
public class GorillaGrabber : MonoBehaviour
{
	// Token: 0x170003FD RID: 1021
	// (get) Token: 0x06002985 RID: 10629 RVA: 0x000CE1E8 File Offset: 0x000CC3E8
	public XRNode XrNode
	{
		get
		{
			return this.xrNode;
		}
	}

	// Token: 0x170003FE RID: 1022
	// (get) Token: 0x06002986 RID: 10630 RVA: 0x000CE1F0 File Offset: 0x000CC3F0
	public bool IsLeftHand
	{
		get
		{
			return this.XrNode == XRNode.LeftHand;
		}
	}

	// Token: 0x170003FF RID: 1023
	// (get) Token: 0x06002987 RID: 10631 RVA: 0x000CE1FB File Offset: 0x000CC3FB
	public bool IsRightHand
	{
		get
		{
			return this.XrNode == XRNode.RightHand;
		}
	}

	// Token: 0x17000400 RID: 1024
	// (get) Token: 0x06002988 RID: 10632 RVA: 0x000CE206 File Offset: 0x000CC406
	public GTPlayer Player
	{
		get
		{
			return this.player;
		}
	}

	// Token: 0x06002989 RID: 10633 RVA: 0x000CE210 File Offset: 0x000CC410
	private void Start()
	{
		ControllerInputPoller.AddUpdateCallback(new Action(this.OnControllerUpdate));
		this.hapticStrengthActual = this.hapticStrength;
		this.audioSource = base.GetComponent<AudioSource>();
		this.player = base.GetComponentInParent<GTPlayer>();
		if (!this.player)
		{
			Debug.LogWarning("Gorilla Grabber Component has no player in hierarchy. Disabling this Gorilla Grabber");
			base.GetComponent<GorillaGrabber>().enabled = false;
		}
	}

	// Token: 0x0600298A RID: 10634 RVA: 0x000CE278 File Offset: 0x000CC478
	private void OnControllerUpdate()
	{
		bool grab = ControllerInputPoller.GetGrab(this.xrNode);
		bool grabMomentary = ControllerInputPoller.GetGrabMomentary(this.xrNode);
		bool grabRelease = ControllerInputPoller.GetGrabRelease(this.xrNode);
		if (this.currentGrabbable != null && (grabRelease || this.GrabDistanceOverCheck()))
		{
			this.Ungrab(null);
		}
		if (grabMomentary)
		{
			this.grabTimeStamp = Time.time;
		}
		if (grab && this.currentGrabbable == null)
		{
			this.currentGrabbable = this.TryGrab(Time.time - this.grabTimeStamp < this.coyoteTimeDuration);
		}
		if (this.currentGrabbable != null && this.hapticStrengthActual > 0f)
		{
			GorillaTagger.Instance.DoVibration(this.xrNode, this.hapticStrengthActual, Time.deltaTime);
			this.hapticStrengthActual -= this.hapticDecay * Time.deltaTime;
		}
	}

	// Token: 0x0600298B RID: 10635 RVA: 0x000CE341 File Offset: 0x000CC541
	private bool GrabDistanceOverCheck()
	{
		return this.currentGrabbedTransform == null || Vector3.Distance(base.transform.position, this.currentGrabbedTransform.TransformPoint(this.localGrabbedPosition)) > this.breakDistance;
	}

	// Token: 0x0600298C RID: 10636 RVA: 0x000CE37C File Offset: 0x000CC57C
	internal void Ungrab(IGorillaGrabable specificGrabbable = null)
	{
		if (specificGrabbable != null && specificGrabbable != this.currentGrabbable)
		{
			return;
		}
		this.currentGrabbable.OnGrabReleased(this);
		PlayerGameEvents.DroppedObject(this.currentGrabbable.name);
		this.currentGrabbable = null;
		this.gripEffects.Stop();
		this.hapticStrengthActual = this.hapticStrength;
	}

	// Token: 0x0600298D RID: 10637 RVA: 0x000CE3D0 File Offset: 0x000CC5D0
	private IGorillaGrabable TryGrab(bool momentary)
	{
		IGorillaGrabable gorillaGrabable = null;
		Debug.DrawRay(base.transform.position, base.transform.forward * (this.grabRadius * this.player.scale), Color.blue, 1f);
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.grabRadius * this.player.scale, this.grabCastResults);
		float num2 = float.MaxValue;
		for (int i = 0; i < num; i++)
		{
			IGorillaGrabable gorillaGrabable2;
			if (this.grabCastResults[i].TryGetComponent<IGorillaGrabable>(out gorillaGrabable2))
			{
				float num3 = Vector3.Distance(base.transform.position, this.FindClosestPoint(this.grabCastResults[i], base.transform.position));
				if (num3 < num2)
				{
					num2 = num3;
					gorillaGrabable = gorillaGrabable2;
				}
			}
		}
		if (gorillaGrabable != null && (!gorillaGrabable.MomentaryGrabOnly() || momentary) && gorillaGrabable.CanBeGrabbed(this))
		{
			gorillaGrabable.OnGrabbed(this, out this.currentGrabbedTransform, out this.localGrabbedPosition);
			PlayerGameEvents.GrabbedObject(gorillaGrabable.name);
		}
		if (gorillaGrabable != null && !gorillaGrabable.CanBeGrabbed(this))
		{
			gorillaGrabable = null;
		}
		return gorillaGrabable;
	}

	// Token: 0x0600298E RID: 10638 RVA: 0x000CE4E3 File Offset: 0x000CC6E3
	private Vector3 FindClosestPoint(Collider collider, Vector3 position)
	{
		if (collider is MeshCollider && !(collider as MeshCollider).convex)
		{
			return position;
		}
		return collider.ClosestPoint(position);
	}

	// Token: 0x0600298F RID: 10639 RVA: 0x000CE504 File Offset: 0x000CC704
	public void Inject(Transform currentGrabbableTransform, Vector3 localGrabbedPosition)
	{
		if (this.currentGrabbable != null)
		{
			this.Ungrab(null);
		}
		if (currentGrabbableTransform != null)
		{
			this.currentGrabbable = currentGrabbableTransform.GetComponent<IGorillaGrabable>();
			this.currentGrabbedTransform = currentGrabbableTransform;
			this.localGrabbedPosition = localGrabbedPosition;
			this.currentGrabbable.OnGrabbed(this, out this.currentGrabbedTransform, out localGrabbedPosition);
		}
	}

	// Token: 0x04002E95 RID: 11925
	private GTPlayer player;

	// Token: 0x04002E96 RID: 11926
	[SerializeField]
	private XRNode xrNode = XRNode.LeftHand;

	// Token: 0x04002E97 RID: 11927
	private AudioSource audioSource;

	// Token: 0x04002E98 RID: 11928
	private Transform currentGrabbedTransform;

	// Token: 0x04002E99 RID: 11929
	private Vector3 localGrabbedPosition;

	// Token: 0x04002E9A RID: 11930
	private IGorillaGrabable currentGrabbable;

	// Token: 0x04002E9B RID: 11931
	[SerializeField]
	private float grabRadius = 0.015f;

	// Token: 0x04002E9C RID: 11932
	[SerializeField]
	private float breakDistance = 0.3f;

	// Token: 0x04002E9D RID: 11933
	[SerializeField]
	private float hapticStrength = 0.2f;

	// Token: 0x04002E9E RID: 11934
	private float hapticStrengthActual = 0.2f;

	// Token: 0x04002E9F RID: 11935
	[SerializeField]
	private float hapticDecay;

	// Token: 0x04002EA0 RID: 11936
	[SerializeField]
	private ParticleSystem gripEffects;

	// Token: 0x04002EA1 RID: 11937
	private Collider[] grabCastResults = new Collider[32];

	// Token: 0x04002EA2 RID: 11938
	private float grabTimeStamp;

	// Token: 0x04002EA3 RID: 11939
	[SerializeField]
	private float coyoteTimeDuration = 0.25f;
}
