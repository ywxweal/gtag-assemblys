using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000D2 RID: 210
public class SecondLookSkeleton : MonoBehaviour
{
	// Token: 0x06000522 RID: 1314 RVA: 0x0001D6C8 File Offset: 0x0001B8C8
	private void Start()
	{
		this.playersSeen = new List<NetPlayer>();
		this.synchValues = base.GetComponent<SecondLookSkeletonSynchValues>();
		this.playerTransform = Camera.main.transform;
		this.tapped = !this.requireTappingToActivate;
		this.localCaught = false;
		this.audioSource = base.GetComponentInChildren<AudioSource>();
		this.spookyGhost.SetActive(false);
		this.angerPointIndex = Random.Range(0, this.angerPoint.Length);
		this.angerPointChangedTime = Time.time;
		this.synchValues.angerPoint = this.angerPointIndex;
		this.spookyGhost.transform.position = this.angerPoint[this.synchValues.angerPoint].position;
		this.spookyGhost.transform.rotation = this.angerPoint[this.synchValues.angerPoint].rotation;
		this.ChangeState(SecondLookSkeleton.GhostState.Unactivated);
		this.rHits = new RaycastHit[20];
		this.lookedAway = false;
		this.firstLookActivated = false;
		this.animator.Play("ArmsOut");
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x0001D7DD File Offset: 0x0001B9DD
	private void Update()
	{
		this.ProcessGhostState();
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x0001D7E8 File Offset: 0x0001B9E8
	public void ChangeState(SecondLookSkeleton.GhostState newState)
	{
		if (newState == this.currentState)
		{
			return;
		}
		switch (newState)
		{
		case SecondLookSkeleton.GhostState.Unactivated:
			this.spookyGhost.gameObject.SetActive(false);
			this.audioSource.GTStop();
			this.audioSource.loop = false;
			if (this.IsMine())
			{
				this.synchValues.angerPoint = Random.Range(0, this.angerPoint.Length);
				this.angerPointIndex = this.synchValues.angerPoint;
				this.angerPointChangedTime = Time.time;
				this.spookyGhost.transform.position = this.angerPoint[this.angerPointIndex].position;
				this.spookyGhost.transform.rotation = this.angerPoint[this.angerPointIndex].rotation;
			}
			this.currentState = SecondLookSkeleton.GhostState.Unactivated;
			return;
		case SecondLookSkeleton.GhostState.Activated:
			this.currentState = SecondLookSkeleton.GhostState.Activated;
			if (this.tapped)
			{
				GTAudioSourceExtensions.GTPlayClipAtPoint(this.initialScream, this.audioSource.transform.position, 1f);
				if (this.spookyText != null)
				{
					this.spookyText.SetActive(true);
				}
				this.spookyGhost.SetActive(true);
			}
			this.animator.Play("ArmsOut");
			this.spookyGhost.transform.rotation = Quaternion.LookRotation(this.playerTransform.position - this.spookyGhost.transform.position, Vector3.up);
			if (this.IsMine())
			{
				this.timeFirstAppeared = Time.time;
				return;
			}
			break;
		case SecondLookSkeleton.GhostState.Patrolling:
			this.playersSeen.Clear();
			if (this.tapped)
			{
				this.spookyGhost.SetActive(true);
				this.animator.Play("CrawlPatrol");
				this.audioSource.loop = true;
				this.audioSource.clip = this.patrolLoop;
				this.audioSource.GTPlay();
			}
			if (this.IsMine())
			{
				this.currentNode = this.pathPoints[Random.Range(0, this.pathPoints.Length)];
				this.nextNode = this.currentNode.connectedNodes[Random.Range(0, this.currentNode.connectedNodes.Length)];
				this.SyncNodes();
				this.spookyGhost.transform.position = this.currentNode.transform.position;
			}
			this.currentState = SecondLookSkeleton.GhostState.Patrolling;
			return;
		case SecondLookSkeleton.GhostState.Chasing:
			this.currentState = SecondLookSkeleton.GhostState.Chasing;
			this.resetChaseHistory.Clear();
			this.animator.Play("CrawlChase");
			this.localThrown = false;
			this.localCaught = false;
			if (this.tapped)
			{
				this.audioSource.clip = this.chaseLoop;
				this.audioSource.loop = true;
				this.audioSource.GTPlay();
				return;
			}
			break;
		case SecondLookSkeleton.GhostState.CaughtPlayer:
			this.currentState = SecondLookSkeleton.GhostState.CaughtPlayer;
			this.heightOffset.localPosition = Vector3.zero;
			if (this.tapped)
			{
				this.audioSource.GTPlayOneShot(this.grabbedSound, 1f);
				this.audioSource.loop = true;
				this.audioSource.clip = this.carryingLoop;
				this.audioSource.GTPlay();
				this.animator.Play("ArmsOut");
			}
			if (!this.IsMine())
			{
				this.SetNodes();
				return;
			}
			break;
		case SecondLookSkeleton.GhostState.PlayerThrown:
			this.currentState = SecondLookSkeleton.GhostState.PlayerThrown;
			this.timeThrown = Time.time;
			this.localThrown = false;
			break;
		case SecondLookSkeleton.GhostState.Reset:
			break;
		default:
			return;
		}
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x0001DB50 File Offset: 0x0001BD50
	private void ProcessGhostState()
	{
		if (this.IsMine())
		{
			switch (this.currentState)
			{
			case SecondLookSkeleton.GhostState.Unactivated:
				if (this.changeAngerPointOnTimeInterval && Time.time - this.angerPointChangedTime > this.changeAngerPointTimeMinutes * 60f)
				{
					this.synchValues.angerPoint = Random.Range(0, this.angerPoint.Length);
					this.angerPointIndex = this.synchValues.angerPoint;
					this.angerPointChangedTime = Time.time;
				}
				this.spookyGhost.transform.position = this.angerPoint[this.angerPointIndex].position;
				this.spookyGhost.transform.rotation = this.angerPoint[this.angerPointIndex].rotation;
				this.CheckActivateGhost();
				return;
			case SecondLookSkeleton.GhostState.Activated:
				if (Time.time > this.timeFirstAppeared + this.timeToFirstDisappear)
				{
					this.ChangeState(SecondLookSkeleton.GhostState.Patrolling);
					return;
				}
				break;
			case SecondLookSkeleton.GhostState.Patrolling:
				if (!this.CheckPlayerSeen() && this.playersSeen.Count == 0)
				{
					this.PatrolMove();
					return;
				}
				this.StartChasing();
				return;
			case SecondLookSkeleton.GhostState.Chasing:
				if (!this.CheckPlayerSeen() || !this.CanGrab())
				{
					this.ChaseMove();
					return;
				}
				this.GrabPlayer();
				return;
			case SecondLookSkeleton.GhostState.CaughtPlayer:
				this.CaughtPlayerUpdate();
				return;
			case SecondLookSkeleton.GhostState.PlayerThrown:
				if (Time.time > this.timeThrown + this.timeThrownCooldown)
				{
					this.ChangeState(SecondLookSkeleton.GhostState.Unactivated);
				}
				break;
			case SecondLookSkeleton.GhostState.Reset:
				break;
			default:
				return;
			}
			return;
		}
		this.SetTappedState();
		switch (this.currentState)
		{
		case SecondLookSkeleton.GhostState.Unactivated:
			this.SetNodes();
			this.spookyGhost.transform.position = this.angerPoint[this.angerPointIndex].position;
			this.spookyGhost.transform.rotation = this.angerPoint[this.angerPointIndex].rotation;
			this.CheckActivateGhost();
			return;
		case SecondLookSkeleton.GhostState.Activated:
			this.FollowPosition();
			return;
		case SecondLookSkeleton.GhostState.Patrolling:
			this.FollowPosition();
			this.CheckPlayerSeen();
			return;
		case SecondLookSkeleton.GhostState.Chasing:
			if (this.CheckPlayerSeen() && this.CanGrab())
			{
				this.GrabPlayer();
			}
			this.FollowPosition();
			return;
		case SecondLookSkeleton.GhostState.CaughtPlayer:
		case SecondLookSkeleton.GhostState.PlayerThrown:
			this.CaughtPlayerUpdate();
			break;
		case SecondLookSkeleton.GhostState.Reset:
			break;
		default:
			return;
		}
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x0001DD70 File Offset: 0x0001BF70
	private void CaughtPlayerUpdate()
	{
		if (this.localThrown)
		{
			return;
		}
		if (this.GhostAtExit())
		{
			if (this.localCaught)
			{
				this.ChuckPlayer();
			}
			if (this.IsMine())
			{
				this.DeactivateGhost();
			}
			return;
		}
		this.CaughtMove();
		if (this.localCaught)
		{
			this.FloatPlayer();
			return;
		}
		if (this.CheckPlayerSeen() && this.CanGrab())
		{
			this.localCaught = true;
		}
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x0001DDD8 File Offset: 0x0001BFD8
	private void SetTappedState()
	{
		if (!this.tapped)
		{
			return;
		}
		if (this.spookyText != null && !this.spookyText.activeSelf)
		{
			this.spookyText.SetActive(true);
		}
		if (this.spookyGhost.activeSelf && this.currentState != SecondLookSkeleton.GhostState.Unactivated)
		{
			return;
		}
		this.spookyGhost.SetActive(true);
		switch (this.currentState)
		{
		case SecondLookSkeleton.GhostState.Unactivated:
			this.spookyGhost.SetActive(false);
			return;
		case SecondLookSkeleton.GhostState.Activated:
			this.animator.Play("ArmsOut");
			return;
		case SecondLookSkeleton.GhostState.Patrolling:
			this.animator.Play("CrawlPatrol");
			this.audioSource.loop = true;
			this.audioSource.clip = this.patrolLoop;
			this.audioSource.GTPlay();
			return;
		case SecondLookSkeleton.GhostState.Chasing:
			this.audioSource.clip = this.chaseLoop;
			this.audioSource.loop = true;
			this.audioSource.GTPlay();
			this.animator.Play("CrawlChase");
			this.spookyGhost.SetActive(true);
			return;
		case SecondLookSkeleton.GhostState.CaughtPlayer:
			this.audioSource.GTPlayOneShot(this.grabbedSound, 1f);
			this.audioSource.loop = true;
			this.audioSource.clip = this.carryingLoop;
			this.audioSource.GTPlay();
			this.animator.Play("ArmsOut");
			break;
		case SecondLookSkeleton.GhostState.PlayerThrown:
			this.animator.Play("ArmsOut");
			return;
		case SecondLookSkeleton.GhostState.Reset:
			break;
		default:
			return;
		}
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x0001DF5C File Offset: 0x0001C15C
	private void FollowPosition()
	{
		this.spookyGhost.transform.position = Vector3.Lerp(this.spookyGhost.transform.position, this.synchValues.position, 0.66f);
		this.spookyGhost.transform.rotation = Quaternion.Lerp(this.spookyGhost.transform.rotation, this.synchValues.rotation, 0.66f);
		if (this.currentState == SecondLookSkeleton.GhostState.Patrolling || this.currentState == SecondLookSkeleton.GhostState.Chasing)
		{
			this.SetHeightOffset();
			return;
		}
		this.heightOffset.localPosition = Vector3.zero;
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x0001DFFC File Offset: 0x0001C1FC
	private void CheckActivateGhost()
	{
		if (!this.tapped || this.currentState != SecondLookSkeleton.GhostState.Unactivated || this.playerTransform == null)
		{
			return;
		}
		this.currentlyLooking = this.IsCurrentlyLooking();
		if (this.requireSecondLookToActivate)
		{
			if (!this.firstLookActivated && this.currentlyLooking)
			{
				this.firstLookActivated = this.currentlyLooking;
				return;
			}
			if (this.firstLookActivated && !this.currentlyLooking)
			{
				this.lookedAway = true;
				return;
			}
			if (this.firstLookActivated && this.lookedAway && this.currentlyLooking)
			{
				this.firstLookActivated = false;
				this.lookedAway = false;
				this.ActivateGhost();
				return;
			}
		}
		else if (this.currentlyLooking)
		{
			this.ActivateGhost();
		}
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x0001E0AC File Offset: 0x0001C2AC
	private bool CanSeePlayer()
	{
		return this.CanSeePlayerWithResults(out this.closest);
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x0001E0BC File Offset: 0x0001C2BC
	private bool CanSeePlayerWithResults(out RaycastHit closest)
	{
		Vector3 vector = this.playerTransform.position - this.lookSource.position;
		int num = Physics.RaycastNonAlloc(this.lookSource.position, vector.normalized, this.rHits, this.maxSeeDistance, this.mask, QueryTriggerInteraction.Ignore);
		closest = this.rHits[0];
		if (num == 0)
		{
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			if (closest.distance > this.rHits[i].distance)
			{
				closest = this.rHits[i];
			}
		}
		return (this.playerMask & (1 << closest.collider.gameObject.layer)) != 0;
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x0001E187 File Offset: 0x0001C387
	private void ActivateGhost()
	{
		if (this.IsMine())
		{
			this.ChangeState(SecondLookSkeleton.GhostState.Activated);
			return;
		}
		this.synchValues.SendRPC("RemoteActivateGhost", RpcTarget.MasterClient, Array.Empty<object>());
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x0001E1AF File Offset: 0x0001C3AF
	private void StartChasing()
	{
		if (!this.IsMine())
		{
			return;
		}
		this.ChangeState(SecondLookSkeleton.GhostState.Chasing);
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x0001E1C4 File Offset: 0x0001C3C4
	private bool CheckPlayerSeen()
	{
		if (!this.tapped)
		{
			return false;
		}
		if (this.playersSeen.Contains(NetworkSystem.Instance.LocalPlayer))
		{
			return true;
		}
		if (!this.CanSeePlayer())
		{
			return false;
		}
		if (NetworkSystem.Instance.InRoom)
		{
			this.synchValues.SendRPC("RemotePlayerSeen", RpcTarget.Others, Array.Empty<object>());
		}
		this.playersSeen.Add(NetworkSystem.Instance.LocalPlayer);
		return true;
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x0001E236 File Offset: 0x0001C436
	public void RemoteActivateGhost()
	{
		if (this.IsMine() && this.currentState == SecondLookSkeleton.GhostState.Unactivated)
		{
			this.ActivateGhost();
		}
	}

	// Token: 0x06000530 RID: 1328 RVA: 0x0001E24E File Offset: 0x0001C44E
	public void RemotePlayerSeen(NetPlayer player)
	{
		if (this.IsMine() && !this.playersSeen.Contains(player))
		{
			this.playersSeen.Add(player);
		}
	}

	// Token: 0x06000531 RID: 1329 RVA: 0x0001E274 File Offset: 0x0001C474
	public void RemotePlayerCaught(NetPlayer player)
	{
		if (this.IsMine() && this.currentState == SecondLookSkeleton.GhostState.Chasing)
		{
			RigContainer rigContainer;
			VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
			if (rigContainer != null && this.playersSeen.Contains(player))
			{
				this.ChangeState(SecondLookSkeleton.GhostState.CaughtPlayer);
			}
		}
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x0001E2C0 File Offset: 0x0001C4C0
	private bool IsCurrentlyLooking()
	{
		return Vector3.Dot(this.playerTransform.forward, -this.spookyGhost.transform.forward) > 0f && (this.spookyGhost.transform.position - this.playerTransform.position).magnitude < this.ghostActivationDistance && this.CanSeePlayer();
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x0001E331 File Offset: 0x0001C531
	private void PatrolMove()
	{
		this.GhostMove(this.nextNode.transform, this.patrolSpeed);
		this.SetHeightOffset();
		this.CheckReachedNextNode(false, false);
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x0001E358 File Offset: 0x0001C558
	private void CheckReachedNextNode(bool forChuck, bool forChase)
	{
		if ((this.nextNode.transform.position - this.spookyGhost.transform.position).magnitude < this.reachNodeDist)
		{
			if (this.nextNode.connectedNodes.Length == 1)
			{
				this.currentNode = this.nextNode;
				this.nextNode = this.nextNode.connectedNodes[0];
				this.SyncNodes();
				return;
			}
			if (forChuck)
			{
				float num = this.nextNode.distanceToExitNode;
				SkeletonPathingNode skeletonPathingNode = this.nextNode.connectedNodes[0];
				for (int i = 0; i < this.nextNode.connectedNodes.Length; i++)
				{
					if (this.nextNode.connectedNodes[i].distanceToExitNode <= num)
					{
						skeletonPathingNode = this.nextNode.connectedNodes[i];
						num = skeletonPathingNode.distanceToExitNode;
					}
				}
				this.currentNode = this.nextNode;
				this.nextNode = skeletonPathingNode;
				this.SyncNodes();
				return;
			}
			if (forChase)
			{
				float num2 = float.MaxValue;
				float num3 = num2;
				RigContainer rigContainer = GorillaTagger.Instance.offlineVRRig.rigContainer;
				RigContainer rigContainer2 = rigContainer;
				for (int j = 0; j < this.playersSeen.Count; j++)
				{
					VRRigCache.Instance.TryGetVrrig(this.playersSeen[j], out rigContainer);
					if (!(rigContainer == null))
					{
						num2 = (rigContainer.transform.position - this.nextNode.transform.position).sqrMagnitude;
						if (num2 < num3)
						{
							rigContainer2 = rigContainer;
							num3 = num2;
						}
					}
				}
				Vector3 vector = rigContainer2.transform.position - this.nextNode.transform.position;
				SkeletonPathingNode skeletonPathingNode2 = this.nextNode.connectedNodes[0];
				num3 = 0f;
				for (int k = 0; k < this.nextNode.connectedNodes.Length; k++)
				{
					Vector3 vector2 = this.nextNode.connectedNodes[k].transform.position - this.nextNode.transform.position;
					num2 = Mathf.Sign(Vector3.Dot(vector, vector2)) * Vector3.Project(vector, vector2).sqrMagnitude;
					if (num2 >= num3)
					{
						skeletonPathingNode2 = this.nextNode.connectedNodes[k];
						num3 = num2;
					}
				}
				this.currentNode = this.nextNode;
				this.nextNode = skeletonPathingNode2;
				this.SyncNodes();
				this.resetChaseHistory.Add(this.nextNode);
				if (this.resetChaseHistory.Count > 8)
				{
					this.resetChaseHistory.RemoveAt(0);
				}
				if (this.resetChaseHistory.Count >= 8 && this.resetChaseHistory[0] == this.resetChaseHistory[2] == this.resetChaseHistory[4] == this.resetChaseHistory[6] && this.resetChaseHistory[1] == this.resetChaseHistory[3] == this.resetChaseHistory[5] == this.resetChaseHistory[7])
				{
					this.resetChaseHistory.Clear();
					this.ChangeState(SecondLookSkeleton.GhostState.Patrolling);
				}
				return;
			}
			SkeletonPathingNode skeletonPathingNode3 = this.nextNode.connectedNodes[Random.Range(0, this.nextNode.connectedNodes.Length)];
			for (int l = 0; l < 10; l++)
			{
				skeletonPathingNode3 = this.nextNode.connectedNodes[Random.Range(0, this.nextNode.connectedNodes.Length)];
				if (!skeletonPathingNode3.ejectionPoint && skeletonPathingNode3 != this.currentNode)
				{
					break;
				}
			}
			this.currentNode = this.nextNode;
			this.nextNode = skeletonPathingNode3;
			this.SyncNodes();
		}
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x0001E722 File Offset: 0x0001C922
	private void ChaseMove()
	{
		this.GhostMove(this.nextNode.transform, this.chaseSpeed);
		this.SetHeightOffset();
		this.CheckReachedNextNode(false, true);
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x0001E749 File Offset: 0x0001C949
	private void CaughtMove()
	{
		this.GhostMove(this.nextNode.transform, this.caughtSpeed);
		this.CheckReachedNextNode(true, false);
		this.SyncNodes();
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x0001E770 File Offset: 0x0001C970
	private void SyncNodes()
	{
		this.synchValues.currentNode = this.pathPoints.IndexOfRef(this.currentNode);
		this.synchValues.nextNode = this.pathPoints.IndexOfRef(this.nextNode);
		this.synchValues.angerPoint = this.angerPointIndex;
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x0001E7C8 File Offset: 0x0001C9C8
	public void SetNodes()
	{
		if (this.synchValues.currentNode > this.pathPoints.Length || this.synchValues.currentNode < 0)
		{
			return;
		}
		this.currentNode = this.pathPoints[this.synchValues.currentNode];
		this.nextNode = this.pathPoints[this.synchValues.nextNode];
		this.angerPointIndex = this.synchValues.angerPoint;
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x0001E83C File Offset: 0x0001CA3C
	private bool GhostAtExit()
	{
		return this.currentNode.distanceToExitNode == 0f && (this.spookyGhost.transform.position - this.currentNode.transform.position).magnitude < this.reachNodeDist;
	}

	// Token: 0x0600053A RID: 1338 RVA: 0x0001E894 File Offset: 0x0001CA94
	private void GhostMove(Transform target, float speed)
	{
		this.spookyGhost.transform.rotation = Quaternion.RotateTowards(this.spookyGhost.transform.rotation, Quaternion.LookRotation(target.position - this.spookyGhost.transform.position, Vector3.up), this.maxRotSpeed * Time.deltaTime);
		this.spookyGhost.transform.position += (target.position - this.spookyGhost.transform.position).normalized * speed * Time.deltaTime;
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x0001E945 File Offset: 0x0001CB45
	private void DeactivateGhost()
	{
		this.ChangeState(SecondLookSkeleton.GhostState.PlayerThrown);
	}

	// Token: 0x0600053C RID: 1340 RVA: 0x0001E950 File Offset: 0x0001CB50
	private bool CanGrab()
	{
		return (this.spookyGhost.transform.position - this.playerTransform.position).magnitude < this.catchDistance;
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x0001E98D File Offset: 0x0001CB8D
	private void GrabPlayer()
	{
		if (this.IsMine())
		{
			if (this.currentState == SecondLookSkeleton.GhostState.Chasing)
			{
				this.ChangeState(SecondLookSkeleton.GhostState.CaughtPlayer);
			}
			this.localCaught = true;
		}
		this.synchValues.SendRPC("RemotePlayerCaught", RpcTarget.MasterClient, Array.Empty<object>());
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x0001E9C4 File Offset: 0x0001CBC4
	private void FloatPlayer()
	{
		RaycastHit raycastHit;
		if (this.CanSeePlayerWithResults(out raycastHit))
		{
			GorillaTagger.Instance.rigidbody.MovePosition(Vector3.MoveTowards(GorillaTagger.Instance.rigidbody.position, this.spookyGhost.transform.position + this.spookyGhost.transform.rotation * this.offsetGrabPosition, this.caughtSpeed * 10f * Time.deltaTime));
		}
		else
		{
			Vector3 vector = raycastHit.point - this.playerTransform.position;
			vector += GTPlayer.Instance.headCollider.radius * 1.05f * vector.normalized;
			GorillaTagger.Instance.transform.parent.position += vector;
			GTPlayer.Instance.InitializeValues();
		}
		GorillaTagger.Instance.rigidbody.velocity = Vector3.zero;
		EquipmentInteractor.instance.ForceStopClimbing();
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, 0.25f);
		GorillaTagger.Instance.StartVibration(true, this.hapticStrength / 4f, Time.deltaTime);
		GorillaTagger.Instance.StartVibration(false, this.hapticStrength / 4f, Time.deltaTime);
	}

	// Token: 0x0600053F RID: 1343 RVA: 0x0001EB14 File Offset: 0x0001CD14
	private void ChuckPlayer()
	{
		this.localCaught = false;
		this.localThrown = true;
		Vector3 vector = this.currentNode.transform.position - this.currentNode.connectedNodes[0].transform.position;
		GorillaTagger instance = GorillaTagger.Instance;
		Rigidbody rigidbody = ((instance != null) ? instance.rigidbody : null);
		GTAudioSourceExtensions.GTPlayClipAtPoint(this.throwSound, this.audioSource.transform.position, 0.25f);
		this.audioSource.GTStop();
		this.audioSource.loop = false;
		if (rigidbody != null)
		{
			rigidbody.velocity = vector.normalized * this.throwForce;
		}
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x0001EBC8 File Offset: 0x0001CDC8
	private void SetHeightOffset()
	{
		int num = Physics.RaycastNonAlloc(this.spookyGhost.transform.position + Vector3.up * this.bodyHeightOffset, Vector3.down, this.rHits, this.maxSeeDistance, this.mask, QueryTriggerInteraction.Ignore);
		if (num == 0)
		{
			this.heightOffset.localPosition = Vector3.zero;
			return;
		}
		RaycastHit raycastHit = this.rHits[0];
		for (int i = 0; i < num; i++)
		{
			if (raycastHit.distance < this.rHits[i].distance)
			{
				raycastHit = this.rHits[i];
			}
		}
		this.heightOffset.localPosition = new Vector3(0f, -raycastHit.distance, 0f);
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x0001EC93 File Offset: 0x0001CE93
	private bool IsMine()
	{
		return !NetworkSystem.Instance.InRoom || this.synchValues.IsMine;
	}

	// Token: 0x040005F6 RID: 1526
	public Transform[] angerPoint;

	// Token: 0x040005F7 RID: 1527
	public int angerPointIndex;

	// Token: 0x040005F8 RID: 1528
	public SkeletonPathingNode[] pathPoints;

	// Token: 0x040005F9 RID: 1529
	public SkeletonPathingNode[] exitPoints;

	// Token: 0x040005FA RID: 1530
	public Transform heightOffset;

	// Token: 0x040005FB RID: 1531
	public bool requireSecondLookToActivate;

	// Token: 0x040005FC RID: 1532
	public bool requireTappingToActivate;

	// Token: 0x040005FD RID: 1533
	public bool changeAngerPointOnTimeInterval;

	// Token: 0x040005FE RID: 1534
	public float changeAngerPointTimeMinutes = 3f;

	// Token: 0x040005FF RID: 1535
	private bool firstLookActivated;

	// Token: 0x04000600 RID: 1536
	private bool lookedAway;

	// Token: 0x04000601 RID: 1537
	private bool currentlyLooking;

	// Token: 0x04000602 RID: 1538
	public float ghostActivationDistance;

	// Token: 0x04000603 RID: 1539
	public GameObject spookyGhost;

	// Token: 0x04000604 RID: 1540
	public float timeFirstAppeared;

	// Token: 0x04000605 RID: 1541
	public float timeToFirstDisappear;

	// Token: 0x04000606 RID: 1542
	public SecondLookSkeleton.GhostState currentState;

	// Token: 0x04000607 RID: 1543
	public GameObject spookyText;

	// Token: 0x04000608 RID: 1544
	public float patrolSpeed;

	// Token: 0x04000609 RID: 1545
	public float chaseSpeed;

	// Token: 0x0400060A RID: 1546
	public float caughtSpeed;

	// Token: 0x0400060B RID: 1547
	public SkeletonPathingNode firstNode;

	// Token: 0x0400060C RID: 1548
	public SkeletonPathingNode currentNode;

	// Token: 0x0400060D RID: 1549
	public SkeletonPathingNode nextNode;

	// Token: 0x0400060E RID: 1550
	public Transform lookSource;

	// Token: 0x0400060F RID: 1551
	private Transform playerTransform;

	// Token: 0x04000610 RID: 1552
	public float reachNodeDist;

	// Token: 0x04000611 RID: 1553
	public float maxRotSpeed;

	// Token: 0x04000612 RID: 1554
	public float hapticStrength;

	// Token: 0x04000613 RID: 1555
	public float hapticDuration;

	// Token: 0x04000614 RID: 1556
	public Vector3 offsetGrabPosition;

	// Token: 0x04000615 RID: 1557
	public float throwForce;

	// Token: 0x04000616 RID: 1558
	public Animator animator;

	// Token: 0x04000617 RID: 1559
	public float bodyHeightOffset;

	// Token: 0x04000618 RID: 1560
	private float timeThrown;

	// Token: 0x04000619 RID: 1561
	public float timeThrownCooldown = 1f;

	// Token: 0x0400061A RID: 1562
	public float catchDistance;

	// Token: 0x0400061B RID: 1563
	public float maxSeeDistance;

	// Token: 0x0400061C RID: 1564
	private RaycastHit[] rHits;

	// Token: 0x0400061D RID: 1565
	public LayerMask mask;

	// Token: 0x0400061E RID: 1566
	public LayerMask playerMask;

	// Token: 0x0400061F RID: 1567
	public AudioSource audioSource;

	// Token: 0x04000620 RID: 1568
	public AudioClip initialScream;

	// Token: 0x04000621 RID: 1569
	public AudioClip patrolLoop;

	// Token: 0x04000622 RID: 1570
	public AudioClip chaseLoop;

	// Token: 0x04000623 RID: 1571
	public AudioClip grabbedSound;

	// Token: 0x04000624 RID: 1572
	public AudioClip carryingLoop;

	// Token: 0x04000625 RID: 1573
	public AudioClip throwSound;

	// Token: 0x04000626 RID: 1574
	public List<SkeletonPathingNode> resetChaseHistory = new List<SkeletonPathingNode>();

	// Token: 0x04000627 RID: 1575
	private SecondLookSkeletonSynchValues synchValues;

	// Token: 0x04000628 RID: 1576
	private bool localCaught;

	// Token: 0x04000629 RID: 1577
	private bool localThrown;

	// Token: 0x0400062A RID: 1578
	public List<NetPlayer> playersSeen;

	// Token: 0x0400062B RID: 1579
	public bool tapped;

	// Token: 0x0400062C RID: 1580
	private RaycastHit closest;

	// Token: 0x0400062D RID: 1581
	private float angerPointChangedTime;

	// Token: 0x020000D3 RID: 211
	public enum GhostState
	{
		// Token: 0x0400062F RID: 1583
		Unactivated,
		// Token: 0x04000630 RID: 1584
		Activated,
		// Token: 0x04000631 RID: 1585
		Patrolling,
		// Token: 0x04000632 RID: 1586
		Chasing,
		// Token: 0x04000633 RID: 1587
		CaughtPlayer,
		// Token: 0x04000634 RID: 1588
		PlayerThrown,
		// Token: 0x04000635 RID: 1589
		Reset
	}
}
