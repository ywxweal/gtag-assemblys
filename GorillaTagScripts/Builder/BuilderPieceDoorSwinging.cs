using System;
using BoingKit;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B5C RID: 2908
	public class BuilderPieceDoorSwinging : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x060047D7 RID: 18391 RVA: 0x00156834 File Offset: 0x00154A34
		private void Awake()
		{
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
			{
				builderSmallMonkeTrigger.onTriggerFirstEntered += this.OnHoldTriggerEntered;
				builderSmallMonkeTrigger.onTriggerLastExited += this.OnHoldTriggerExited;
			}
			this.frontTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnFrontTriggerEntered));
			this.backTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnBackTriggerEntered));
		}

		// Token: 0x060047D8 RID: 18392 RVA: 0x001568B4 File Offset: 0x00154AB4
		private void OnDestroy()
		{
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
			{
				builderSmallMonkeTrigger.onTriggerFirstEntered -= this.OnHoldTriggerEntered;
				builderSmallMonkeTrigger.onTriggerLastExited -= this.OnHoldTriggerExited;
			}
			this.frontTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnFrontTriggerEntered));
			this.backTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnBackTriggerEntered));
		}

		// Token: 0x060047D9 RID: 18393 RVA: 0x00156934 File Offset: 0x00154B34
		private void OnFrontTriggerEntered()
		{
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				if (NetworkSystem.Instance.IsMasterClient)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 7, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 7);
			}
		}

		// Token: 0x060047DA RID: 18394 RVA: 0x001569A8 File Offset: 0x00154BA8
		private void OnBackTriggerEntered()
		{
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				if (NetworkSystem.Instance.IsMasterClient)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 3);
			}
		}

		// Token: 0x060047DB RID: 18395 RVA: 0x00156A1C File Offset: 0x00154C1C
		private void OnHoldTriggerEntered()
		{
			this.peopleInHoldOpenVolume = true;
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			BuilderPieceDoorSwinging.SwingingDoorState swingingDoorState = this.currentState;
			if (swingingDoorState != BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				if (swingingDoorState == BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut)
				{
					this.openSound.Play();
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				if (swingingDoorState != BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn)
				{
					return;
				}
				this.openSound.Play();
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 8, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x060047DC RID: 18396 RVA: 0x00156ACC File Offset: 0x00154CCC
		private void OnHoldTriggerExited()
		{
			this.peopleInHoldOpenVolume = false;
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
			{
				builderSmallMonkeTrigger.ValidateOverlappingColliders();
				if (builderSmallMonkeTrigger.overlapCount > 0)
				{
					this.peopleInHoldOpenVolume = true;
					break;
				}
			}
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn && !this.peopleInHoldOpenVolume)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 5, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				return;
			}
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut && !this.peopleInHoldOpenVolume)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x060047DD RID: 18397 RVA: 0x00156BA0 File Offset: 0x00154DA0
		private void SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState value)
		{
			bool flag = this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed;
			bool flag2 = value == BuilderPieceDoorSwinging.SwingingDoorState.Closed;
			this.currentState = value;
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				this.frontTrigger.enabled = true;
				this.backTrigger.enabled = true;
			}
			else
			{
				this.frontTrigger.enabled = false;
				this.backTrigger.enabled = false;
			}
			if (flag != flag2)
			{
				if (flag2)
				{
					this.myPiece.GetTable().UnregisterFunctionalPiece(this);
					return;
				}
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
		}

		// Token: 0x060047DE RID: 18398 RVA: 0x00156C28 File Offset: 0x00154E28
		private void UpdateDoorStateMaster()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn:
				if (Mathf.Abs(this.doorSpring.Value) < 1f && Mathf.Abs(this.doorSpring.Velocity) < this.doorClosedVelocityMag)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.Closed);
					return;
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenIn:
				if (Time.time - this.tLastOpened > this.timeUntilDoorCloses)
				{
					this.peopleInHoldOpenVolume = false;
					foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
					{
						builderSmallMonkeTrigger.ValidateOverlappingColliders();
						if (builderSmallMonkeTrigger.overlapCount > 0)
						{
							this.peopleInHoldOpenVolume = true;
							break;
						}
					}
					if (this.peopleInHoldOpenVolume)
					{
						BuilderPieceDoorSwinging.SwingingDoorState swingingDoorState = ((this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.OpenIn) ? BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn : BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut);
						this.checkHoldTriggersTime = (double)(Time.time + this.checkHoldTriggersDelay);
						this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)swingingDoorState, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
						return;
					}
					BuilderPieceDoorSwinging.SwingingDoorState swingingDoorState2 = ((this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.OpenIn) ? BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn : BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut);
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)swingingDoorState2, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut:
				if (Mathf.Abs(this.doorSpring.Value) > 89f)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.OpenOut);
					return;
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn:
				if ((double)Time.time > this.checkHoldTriggersTime)
				{
					foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger2 in this.doorHoldTriggers)
					{
						builderSmallMonkeTrigger2.ValidateOverlappingColliders();
						if (builderSmallMonkeTrigger2.overlapCount > 0)
						{
							this.peopleInHoldOpenVolume = true;
							break;
						}
					}
					if (this.peopleInHoldOpenVolume)
					{
						this.checkHoldTriggersTime = (double)(Time.time + this.checkHoldTriggersDelay);
						return;
					}
					BuilderPieceDoorSwinging.SwingingDoorState swingingDoorState3 = ((this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn) ? BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn : BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut);
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)swingingDoorState3, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn:
				if (Mathf.Abs(this.doorSpring.Value) > 89f)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.OpenIn);
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x060047DF RID: 18399 RVA: 0x00156E70 File Offset: 0x00155070
		private void UpdateDoorState()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn:
				if (Mathf.Abs(this.doorSpring.Value) < 1f && Mathf.Abs(this.doorSpring.Velocity) < this.doorClosedVelocityMag)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.Closed);
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenIn:
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut:
				if (Mathf.Abs(this.doorSpring.Value) > 89f)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.OpenOut);
					return;
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn:
				if (Mathf.Abs(this.doorSpring.Value) > 89f)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.OpenIn);
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x060047E0 RID: 18400 RVA: 0x00156F20 File Offset: 0x00155120
		private void CloseDoor()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut:
				this.closeSound.Play();
				this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut);
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn:
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenIn:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn:
				this.closeSound.Play();
				this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn);
				return;
			default:
				return;
			}
		}

		// Token: 0x060047E1 RID: 18401 RVA: 0x00156F7E File Offset: 0x0015517E
		private void OpenDoor(bool openIn)
		{
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				this.tLastOpened = Time.time;
				this.openSound.Play();
				this.SetDoorState(openIn ? BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn : BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut);
			}
		}

		// Token: 0x060047E2 RID: 18402 RVA: 0x00156FAC File Offset: 0x001551AC
		private void UpdateDoorAnimation()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut:
				this.doorSpring.TrackDampingRatio(-90f, 3.1415927f * this.doorOpenSpeed, 1f, Time.deltaTime);
				this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
				if (this.isDoubleDoor && this.doorTransformB != null)
				{
					this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
					return;
				}
				return;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenIn:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn:
				this.doorSpring.TrackDampingRatio(90f, 3.1415927f * this.doorOpenSpeed, 1f, Time.deltaTime);
				this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
				if (this.isDoubleDoor && this.doorTransformB != null)
				{
					this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
					return;
				}
				return;
			}
			this.doorSpring.TrackDampingRatio(0f, 3.1415927f * this.doorCloseSpeed, this.dampingRatio, Time.deltaTime);
			this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
			if (this.isDoubleDoor && this.doorTransformB != null)
			{
				this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
			}
		}

		// Token: 0x060047E3 RID: 18403 RVA: 0x0015719C File Offset: 0x0015539C
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.tLastOpened = 0f;
			this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.Closed);
			this.doorSpring.Reset();
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
		}

		// Token: 0x060047E4 RID: 18404 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPieceDestroy()
		{
		}

		// Token: 0x060047E5 RID: 18405 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x060047E6 RID: 18406 RVA: 0x001571E4 File Offset: 0x001553E4
		public void OnPieceActivate()
		{
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
		}

		// Token: 0x060047E7 RID: 18407 RVA: 0x00157210 File Offset: 0x00155410
		public void OnPieceDeactivate()
		{
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			this.myPiece.functionalPieceState = 0;
			this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.Closed);
			this.doorSpring.Reset();
			this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
			if (this.isDoubleDoor && this.doorTransformB != null)
			{
				this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
			}
		}

		// Token: 0x060047E8 RID: 18408 RVA: 0x001572BC File Offset: 0x001554BC
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			switch (newState)
			{
			case 1:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.OpenOut || this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut)
				{
					this.CloseDoor();
				}
				break;
			case 3:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
				{
					this.OpenDoor(false);
				}
				break;
			case 4:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut)
				{
					this.openSound.Play();
				}
				break;
			case 5:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.OpenIn || this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn)
				{
					this.CloseDoor();
				}
				break;
			case 7:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
				{
					this.OpenDoor(true);
				}
				break;
			case 8:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn)
				{
					this.openSound.Play();
				}
				break;
			}
			this.SetDoorState((BuilderPieceDoorSwinging.SwingingDoorState)newState);
		}

		// Token: 0x060047E9 RID: 18409 RVA: 0x0015738C File Offset: 0x0015558C
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (this.IsStateValid(newState) && instigator != null && (newState == 7 || newState == 3) && this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x060047EA RID: 18410 RVA: 0x001573EA File Offset: 0x001555EA
		public bool IsStateValid(byte state)
		{
			return state <= 8;
		}

		// Token: 0x060047EB RID: 18411 RVA: 0x001573F4 File Offset: 0x001555F4
		public void FunctionalPieceUpdate()
		{
			if (this.myPiece != null && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced)
			{
				if (!NetworkSystem.Instance.InRoom && this.currentState != BuilderPieceDoorSwinging.SwingingDoorState.Closed)
				{
					this.CloseDoor();
				}
				else if (NetworkSystem.Instance.IsMasterClient)
				{
					this.UpdateDoorStateMaster();
				}
				else
				{
					this.UpdateDoorState();
				}
				this.UpdateDoorAnimation();
			}
		}

		// Token: 0x04004A54 RID: 19028
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04004A55 RID: 19029
		[SerializeField]
		private Vector3 rotateAxis = Vector3.up;

		// Token: 0x04004A56 RID: 19030
		[SerializeField]
		private Transform doorTransform;

		// Token: 0x04004A57 RID: 19031
		[SerializeField]
		private Collider[] triggerVolumes;

		// Token: 0x04004A58 RID: 19032
		[SerializeField]
		private BuilderSmallMonkeTrigger[] doorHoldTriggers;

		// Token: 0x04004A59 RID: 19033
		[SerializeField]
		private BuilderSmallHandTrigger frontTrigger;

		// Token: 0x04004A5A RID: 19034
		[SerializeField]
		private BuilderSmallHandTrigger backTrigger;

		// Token: 0x04004A5B RID: 19035
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04004A5C RID: 19036
		[SerializeField]
		private SoundBankPlayer openSound;

		// Token: 0x04004A5D RID: 19037
		[SerializeField]
		private SoundBankPlayer closeSound;

		// Token: 0x04004A5E RID: 19038
		[SerializeField]
		private float doorOpenSpeed = 1f;

		// Token: 0x04004A5F RID: 19039
		[SerializeField]
		private float doorCloseSpeed = 1f;

		// Token: 0x04004A60 RID: 19040
		[SerializeField]
		[Range(1.5f, 10f)]
		private float timeUntilDoorCloses = 3f;

		// Token: 0x04004A61 RID: 19041
		[SerializeField]
		private float doorClosedVelocityMag = 30f;

		// Token: 0x04004A62 RID: 19042
		[SerializeField]
		private float dampingRatio = 0.5f;

		// Token: 0x04004A63 RID: 19043
		[Header("Double Door Settings")]
		[SerializeField]
		private bool isDoubleDoor;

		// Token: 0x04004A64 RID: 19044
		[SerializeField]
		private Vector3 rotateAxisB = Vector3.down;

		// Token: 0x04004A65 RID: 19045
		[SerializeField]
		private Transform doorTransformB;

		// Token: 0x04004A66 RID: 19046
		private BuilderPieceDoorSwinging.SwingingDoorState currentState;

		// Token: 0x04004A67 RID: 19047
		private float tLastOpened;

		// Token: 0x04004A68 RID: 19048
		private FloatSpring doorSpring;

		// Token: 0x04004A69 RID: 19049
		private bool peopleInHoldOpenVolume;

		// Token: 0x04004A6A RID: 19050
		private double checkHoldTriggersTime;

		// Token: 0x04004A6B RID: 19051
		private float checkHoldTriggersDelay = 3f;

		// Token: 0x04004A6C RID: 19052
		private int pushDirection = 1;

		// Token: 0x02000B5D RID: 2909
		private enum SwingingDoorState
		{
			// Token: 0x04004A6E RID: 19054
			Closed,
			// Token: 0x04004A6F RID: 19055
			ClosingOut,
			// Token: 0x04004A70 RID: 19056
			OpenOut,
			// Token: 0x04004A71 RID: 19057
			OpeningOut,
			// Token: 0x04004A72 RID: 19058
			HeldOpenOut,
			// Token: 0x04004A73 RID: 19059
			ClosingIn,
			// Token: 0x04004A74 RID: 19060
			OpenIn,
			// Token: 0x04004A75 RID: 19061
			OpeningIn,
			// Token: 0x04004A76 RID: 19062
			HeldOpenIn
		}
	}
}
