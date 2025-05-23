using System;
using BoingKit;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B5A RID: 2906
	public class BuilderPieceDoor : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x060047C2 RID: 18370 RVA: 0x00155D4C File Offset: 0x00153F4C
		private void Awake()
		{
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
			{
				builderSmallMonkeTrigger.onTriggerFirstEntered += this.OnHoldTriggerEntered;
				builderSmallMonkeTrigger.onTriggerLastExited += this.OnHoldTriggerExited;
			}
			BuilderSmallHandTrigger[] array2 = this.doorButtonTriggers;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].TriggeredEvent.AddListener(new UnityAction(this.OnDoorButtonTriggered));
			}
		}

		// Token: 0x060047C3 RID: 18371 RVA: 0x00155DC4 File Offset: 0x00153FC4
		private void OnDestroy()
		{
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
			{
				builderSmallMonkeTrigger.onTriggerFirstEntered -= this.OnHoldTriggerEntered;
				builderSmallMonkeTrigger.onTriggerLastExited -= this.OnHoldTriggerExited;
			}
			BuilderSmallHandTrigger[] array2 = this.doorButtonTriggers;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].TriggeredEvent.RemoveListener(new UnityAction(this.OnDoorButtonTriggered));
			}
		}

		// Token: 0x060047C4 RID: 18372 RVA: 0x00155E3C File Offset: 0x0015403C
		private void SetDoorState(BuilderPieceDoor.DoorState value)
		{
			bool flag = this.currentState == BuilderPieceDoor.DoorState.Closed || (this.currentState == BuilderPieceDoor.DoorState.Open && this.IsToggled);
			bool flag2 = value == BuilderPieceDoor.DoorState.Closed || (value == BuilderPieceDoor.DoorState.Open && this.IsToggled);
			this.currentState = value;
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

		// Token: 0x060047C5 RID: 18373 RVA: 0x00155EAC File Offset: 0x001540AC
		private void UpdateDoorStateMaster()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoor.DoorState.Closing:
				if (this.doorSpring.Value < 1f)
				{
					this.doorSpring.Reset();
					this.doorTransform.localRotation = Quaternion.identity;
					if (this.isDoubleDoor && this.doorTransformB != null)
					{
						this.doorTransformB.localRotation = Quaternion.identity;
					}
					this.SetDoorState(BuilderPieceDoor.DoorState.Closed);
					return;
				}
				break;
			case BuilderPieceDoor.DoorState.Open:
				if (!this.IsToggled && Time.time - this.tLastOpened > this.timeUntilDoorCloses)
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
						this.CheckHoldTriggersTime = (double)(Time.time + this.checkHoldTriggersDelay);
						this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
						return;
					}
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			case BuilderPieceDoor.DoorState.Opening:
				if (this.doorSpring.Value > 89f)
				{
					this.SetDoorState(BuilderPieceDoor.DoorState.Open);
					return;
				}
				break;
			case BuilderPieceDoor.DoorState.HeldOpen:
				if (!this.IsToggled && (double)Time.time > this.CheckHoldTriggersTime)
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
						this.CheckHoldTriggersTime = (double)(Time.time + this.checkHoldTriggersDelay);
						return;
					}
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x060047C6 RID: 18374 RVA: 0x001560C0 File Offset: 0x001542C0
		private void UpdateDoorState()
		{
			BuilderPieceDoor.DoorState doorState = this.currentState;
			if (doorState != BuilderPieceDoor.DoorState.Closing)
			{
				if (doorState == BuilderPieceDoor.DoorState.Opening && this.doorSpring.Value > 89f)
				{
					this.SetDoorState(BuilderPieceDoor.DoorState.Open);
					return;
				}
			}
			else if (this.doorSpring.Value < 1f)
			{
				this.doorSpring.Reset();
				this.doorTransform.localRotation = Quaternion.identity;
				if (this.isDoubleDoor && this.doorTransformB != null)
				{
					this.doorTransformB.localRotation = Quaternion.identity;
				}
				this.SetDoorState(BuilderPieceDoor.DoorState.Closed);
			}
		}

		// Token: 0x060047C7 RID: 18375 RVA: 0x00156150 File Offset: 0x00154350
		private void CloseDoor()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoor.DoorState.Closed:
			case BuilderPieceDoor.DoorState.Closing:
			case BuilderPieceDoor.DoorState.Opening:
				return;
			case BuilderPieceDoor.DoorState.Open:
			case BuilderPieceDoor.DoorState.HeldOpen:
				this.closeSound.Play();
				this.SetDoorState(BuilderPieceDoor.DoorState.Closing);
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x060047C8 RID: 18376 RVA: 0x0015619C File Offset: 0x0015439C
		private void OpenDoor()
		{
			BuilderPieceDoor.DoorState doorState = this.currentState;
			if (doorState == BuilderPieceDoor.DoorState.Closed)
			{
				this.tLastOpened = Time.time;
				this.openSound.Play();
				this.SetDoorState(BuilderPieceDoor.DoorState.Opening);
				return;
			}
			if (doorState - BuilderPieceDoor.DoorState.Closing > 3)
			{
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x060047C9 RID: 18377 RVA: 0x001561E0 File Offset: 0x001543E0
		private void UpdateDoorAnimation()
		{
			BuilderPieceDoor.DoorState doorState = this.currentState;
			if (doorState > BuilderPieceDoor.DoorState.Closing && doorState - BuilderPieceDoor.DoorState.Open <= 2)
			{
				this.doorSpring.TrackDampingRatio(90f, 3.1415927f * this.doorOpenSpeed, 1f, Time.deltaTime);
				this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
				if (this.isDoubleDoor && this.doorTransformB != null)
				{
					this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
					return;
				}
			}
			else
			{
				this.doorSpring.TrackDampingRatio(0f, 3.1415927f * this.doorCloseSpeed, 1f, Time.deltaTime);
				this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
				if (this.isDoubleDoor && this.doorTransformB != null)
				{
					this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
				}
			}
		}

		// Token: 0x060047CA RID: 18378 RVA: 0x00156320 File Offset: 0x00154520
		private void OnDoorButtonTriggered()
		{
			BuilderPieceDoor.DoorState doorState = this.currentState;
			if (doorState != BuilderPieceDoor.DoorState.Closed)
			{
				if (doorState != BuilderPieceDoor.DoorState.Open)
				{
					return;
				}
				if (this.IsToggled)
				{
					if (NetworkSystem.Instance.IsMasterClient)
					{
						this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
						return;
					}
					this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
				}
				return;
			}
			else
			{
				if (NetworkSystem.Instance.IsMasterClient)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 3);
				return;
			}
		}

		// Token: 0x060047CB RID: 18379 RVA: 0x00156404 File Offset: 0x00154604
		private void OnHoldTriggerEntered()
		{
			this.peopleInHoldOpenVolume = true;
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			BuilderPieceDoor.DoorState doorState = this.currentState;
			if (doorState != BuilderPieceDoor.DoorState.Closed)
			{
				if (doorState != BuilderPieceDoor.DoorState.Closing)
				{
					return;
				}
				if (!this.IsToggled)
				{
					this.openSound.Play();
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				}
			}
			else if (this.isAutomatic)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				return;
			}
		}

		// Token: 0x060047CC RID: 18380 RVA: 0x001564B0 File Offset: 0x001546B0
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
			if (this.currentState == BuilderPieceDoor.DoorState.HeldOpen && !this.peopleInHoldOpenVolume)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x060047CD RID: 18381 RVA: 0x00156544 File Offset: 0x00154744
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.tLastOpened = 0f;
			this.SetDoorState(BuilderPieceDoor.DoorState.Closed);
			this.doorSpring.Reset();
			this.doorTransform.localRotation = Quaternion.identity;
			if (this.isDoubleDoor && this.doorTransformB != null)
			{
				this.doorTransformB.localRotation = Quaternion.identity;
			}
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			if (this.lineRenderers != null)
			{
				LineRenderer[] array2 = this.lineRenderers;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].widthMultiplier = this.myPiece.GetScale();
				}
			}
		}

		// Token: 0x060047CE RID: 18382 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPieceDestroy()
		{
		}

		// Token: 0x060047CF RID: 18383 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x060047D0 RID: 18384 RVA: 0x001565F4 File Offset: 0x001547F4
		public void OnPieceActivate()
		{
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
		}

		// Token: 0x060047D1 RID: 18385 RVA: 0x00156620 File Offset: 0x00154820
		public void OnPieceDeactivate()
		{
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			this.myPiece.functionalPieceState = 0;
			this.SetDoorState(BuilderPieceDoor.DoorState.Closed);
			this.doorSpring.Reset();
			this.doorTransform.localRotation = Quaternion.identity;
			if (this.isDoubleDoor && this.doorTransformB != null)
			{
				this.doorTransformB.localRotation = Quaternion.identity;
			}
		}

		// Token: 0x060047D2 RID: 18386 RVA: 0x001566A0 File Offset: 0x001548A0
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (this.IsStateValid(newState) && instigator != null && this.currentState != (BuilderPieceDoor.DoorState)newState)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x060047D3 RID: 18387 RVA: 0x001566F8 File Offset: 0x001548F8
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			switch (newState)
			{
			case 1:
				if (this.currentState == BuilderPieceDoor.DoorState.Open || this.currentState == BuilderPieceDoor.DoorState.HeldOpen)
				{
					this.CloseDoor();
				}
				break;
			case 3:
				if (this.currentState == BuilderPieceDoor.DoorState.Closed)
				{
					this.OpenDoor();
				}
				break;
			case 4:
				if (this.currentState == BuilderPieceDoor.DoorState.Closing)
				{
					this.openSound.Play();
				}
				break;
			}
			this.SetDoorState((BuilderPieceDoor.DoorState)newState);
		}

		// Token: 0x060047D4 RID: 18388 RVA: 0x00156770 File Offset: 0x00154970
		public bool IsStateValid(byte state)
		{
			return state < 5;
		}

		// Token: 0x060047D5 RID: 18389 RVA: 0x00156778 File Offset: 0x00154978
		public void FunctionalPieceUpdate()
		{
			if (this.myPiece != null && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced)
			{
				if (!NetworkSystem.Instance.InRoom && this.currentState != BuilderPieceDoor.DoorState.Closed)
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

		// Token: 0x04004A36 RID: 18998
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04004A37 RID: 18999
		[SerializeField]
		private Vector3 rotateAxis = Vector3.up;

		// Token: 0x04004A38 RID: 19000
		[Tooltip("True if the door stays open until the button is triggered again")]
		[SerializeField]
		private bool IsToggled;

		// Token: 0x04004A39 RID: 19001
		[Tooltip("True if the door opens when players enter the Keep Open Trigger")]
		[SerializeField]
		private bool isAutomatic;

		// Token: 0x04004A3A RID: 19002
		[SerializeField]
		private Transform doorTransform;

		// Token: 0x04004A3B RID: 19003
		[SerializeField]
		private Collider[] triggerVolumes;

		// Token: 0x04004A3C RID: 19004
		[SerializeField]
		private BuilderSmallHandTrigger[] doorButtonTriggers;

		// Token: 0x04004A3D RID: 19005
		[SerializeField]
		private BuilderSmallMonkeTrigger[] doorHoldTriggers;

		// Token: 0x04004A3E RID: 19006
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04004A3F RID: 19007
		[SerializeField]
		private SoundBankPlayer openSound;

		// Token: 0x04004A40 RID: 19008
		[SerializeField]
		private SoundBankPlayer closeSound;

		// Token: 0x04004A41 RID: 19009
		[SerializeField]
		private float doorOpenSpeed = 1f;

		// Token: 0x04004A42 RID: 19010
		[SerializeField]
		private float doorCloseSpeed = 1f;

		// Token: 0x04004A43 RID: 19011
		[SerializeField]
		[Range(1.5f, 10f)]
		private float timeUntilDoorCloses = 3f;

		// Token: 0x04004A44 RID: 19012
		[Header("Double Door Settings")]
		[SerializeField]
		private bool isDoubleDoor;

		// Token: 0x04004A45 RID: 19013
		[SerializeField]
		private Vector3 rotateAxisB = Vector3.down;

		// Token: 0x04004A46 RID: 19014
		[SerializeField]
		private Transform doorTransformB;

		// Token: 0x04004A47 RID: 19015
		[SerializeField]
		private LineRenderer[] lineRenderers;

		// Token: 0x04004A48 RID: 19016
		private BuilderPieceDoor.DoorState currentState;

		// Token: 0x04004A49 RID: 19017
		private float tLastOpened;

		// Token: 0x04004A4A RID: 19018
		private FloatSpring doorSpring;

		// Token: 0x04004A4B RID: 19019
		private bool peopleInHoldOpenVolume;

		// Token: 0x04004A4C RID: 19020
		private double CheckHoldTriggersTime;

		// Token: 0x04004A4D RID: 19021
		private float checkHoldTriggersDelay = 3f;

		// Token: 0x02000B5B RID: 2907
		public enum DoorState
		{
			// Token: 0x04004A4F RID: 19023
			Closed,
			// Token: 0x04004A50 RID: 19024
			Closing,
			// Token: 0x04004A51 RID: 19025
			Open,
			// Token: 0x04004A52 RID: 19026
			Opening,
			// Token: 0x04004A53 RID: 19027
			HeldOpen
		}
	}
}
