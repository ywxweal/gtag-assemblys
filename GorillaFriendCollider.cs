using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020008DA RID: 2266
public class GorillaFriendCollider : MonoBehaviour
{
	// Token: 0x06003721 RID: 14113 RVA: 0x0010ABC0 File Offset: 0x00108DC0
	public void Awake()
	{
		this.thisCapsule = base.GetComponent<CapsuleCollider>();
		this.thisBox = base.GetComponent<BoxCollider>();
		this.jiggleAmount = Random.Range(0f, 1f);
		this.tagAndBodyLayerMask = LayerMask.GetMask(new string[] { "Gorilla Tag Collider" }) | LayerMask.GetMask(new string[] { "Gorilla Body Collider" });
	}

	// Token: 0x06003722 RID: 14114 RVA: 0x0010AC27 File Offset: 0x00108E27
	public void OnEnable()
	{
		base.StartCoroutine(this.UpdatePlayersInSphere());
	}

	// Token: 0x06003723 RID: 14115 RVA: 0x0010AC36 File Offset: 0x00108E36
	public void OnDisable()
	{
		base.StopCoroutine(this.UpdatePlayersInSphere());
	}

	// Token: 0x06003724 RID: 14116 RVA: 0x0010AC44 File Offset: 0x00108E44
	private void AddUserID(in string userID)
	{
		if (this.playerIDsCurrentlyTouching.Contains(userID))
		{
			return;
		}
		this.playerIDsCurrentlyTouching.Add(userID);
	}

	// Token: 0x06003725 RID: 14117 RVA: 0x0010AC63 File Offset: 0x00108E63
	private IEnumerator UpdatePlayersInSphere()
	{
		yield return new WaitForSeconds(1f + this.jiggleAmount);
		for (;;)
		{
			if (!NetworkSystem.Instance.InRoom && !this.runCheckWhileNotInRoom)
			{
				yield return this.wait1Sec;
			}
			else
			{
				this.playerIDsCurrentlyTouching.Clear();
				if (this.thisBox != null)
				{
					this.collisions = Physics.OverlapBoxNonAlloc(this.thisBox.transform.position, this.thisBox.size / 2f, this.overlapColliders, this.thisBox.transform.rotation, this.tagAndBodyLayerMask);
				}
				else
				{
					this.collisions = Physics.OverlapSphereNonAlloc(base.transform.position, this.thisCapsule.radius, this.overlapColliders, this.tagAndBodyLayerMask);
				}
				this.collisions = Mathf.Min(this.collisions, this.overlapColliders.Length);
				if (this.collisions > 0)
				{
					for (int i = 0; i < this.collisions; i++)
					{
						this.otherCollider = this.overlapColliders[i];
						if (!(this.otherCollider == null))
						{
							this.otherColliderGO = this.otherCollider.attachedRigidbody.gameObject;
							this.collidingRig = this.otherColliderGO.GetComponent<VRRig>();
							if (this.collidingRig == null || this.collidingRig.creator == null || this.collidingRig.creator.IsNull || string.IsNullOrEmpty(this.collidingRig.creator.UserId))
							{
								GTPlayer component = this.otherColliderGO.GetComponent<GTPlayer>();
								if (component == null || NetworkSystem.Instance.LocalPlayer == null)
								{
									goto IL_02D9;
								}
								if (this.thisCapsule != null && this.applyCapsuleYLimits)
								{
									float y = component.bodyCollider.transform.position.y;
									if (y < this.capsuleColliderYLimits.x || y > this.capsuleColliderYLimits.y)
									{
										goto IL_02D9;
									}
								}
								string text = NetworkSystem.Instance.LocalPlayer.UserId;
								this.AddUserID(in text);
							}
							else
							{
								if (this.thisCapsule != null && this.applyCapsuleYLimits)
								{
									float y2 = this.collidingRig.bodyTransform.transform.position.y;
									if (y2 < this.capsuleColliderYLimits.x || y2 > this.capsuleColliderYLimits.y)
									{
										goto IL_02D9;
									}
								}
								string text = this.collidingRig.creator.UserId;
								this.AddUserID(in text);
							}
							this.overlapColliders[i] = null;
						}
						IL_02D9:;
					}
					if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.LocalPlayer != null && this.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId) && GorillaComputer.instance.friendJoinCollider != this)
					{
						GorillaComputer.instance.allowedMapsToJoin = this.myAllowedMapsToJoin;
						GorillaComputer.instance.friendJoinCollider = this;
						GorillaComputer.instance.UpdateScreen();
					}
					this.otherCollider = null;
					this.otherColliderGO = null;
					this.collidingRig = null;
				}
				yield return this.wait1Sec;
			}
		}
		yield break;
	}

	// Token: 0x04003CA8 RID: 15528
	public List<string> playerIDsCurrentlyTouching = new List<string>();

	// Token: 0x04003CA9 RID: 15529
	private CapsuleCollider thisCapsule;

	// Token: 0x04003CAA RID: 15530
	private BoxCollider thisBox;

	// Token: 0x04003CAB RID: 15531
	[Tooltip("If using a capsule collider, the player position can be checked against these minimum and maximum Y limits (world position) to make it behave more like a cylinder check")]
	public bool applyCapsuleYLimits;

	// Token: 0x04003CAC RID: 15532
	[Tooltip("If the player's Y world position is lower than Limits.x or higher than Limits.y, they will not be considered \"Inside\" the friend collider")]
	public Vector2 capsuleColliderYLimits = Vector2.zero;

	// Token: 0x04003CAD RID: 15533
	public bool runCheckWhileNotInRoom;

	// Token: 0x04003CAE RID: 15534
	public string[] myAllowedMapsToJoin;

	// Token: 0x04003CAF RID: 15535
	private readonly Collider[] overlapColliders = new Collider[20];

	// Token: 0x04003CB0 RID: 15536
	private int tagAndBodyLayerMask;

	// Token: 0x04003CB1 RID: 15537
	private float jiggleAmount;

	// Token: 0x04003CB2 RID: 15538
	private Collider otherCollider;

	// Token: 0x04003CB3 RID: 15539
	private GameObject otherColliderGO;

	// Token: 0x04003CB4 RID: 15540
	private VRRig collidingRig;

	// Token: 0x04003CB5 RID: 15541
	private int collisions;

	// Token: 0x04003CB6 RID: 15542
	private WaitForSeconds wait1Sec = new WaitForSeconds(1f);
}
