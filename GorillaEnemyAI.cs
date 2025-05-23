using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020006EE RID: 1774
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class GorillaEnemyAI : MonoBehaviourPun, IPunObservable, IInRoomCallbacks
{
	// Token: 0x06002C31 RID: 11313 RVA: 0x000D9D58 File Offset: 0x000D7F58
	private void Start()
	{
		this.agent = base.GetComponent<NavMeshAgent>();
		this.r = base.GetComponent<Rigidbody>();
		this.r.useGravity = true;
		if (!base.photonView.IsMine)
		{
			this.agent.enabled = false;
			this.r.isKinematic = true;
		}
	}

	// Token: 0x06002C32 RID: 11314 RVA: 0x000D9DB0 File Offset: 0x000D7FB0
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.eulerAngles);
			return;
		}
		this.targetPosition = (Vector3)stream.ReceiveNext();
		this.targetRotation = (Vector3)stream.ReceiveNext();
	}

	// Token: 0x06002C33 RID: 11315 RVA: 0x000D9E14 File Offset: 0x000D8014
	private void Update()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.FindClosestPlayer();
			if (this.playerTransform != null)
			{
				this.agent.destination = this.playerTransform.position;
			}
			base.transform.LookAt(new Vector3(this.playerTransform.transform.position.x, base.transform.position.y, this.playerTransform.position.z));
			this.r.velocity *= 0.99f;
			return;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, this.lerpValue);
		base.transform.eulerAngles = Vector3.Lerp(base.transform.eulerAngles, this.targetRotation, this.lerpValue);
	}

	// Token: 0x06002C34 RID: 11316 RVA: 0x000D9F04 File Offset: 0x000D8104
	private void FindClosestPlayer()
	{
		VRRig[] array = Object.FindObjectsOfType<VRRig>();
		VRRig vrrig = null;
		float num = 100000f;
		foreach (VRRig vrrig2 in array)
		{
			Vector3 vector = vrrig2.transform.position - base.transform.position;
			if (vector.magnitude < num)
			{
				vrrig = vrrig2;
				num = vector.magnitude;
			}
		}
		this.playerTransform = vrrig.transform;
	}

	// Token: 0x06002C35 RID: 11317 RVA: 0x000D9F75 File Offset: 0x000D8175
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == 19)
		{
			PhotonNetwork.Destroy(base.photonView);
		}
	}

	// Token: 0x06002C36 RID: 11318 RVA: 0x000D9F91 File Offset: 0x000D8191
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.agent.enabled = true;
			this.r.isKinematic = false;
		}
	}

	// Token: 0x06002C37 RID: 11319 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06002C38 RID: 11320 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x06002C39 RID: 11321 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06002C3A RID: 11322 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x04003261 RID: 12897
	public Transform playerTransform;

	// Token: 0x04003262 RID: 12898
	private NavMeshAgent agent;

	// Token: 0x04003263 RID: 12899
	private Rigidbody r;

	// Token: 0x04003264 RID: 12900
	private Vector3 targetPosition;

	// Token: 0x04003265 RID: 12901
	private Vector3 targetRotation;

	// Token: 0x04003266 RID: 12902
	public float lerpValue;
}
