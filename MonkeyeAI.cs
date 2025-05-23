using System;
using System.Collections.Generic;
using GorillaLocomotion;
using JetBrains.Annotations;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000BA RID: 186
[RequireComponent(typeof(NetworkView))]
public class MonkeyeAI : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06000485 RID: 1157 RVA: 0x00019F18 File Offset: 0x00018118
	private string UserIdFromRig(VRRig rig)
	{
		if (rig == null)
		{
			return "";
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			if (rig == GorillaTagger.Instance.offlineVRRig)
			{
				return "-1";
			}
			Debug.Log("Not in a room but not targeting offline rig");
			return null;
		}
		else
		{
			if (rig == GorillaTagger.Instance.offlineVRRig)
			{
				return NetworkSystem.Instance.LocalPlayer.UserId;
			}
			if (rig.creator == null)
			{
				return "";
			}
			return rig.creator.UserId;
		}
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x00019FA0 File Offset: 0x000181A0
	private VRRig GetRig(string userId)
	{
		if (userId == "")
		{
			return null;
		}
		if (NetworkSystem.Instance.InRoom || !(userId != "-1"))
		{
			foreach (VRRig vrrig in this.GetValidChoosableRigs())
			{
				if (!(vrrig == null))
				{
					NetPlayer creator = vrrig.creator;
					if (creator != null && userId == creator.UserId)
					{
						return vrrig;
					}
				}
			}
			return null;
		}
		if (userId == "-1 " && GorillaTagger.Instance != null)
		{
			return GorillaTagger.Instance.offlineVRRig;
		}
		return null;
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x0001A064 File Offset: 0x00018264
	private float Distance2D(Vector3 a, Vector3 b)
	{
		Vector2 vector = new Vector2(a.x, a.z);
		Vector2 vector2 = new Vector2(b.x, b.z);
		return Vector2.Distance(vector, vector2);
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x0001A09C File Offset: 0x0001829C
	private Transform PickRandomPatrolPoint()
	{
		int num;
		do
		{
			num = Random.Range(0, this.patrolPts.Count);
		}
		while (num == this.patrolIdx);
		this.patrolIdx = num;
		return this.patrolPts[num];
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x0001A0DC File Offset: 0x000182DC
	private void PickNewPath(bool pathFinished = false)
	{
		if (this.calculatingPath)
		{
			return;
		}
		this.currentWaypoint = 0;
		switch (this.replState.state)
		{
		case MonkeyeAI_ReplState.EStates.Patrolling:
			if (this.patrolCount == this.maxPatrols)
			{
				this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
				this.targetPosition = this.PickRandomPatrolPoint().position;
				this.patrolCount = 0;
			}
			else
			{
				this.targetPosition = this.PickRandomPatrolPoint().position;
				this.patrolCount++;
			}
			break;
		case MonkeyeAI_ReplState.EStates.Chasing:
			if (!this.lockedOn)
			{
				Vector3 position = base.transform.position;
				VRRig vrrig;
				if (this.ClosestPlayer(in position, out vrrig) && vrrig != this.targetRig)
				{
					this.SetTargetPlayer(vrrig);
				}
			}
			if (this.targetRig == null)
			{
				this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
				this.targetPosition = this.sleepPt.position;
			}
			else
			{
				this.targetPosition = this.targetRig.transform.position;
			}
			break;
		case MonkeyeAI_ReplState.EStates.ReturnToSleepPt:
			this.targetPosition = this.sleepPt.position;
			break;
		}
		this.calculatingPath = true;
		this.seeker.StartPath(base.transform.position, this.targetPosition, new OnPathDelegate(this.OnPathComplete));
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x0001A22C File Offset: 0x0001842C
	private void Awake()
	{
		this.lazerFx = base.GetComponent<Monkeye_LazerFX>();
		this.animController = base.GetComponent<Animator>();
		this.layerBase = this.animController.GetLayerIndex("Base_Layer");
		this.layerForward = this.animController.GetLayerIndex("MoveFwdAddPose");
		this.layerLeft = this.animController.GetLayerIndex("TurnLAddPose");
		this.layerRight = this.animController.GetLayerIndex("TurnRAddPose");
		this.seeker = base.GetComponent<Seeker>();
		this.renderer = this.portalFx.GetComponent<Renderer>();
		this.portalMatPropBlock = new MaterialPropertyBlock();
		this.monkEyeMatPropBlock = new MaterialPropertyBlock();
		this.layerMask = UnityLayer.Default.ToLayerMask() | UnityLayer.GorillaObject.ToLayerMask();
		this.SetDefaultAttackState();
		this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		this.replStateRequestableOwnershipGaurd = this.replState.GetComponent<RequestableOwnershipGuard>();
		this.myRequestableOwnershipGaurd = base.GetComponent<RequestableOwnershipGuard>();
		if (this.monkEyeColor.a != 0f || this.monkEyeEyeColorNormal.a != 0f)
		{
			if (this.monkEyeColor.a != 0f)
			{
				this.monkEyeMatPropBlock.SetVector(MonkeyeAI.ColorShaderProp, this.monkEyeColor);
			}
			if (this.monkEyeEyeColorNormal.a != 0f)
			{
				this.monkEyeMatPropBlock.SetVector(MonkeyeAI.EyeColorShaderProp, this.monkEyeEyeColorNormal);
			}
			this.skinnedMeshRenderer.SetPropertyBlock(this.monkEyeMatPropBlock);
		}
		base.InvokeRepeating("AntiOverlapAssurance", 0.2f, 0.5f);
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x0001A3C3 File Offset: 0x000185C3
	private void Start()
	{
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x0001A3D8 File Offset: 0x000185D8
	private void OnPathComplete(Path path_)
	{
		this.path = path_;
		this.currentWaypoint = 0;
		if (this.path.vectorPath.Count < 1)
		{
			base.transform.position = this.sleepPt.position;
			base.transform.rotation = this.sleepPt.rotation;
			this.path = null;
		}
		this.calculatingPath = false;
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x0001A440 File Offset: 0x00018640
	private void FollowPath()
	{
		if (this.path == null || this.currentWaypoint >= this.path.vectorPath.Count || this.currentWaypoint < 0)
		{
			this.PickNewPath(false);
			if (this.path == null)
			{
				return;
			}
		}
		if (this.Distance2D(base.transform.position, this.path.vectorPath[this.currentWaypoint]) < 0.01f)
		{
			if (this.currentWaypoint + 1 == this.path.vectorPath.Count)
			{
				this.PickNewPath(true);
				return;
			}
			this.currentWaypoint++;
		}
		Vector3 normalized = (this.path.vectorPath[this.currentWaypoint] - base.transform.position).normalized;
		normalized.y = 0f;
		if (this.animController.GetCurrentAnimatorStateInfo(0).IsName("Move"))
		{
			Vector3 vector = normalized * this.speed;
			base.transform.position += vector * this.deltaTime;
		}
		Mathf.Clamp01(Vector3.Dot(base.transform.forward, normalized) / 1.5707964f);
		if (Mathf.Sign(Vector3.Cross(base.transform.forward, normalized).y) > 0f)
		{
			this.animController.SetLayerWeight(this.layerRight, 0f);
		}
		else
		{
			this.animController.SetLayerWeight(this.layerLeft, 0f);
		}
		this.animController.SetLayerWeight(this.layerForward, 0f);
		Vector3 vector2 = Vector3.RotateTowards(base.transform.forward, normalized, this.rotationSpeed * this.deltaTime, 0f);
		base.transform.rotation = Quaternion.LookRotation(vector2);
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x0001A624 File Offset: 0x00018824
	private bool PlayerNear(VRRig rig, float dist, out float playerDist)
	{
		if (rig == null)
		{
			playerDist = float.PositiveInfinity;
			return false;
		}
		playerDist = this.Distance2D(rig.transform.position, base.transform.position);
		return playerDist < dist && Physics.RaycastNonAlloc(new Ray(base.transform.position, rig.transform.position - base.transform.position), this.rayResults, playerDist, this.layerMask) <= 0;
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x0001A6B4 File Offset: 0x000188B4
	private void Sleeping()
	{
		this.audioSource.volume = Mathf.Min(this.sleepLoopVolume, this.audioSource.volume + this.deltaTime / this.sleepDuration);
		if (this.audioSource.volume == this.sleepLoopVolume)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
			this.PickNewPath(false);
		}
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x0001A714 File Offset: 0x00018914
	private bool ClosestPlayer(in Vector3 myPos, out VRRig outRig)
	{
		float num = float.MaxValue;
		outRig = null;
		foreach (VRRig vrrig in this.GetValidChoosableRigs())
		{
			float num2 = 0f;
			if (this.PlayerNear(vrrig, this.chaseDistance, out num2) && num2 < num)
			{
				num = num2;
				outRig = vrrig;
			}
		}
		return num != float.MaxValue;
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x0001A794 File Offset: 0x00018994
	private bool CheckForChase()
	{
		foreach (VRRig vrrig in this.GetValidChoosableRigs())
		{
			float num = 0f;
			if (this.PlayerNear(vrrig, this.wakeDistance, out num))
			{
				this.SetTargetPlayer(vrrig);
				this.SetState(MonkeyeAI_ReplState.EStates.Chasing);
				this.PickNewPath(false);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x0001A814 File Offset: 0x00018A14
	public void SetChasePlayer(VRRig rig)
	{
		if (!this.GetValidChoosableRigs().Contains(rig))
		{
			return;
		}
		this.SetTargetPlayer(rig);
		this.lockedOn = true;
		this.SetState(MonkeyeAI_ReplState.EStates.Chasing);
		this.PickNewPath(false);
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x0001A841 File Offset: 0x00018A41
	public void SetSleep()
	{
		if (this.replState.state == MonkeyeAI_ReplState.EStates.Patrolling || this.replState.state == MonkeyeAI_ReplState.EStates.Chasing)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		}
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x0001A868 File Offset: 0x00018A68
	private void Patrolling()
	{
		this.audioSource.volume = Mathf.Min(this.patrolLoopVolume, this.audioSource.volume + this.deltaTime / this.patrolLoopFadeInTime);
		if (this.path == null)
		{
			this.PickNewPath(false);
		}
		if (this.audioSource.volume == this.patrolLoopVolume)
		{
			this.CheckForChase();
		}
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x0001A8D0 File Offset: 0x00018AD0
	private void Chasing()
	{
		this.audioSource.volume = Mathf.Min(this.chaseLoopVolume, this.audioSource.volume + this.deltaTime / this.chaseLoopFadeInTime);
		this.PickNewPath(false);
		if (this.targetRig == null)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
			return;
		}
		if (this.Distance2D(base.transform.position, this.targetRig.transform.position) < this.attackDistance)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.BeginAttack);
			return;
		}
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x0001A95C File Offset: 0x00018B5C
	private void ReturnToSleepPt()
	{
		if (this.path == null)
		{
			this.PickNewPath(false);
		}
		if (this.CheckForChase())
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Chasing);
			return;
		}
		if (this.Distance2D(base.transform.position, this.sleepPt.position) < 0.01f)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		}
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x0001A9B4 File Offset: 0x00018BB4
	private void UpdateClientState()
	{
		if (this.wasConnectedToRoom && !NetworkSystem.Instance.InRoom)
		{
			this.SetDefaultState();
			return;
		}
		if (ColliderEnabledManager.instance != null && !this.replState.floorEnabled)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				if (this.replState.userId == "-1")
				{
					ColliderEnabledManager.instance.DisableFloorForFrame();
				}
			}
			else if (this.replState.userId == NetworkSystem.Instance.LocalPlayer.UserId)
			{
				ColliderEnabledManager.instance.DisableFloorForFrame();
			}
		}
		if (this.portalFx.activeSelf != this.replState.portalEnabled)
		{
			this.portalFx.SetActive(this.replState.portalEnabled);
		}
		this.portalFx.transform.position = new Vector3(this.replState.attackPos.x, this.portalFx.transform.position.y, this.replState.attackPos.z);
		this.replState.timer -= this.deltaTime;
		if (this.replState.timer < 0f)
		{
			this.replState.timer = 0f;
		}
		VRRig rig = this.GetRig(this.replState.userId);
		if (this.replState.state >= MonkeyeAI_ReplState.EStates.BeginAttack)
		{
			if (rig == null)
			{
				this.lazerFx.DisableLazer();
			}
			else if (this.replState.state < MonkeyeAI_ReplState.EStates.DropPlayer)
			{
				this.lazerFx.EnableLazer(this.eyeBones, rig);
			}
			else
			{
				this.lazerFx.DisableLazer();
			}
		}
		else
		{
			this.lazerFx.DisableLazer();
		}
		if (this.replState.portalEnabled)
		{
			this.portalColor.a = this.replState.alpha;
			this.portalMatPropBlock.SetVector(MonkeyeAI.tintColorShaderProp, this.portalColor);
			this.renderer.SetPropertyBlock(this.portalMatPropBlock);
		}
		if (GorillaTagger.Instance.offlineVRRig == rig && this.replState.freezePlayer)
		{
			GTPlayer.Instance.SetMaximumSlipThisFrame();
			Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
			Vector3 vector = rigidbody.velocity;
			rigidbody.velocity = new Vector3(vector.x * this.deltaTime * 4f, Mathf.Min(vector.y, 0f), vector.x * this.deltaTime * 4f);
		}
		if (!this.replState.IsMine)
		{
			this.SetClientState(this.replState.state);
		}
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x0001AC59 File Offset: 0x00018E59
	private void SetDefaultState()
	{
		this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		this.SetDefaultAttackState();
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x0001AC68 File Offset: 0x00018E68
	private void SetDefaultAttackState()
	{
		this.replState.floorEnabled = true;
		this.replState.timer = 0f;
		this.replState.userId = "";
		this.replState.attackPos = base.transform.position;
		this.replState.portalEnabled = false;
		this.replState.freezePlayer = false;
		this.replState.alpha = 0f;
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x0001ACDF File Offset: 0x00018EDF
	private void ExitAttackState()
	{
		this.SetDefaultAttackState();
		this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x0001ACF0 File Offset: 0x00018EF0
	private void BeginAttack()
	{
		this.path = null;
		this.replState.freezePlayer = true;
		if (this.replState.timer <= 0f)
		{
			if (this.audioSource.enabled)
			{
				this.audioSource.GTPlayOneShot(this.attackSound, this.attackVolume);
			}
			this.replState.timer = this.openFloorTime;
			this.replState.portalEnabled = true;
			this.SetState(MonkeyeAI_ReplState.EStates.OpenFloor);
		}
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x0001AD6C File Offset: 0x00018F6C
	private void OpenFloor()
	{
		this.replState.alpha = Mathf.Lerp(0f, 1f, 1f - Mathf.Clamp01(this.replState.timer / this.openFloorTime));
		if (this.replState.timer <= 0f)
		{
			this.replState.timer = this.dropPlayerTime;
			this.replState.floorEnabled = false;
			this.SetState(MonkeyeAI_ReplState.EStates.DropPlayer);
		}
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x0001ADE6 File Offset: 0x00018FE6
	private void DropPlayer()
	{
		if (this.replState.timer <= 0f)
		{
			this.replState.timer = this.dropPlayerTime;
			this.replState.floorEnabled = true;
			this.SetState(MonkeyeAI_ReplState.EStates.CloseFloor);
		}
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x0001AE1E File Offset: 0x0001901E
	private void CloseFloor()
	{
		if (this.replState.timer <= 0f)
		{
			this.ExitAttackState();
		}
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x0001AE38 File Offset: 0x00019038
	private void ValidateChasingRig()
	{
		if (this.targetRig == null)
		{
			this.SetTargetPlayer(null);
			return;
		}
		bool flag = false;
		foreach (VRRig vrrig in this.GetValidChoosableRigs())
		{
			if (vrrig == this.targetRig)
			{
				flag = true;
				this.SetTargetPlayer(vrrig);
				break;
			}
		}
		if (!flag)
		{
			this.SetTargetPlayer(null);
		}
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x0001AEC0 File Offset: 0x000190C0
	public void SetState(MonkeyeAI_ReplState.EStates state_)
	{
		if (this.replState.IsMine)
		{
			this.replState.state = state_;
		}
		this.animController.SetInteger(MonkeyeAI.animStateID, (int)this.replState.state);
		switch (this.replState.state)
		{
		case MonkeyeAI_ReplState.EStates.Sleeping:
			this.setEyeColor(this.monkEyeEyeColorNormal);
			this.lockedOn = false;
			this.audioSource.clip = this.sleepLoopSound;
			this.audioSource.volume = 0f;
			if (this.audioSource.enabled)
			{
				this.audioSource.GTPlay();
				return;
			}
			break;
		case MonkeyeAI_ReplState.EStates.Patrolling:
			this.setEyeColor(this.monkEyeEyeColorNormal);
			this.lockedOn = false;
			this.audioSource.clip = this.patrolLoopSound;
			this.audioSource.loop = true;
			this.audioSource.volume = 0f;
			if (this.audioSource.enabled)
			{
				this.audioSource.GTPlay();
			}
			this.patrolCount = 0;
			return;
		case MonkeyeAI_ReplState.EStates.Chasing:
			this.setEyeColor(this.monkEyeEyeColorNormal);
			this.audioSource.loop = true;
			this.audioSource.volume = 0f;
			this.audioSource.clip = this.chaseLoopSound;
			if (this.audioSource.enabled)
			{
				this.audioSource.GTPlay();
				return;
			}
			break;
		case MonkeyeAI_ReplState.EStates.ReturnToSleepPt:
		case MonkeyeAI_ReplState.EStates.GoToSleep:
			break;
		case MonkeyeAI_ReplState.EStates.BeginAttack:
			this.setEyeColor(this.monkEyeEyeColorAttacking);
			if (this.replState.IsMine)
			{
				this.replState.attackPos = ((this.targetRig != null) ? this.targetRig.transform.position : base.transform.position);
				this.replState.timer = this.beginAttackTime;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x0001B090 File Offset: 0x00019290
	public void SetClientState(MonkeyeAI_ReplState.EStates state_)
	{
		this.animController.SetInteger(MonkeyeAI.animStateID, (int)this.replState.state);
		if (this.previousState != this.replState.state)
		{
			this.previousState = this.replState.state;
			switch (this.replState.state)
			{
			case MonkeyeAI_ReplState.EStates.Sleeping:
				this.setEyeColor(this.monkEyeEyeColorNormal);
				this.lockedOn = false;
				this.audioSource.clip = this.sleepLoopSound;
				this.audioSource.volume = Mathf.Min(this.sleepLoopVolume, this.audioSource.volume + this.deltaTime / this.sleepDuration);
				if (this.audioSource.enabled)
				{
					this.audioSource.GTPlay();
				}
				break;
			case MonkeyeAI_ReplState.EStates.Patrolling:
				this.setEyeColor(this.monkEyeEyeColorNormal);
				this.lockedOn = false;
				this.audioSource.clip = this.patrolLoopSound;
				this.audioSource.loop = true;
				this.audioSource.volume = Mathf.Min(this.patrolLoopVolume, this.audioSource.volume + this.deltaTime / this.patrolLoopFadeInTime);
				if (this.audioSource.enabled)
				{
					this.audioSource.GTPlay();
				}
				this.patrolCount = 0;
				break;
			case MonkeyeAI_ReplState.EStates.Chasing:
				this.setEyeColor(this.monkEyeEyeColorNormal);
				this.audioSource.loop = true;
				this.audioSource.volume = Mathf.Min(this.chaseLoopVolume, this.audioSource.volume + this.deltaTime / this.chaseLoopFadeInTime);
				this.audioSource.clip = this.chaseLoopSound;
				if (this.audioSource.enabled)
				{
					this.audioSource.GTPlay();
				}
				break;
			case MonkeyeAI_ReplState.EStates.BeginAttack:
				this.setEyeColor(this.monkEyeEyeColorAttacking);
				break;
			}
		}
		switch (this.replState.state)
		{
		case MonkeyeAI_ReplState.EStates.Sleeping:
			this.audioSource.volume = Mathf.Min(this.sleepLoopVolume, this.audioSource.volume + this.deltaTime / this.sleepDuration);
			return;
		case MonkeyeAI_ReplState.EStates.Patrolling:
			this.audioSource.volume = Mathf.Min(this.patrolLoopVolume, this.audioSource.volume + this.deltaTime / this.patrolLoopFadeInTime);
			return;
		case MonkeyeAI_ReplState.EStates.Chasing:
			this.audioSource.volume = Mathf.Min(this.chaseLoopVolume, this.audioSource.volume + this.deltaTime / this.chaseLoopFadeInTime);
			return;
		default:
			return;
		}
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x0001B32D File Offset: 0x0001952D
	private void setEyeColor(Color c)
	{
		if (c.a != 0f)
		{
			this.monkEyeMatPropBlock.SetVector(MonkeyeAI.EyeColorShaderProp, c);
			this.skinnedMeshRenderer.SetPropertyBlock(this.monkEyeMatPropBlock);
		}
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x0001B364 File Offset: 0x00019564
	public List<VRRig> GetValidChoosableRigs()
	{
		this.validRigs.Clear();
		foreach (VRRig vrrig in this.playerCollection.containedRigs)
		{
			if ((NetworkSystem.Instance.InRoom || vrrig.isOfflineVRRig) && !(vrrig == null))
			{
				this.validRigs.Add(vrrig);
			}
		}
		return this.validRigs;
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x0001B3F0 File Offset: 0x000195F0
	public void SliceUpdate()
	{
		this.wasConnectedToRoom = NetworkSystem.Instance.InRoom;
		this.deltaTime = Time.time - this.lastTime;
		this.lastTime = Time.time;
		this.UpdateClientState();
		if (NetworkSystem.Instance.InRoom && !this.replState.IsMine)
		{
			this.path = null;
			return;
		}
		if (!this.playerCollection.gameObject.activeInHierarchy)
		{
			NetPlayer netPlayer = null;
			float num = float.PositiveInfinity;
			foreach (VRRig vrrig in this.playersInRoomCollection.containedRigs)
			{
				if (!(vrrig == null))
				{
					float num2 = Vector3.Distance(base.transform.position, vrrig.transform.position);
					if (num2 < num)
					{
						netPlayer = vrrig.creator;
						num = num2;
					}
				}
			}
			if (num > 6f)
			{
				return;
			}
			this.path = null;
			if (netPlayer == null)
			{
				return;
			}
			this.replStateRequestableOwnershipGaurd.TransferOwnership(netPlayer, "");
			this.myRequestableOwnershipGaurd.TransferOwnership(netPlayer, "");
			return;
		}
		else
		{
			this.ValidateChasingRig();
			switch (this.replState.state)
			{
			case MonkeyeAI_ReplState.EStates.Sleeping:
				this.Sleeping();
				break;
			case MonkeyeAI_ReplState.EStates.Patrolling:
				this.Patrolling();
				break;
			case MonkeyeAI_ReplState.EStates.Chasing:
				this.Chasing();
				break;
			case MonkeyeAI_ReplState.EStates.ReturnToSleepPt:
				this.ReturnToSleepPt();
				break;
			case MonkeyeAI_ReplState.EStates.BeginAttack:
				this.BeginAttack();
				break;
			case MonkeyeAI_ReplState.EStates.OpenFloor:
				this.OpenFloor();
				break;
			case MonkeyeAI_ReplState.EStates.DropPlayer:
				this.DropPlayer();
				break;
			case MonkeyeAI_ReplState.EStates.CloseFloor:
				this.CloseFloor();
				break;
			}
			if (this.path == null)
			{
				return;
			}
			this.FollowPath();
			this.velocity = base.transform.position - this.prevPosition;
			this.prevPosition = base.transform.position;
			return;
		}
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x00017251 File Offset: 0x00015451
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x0001725A File Offset: 0x0001545A
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x0001B5DC File Offset: 0x000197DC
	private void AntiOverlapAssurance()
	{
		try
		{
			if ((!NetworkSystem.Instance.InRoom || this.replState.IsMine) && this.playerCollection.gameObject.activeInHierarchy)
			{
				foreach (MonkeyeAI monkeyeAI in this.playerCollection.monkeyeAis)
				{
					if (!(monkeyeAI == this) && Vector3.Distance(base.transform.position, monkeyeAI.transform.position) < this.overlapRadius && (double)Vector3.Dot(base.transform.forward, monkeyeAI.transform.forward) > 0.2)
					{
						MonkeyeAI_ReplState.EStates state = this.replState.state;
						if (state != MonkeyeAI_ReplState.EStates.Patrolling)
						{
							if (state == MonkeyeAI_ReplState.EStates.Chasing)
							{
								if (monkeyeAI.replState.state == MonkeyeAI_ReplState.EStates.Chasing)
								{
									this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
								}
							}
						}
						else
						{
							this.PickNewPath(false);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex, this);
		}
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x0001B704 File Offset: 0x00019904
	private void SetTargetPlayer([CanBeNull] VRRig rig)
	{
		if (rig == null)
		{
			this.replState.userId = "";
			this.replState.freezePlayer = false;
			this.replState.floorEnabled = true;
			this.replState.portalEnabled = false;
			this.targetRig = null;
			return;
		}
		this.replState.userId = this.UserIdFromRig(rig);
		this.targetRig = rig;
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04000531 RID: 1329
	public List<Transform> patrolPts;

	// Token: 0x04000532 RID: 1330
	public Transform sleepPt;

	// Token: 0x04000533 RID: 1331
	private int patrolIdx = -1;

	// Token: 0x04000534 RID: 1332
	private int patrolCount;

	// Token: 0x04000535 RID: 1333
	private Vector3 targetPosition;

	// Token: 0x04000536 RID: 1334
	private MaterialPropertyBlock portalMatPropBlock;

	// Token: 0x04000537 RID: 1335
	private MaterialPropertyBlock monkEyeMatPropBlock;

	// Token: 0x04000538 RID: 1336
	private Renderer renderer;

	// Token: 0x04000539 RID: 1337
	private AIDestinationSetter aiDest;

	// Token: 0x0400053A RID: 1338
	private AIPath aiPath;

	// Token: 0x0400053B RID: 1339
	private AILerp aiLerp;

	// Token: 0x0400053C RID: 1340
	private Seeker seeker;

	// Token: 0x0400053D RID: 1341
	private Path path;

	// Token: 0x0400053E RID: 1342
	private int currentWaypoint;

	// Token: 0x0400053F RID: 1343
	private bool calculatingPath;

	// Token: 0x04000540 RID: 1344
	private Monkeye_LazerFX lazerFx;

	// Token: 0x04000541 RID: 1345
	private Animator animController;

	// Token: 0x04000542 RID: 1346
	private RaycastHit[] rayResults = new RaycastHit[1];

	// Token: 0x04000543 RID: 1347
	private LayerMask layerMask;

	// Token: 0x04000544 RID: 1348
	private bool wasConnectedToRoom;

	// Token: 0x04000545 RID: 1349
	public SkinnedMeshRenderer skinnedMeshRenderer;

	// Token: 0x04000546 RID: 1350
	public MazePlayerCollection playerCollection;

	// Token: 0x04000547 RID: 1351
	public PlayerCollection playersInRoomCollection;

	// Token: 0x04000548 RID: 1352
	private List<VRRig> validRigs = new List<VRRig>();

	// Token: 0x04000549 RID: 1353
	public GameObject portalFx;

	// Token: 0x0400054A RID: 1354
	public Transform[] eyeBones;

	// Token: 0x0400054B RID: 1355
	public float speed = 0.1f;

	// Token: 0x0400054C RID: 1356
	public float rotationSpeed = 1f;

	// Token: 0x0400054D RID: 1357
	public float wakeDistance = 1f;

	// Token: 0x0400054E RID: 1358
	public float chaseDistance = 3f;

	// Token: 0x0400054F RID: 1359
	public float sleepDuration = 3f;

	// Token: 0x04000550 RID: 1360
	public float attackDistance = 0.1f;

	// Token: 0x04000551 RID: 1361
	public float beginAttackTime = 1f;

	// Token: 0x04000552 RID: 1362
	public float openFloorTime = 3f;

	// Token: 0x04000553 RID: 1363
	public float dropPlayerTime = 1f;

	// Token: 0x04000554 RID: 1364
	public float closeFloorTime = 1f;

	// Token: 0x04000555 RID: 1365
	public Color portalColor;

	// Token: 0x04000556 RID: 1366
	public Color gorillaPortalColor;

	// Token: 0x04000557 RID: 1367
	public Color monkEyeColor;

	// Token: 0x04000558 RID: 1368
	public Color monkEyeEyeColorNormal;

	// Token: 0x04000559 RID: 1369
	public Color monkEyeEyeColorAttacking;

	// Token: 0x0400055A RID: 1370
	public int maxPatrols = 4;

	// Token: 0x0400055B RID: 1371
	private VRRig targetRig;

	// Token: 0x0400055C RID: 1372
	private float deltaTime;

	// Token: 0x0400055D RID: 1373
	private float lastTime;

	// Token: 0x0400055E RID: 1374
	public MonkeyeAI_ReplState replState;

	// Token: 0x0400055F RID: 1375
	private MonkeyeAI_ReplState.EStates previousState;

	// Token: 0x04000560 RID: 1376
	private RequestableOwnershipGuard replStateRequestableOwnershipGaurd;

	// Token: 0x04000561 RID: 1377
	private RequestableOwnershipGuard myRequestableOwnershipGaurd;

	// Token: 0x04000562 RID: 1378
	private int layerBase;

	// Token: 0x04000563 RID: 1379
	private int layerForward = 1;

	// Token: 0x04000564 RID: 1380
	private int layerLeft = 2;

	// Token: 0x04000565 RID: 1381
	private int layerRight = 3;

	// Token: 0x04000566 RID: 1382
	private static readonly int EmissionColorShaderProp = Shader.PropertyToID("_EmissionColor");

	// Token: 0x04000567 RID: 1383
	private static readonly int ColorShaderProp = Shader.PropertyToID("_BaseColor");

	// Token: 0x04000568 RID: 1384
	private static readonly int EyeColorShaderProp = Shader.PropertyToID("_GChannelColor");

	// Token: 0x04000569 RID: 1385
	private static readonly int tintColorShaderProp = Shader.PropertyToID("_TintColor");

	// Token: 0x0400056A RID: 1386
	private static readonly int animStateID = Animator.StringToHash("state");

	// Token: 0x0400056B RID: 1387
	private Vector3 prevPosition;

	// Token: 0x0400056C RID: 1388
	private Vector3 velocity;

	// Token: 0x0400056D RID: 1389
	public AudioSource audioSource;

	// Token: 0x0400056E RID: 1390
	public AudioClip sleepLoopSound;

	// Token: 0x0400056F RID: 1391
	public float sleepLoopVolume = 0.5f;

	// Token: 0x04000570 RID: 1392
	[FormerlySerializedAs("moveLoopSound")]
	public AudioClip patrolLoopSound;

	// Token: 0x04000571 RID: 1393
	public float patrolLoopVolume = 0.5f;

	// Token: 0x04000572 RID: 1394
	public float patrolLoopFadeInTime = 1f;

	// Token: 0x04000573 RID: 1395
	public AudioClip chaseLoopSound;

	// Token: 0x04000574 RID: 1396
	public float chaseLoopVolume = 0.5f;

	// Token: 0x04000575 RID: 1397
	public float chaseLoopFadeInTime = 0.05f;

	// Token: 0x04000576 RID: 1398
	public AudioClip attackSound;

	// Token: 0x04000577 RID: 1399
	public float attackVolume = 0.5f;

	// Token: 0x04000578 RID: 1400
	public float overlapRadius;

	// Token: 0x04000579 RID: 1401
	private bool lockedOn;
}
