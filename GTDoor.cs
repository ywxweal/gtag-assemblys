using System;
using BoingKit;
using Fusion;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

// Token: 0x020001D7 RID: 471
public class GTDoor : NetworkSceneObject
{
	// Token: 0x06000B07 RID: 2823 RVA: 0x0003AF3C File Offset: 0x0003913C
	protected override void Start()
	{
		base.Start();
		Collider[] array = this.doorColliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		this.tLastOpened = 0f;
		GTDoorTrigger[] array2 = this.doorButtonTriggers;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].TriggeredEvent.AddListener(new UnityAction(this.DoorButtonTriggered));
		}
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x0003AFA8 File Offset: 0x000391A8
	private void Update()
	{
		if (this.currentState == GTDoor.DoorState.Open || this.currentState == GTDoor.DoorState.Closed)
		{
			if (Time.time < this.lastChecked + this.secondsCheck)
			{
				return;
			}
			this.lastChecked = Time.time;
		}
		this.UpdateDoorState();
		this.UpdateDoorAnimation();
		Collider[] array;
		if (this.currentState == GTDoor.DoorState.Closed)
		{
			array = this.doorColliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
			return;
		}
		array = this.doorColliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x0003B038 File Offset: 0x00039238
	private void UpdateDoorState()
	{
		this.peopleInHoldOpenVolume = false;
		foreach (GTDoorTrigger gtdoorTrigger in this.doorHoldOpenTriggers)
		{
			gtdoorTrigger.ValidateOverlappingColliders();
			if (gtdoorTrigger.overlapCount > 0)
			{
				this.peopleInHoldOpenVolume = true;
				break;
			}
		}
		switch (this.currentState)
		{
		case GTDoor.DoorState.Closed:
			if (this.buttonTriggeredThisFrame)
			{
				this.buttonTriggeredThisFrame = false;
				if (!NetworkSystem.Instance.InRoom)
				{
					this.OpenDoor();
				}
				else
				{
					this.currentState = GTDoor.DoorState.OpeningWaitingOnRPC;
					this.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[] { GTDoor.DoorState.Opening });
				}
			}
			break;
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
			break;
		case GTDoor.DoorState.Closing:
			if (this.doorSpring.Value < 1f)
			{
				this.currentState = GTDoor.DoorState.Closed;
			}
			if (this.peopleInHoldOpenVolume)
			{
				this.currentState = GTDoor.DoorState.HeldOpenLocally;
				if (NetworkSystem.Instance.InRoom && base.IsMine)
				{
					this.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[] { GTDoor.DoorState.HeldOpen });
				}
				this.audioSource.GTPlayOneShot(this.openSound, 1f);
			}
			break;
		case GTDoor.DoorState.Open:
			if (Time.time - this.tLastOpened > this.timeUntilDoorCloses)
			{
				if (this.peopleInHoldOpenVolume)
				{
					this.currentState = GTDoor.DoorState.HeldOpenLocally;
					if (NetworkSystem.Instance.InRoom && base.IsMine)
					{
						this.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[] { GTDoor.DoorState.HeldOpen });
					}
				}
				else if (!NetworkSystem.Instance.InRoom)
				{
					this.CloseDoor();
				}
				else if (base.IsMine)
				{
					this.currentState = GTDoor.DoorState.ClosingWaitingOnRPC;
					this.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[] { GTDoor.DoorState.Closing });
				}
			}
			break;
		case GTDoor.DoorState.Opening:
			if (this.doorSpring.Value > 89f)
			{
				this.currentState = GTDoor.DoorState.Open;
			}
			break;
		case GTDoor.DoorState.HeldOpen:
			if (!this.peopleInHoldOpenVolume)
			{
				if (!NetworkSystem.Instance.InRoom)
				{
					this.CloseDoor();
				}
				else if (base.IsMine)
				{
					this.currentState = GTDoor.DoorState.ClosingWaitingOnRPC;
					this.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[] { GTDoor.DoorState.Closing });
				}
			}
			break;
		case GTDoor.DoorState.HeldOpenLocally:
			if (!this.peopleInHoldOpenVolume)
			{
				this.CloseDoor();
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			GTDoor.DoorState doorState = this.currentState;
			if (doorState == GTDoor.DoorState.ClosingWaitingOnRPC)
			{
				this.CloseDoor();
				return;
			}
			if (doorState != GTDoor.DoorState.OpeningWaitingOnRPC)
			{
				return;
			}
			this.OpenDoor();
		}
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x0003B2DC File Offset: 0x000394DC
	private void DoorButtonTriggered()
	{
		GTDoor.DoorState doorState = this.currentState;
		if (doorState - GTDoor.DoorState.Open <= 4)
		{
			return;
		}
		this.buttonTriggeredThisFrame = true;
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x0003B300 File Offset: 0x00039500
	private void OpenDoor()
	{
		switch (this.currentState)
		{
		case GTDoor.DoorState.Closed:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
			this.ResetDoorOpenedTime();
			this.audioSource.GTPlayOneShot(this.openSound, 1f);
			this.currentState = GTDoor.DoorState.Opening;
			return;
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Closing:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.Opening:
		case GTDoor.DoorState.HeldOpen:
		case GTDoor.DoorState.HeldOpenLocally:
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x0003B368 File Offset: 0x00039568
	private void CloseDoor()
	{
		switch (this.currentState)
		{
		case GTDoor.DoorState.Closed:
		case GTDoor.DoorState.Closing:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
		case GTDoor.DoorState.Opening:
			return;
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.HeldOpen:
		case GTDoor.DoorState.HeldOpenLocally:
			this.audioSource.GTPlayOneShot(this.closeSound, 1f);
			this.currentState = GTDoor.DoorState.Closing;
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x0003B3C8 File Offset: 0x000395C8
	private void UpdateDoorAnimation()
	{
		switch (this.currentState)
		{
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.Opening:
		case GTDoor.DoorState.HeldOpen:
		case GTDoor.DoorState.HeldOpenLocally:
			this.doorSpring.TrackDampingRatio(90f, 3.1415927f * this.doorOpenSpeed, 1f, Time.deltaTime);
			this.doorTransform.localRotation = Quaternion.Euler(new Vector3(0f, this.doorSpring.Value, 0f));
			return;
		}
		this.doorSpring.TrackDampingRatio(0f, 3.1415927f * this.doorCloseSpeed, 1f, Time.deltaTime);
		this.doorTransform.localRotation = Quaternion.Euler(new Vector3(0f, this.doorSpring.Value, 0f));
	}

	// Token: 0x06000B0E RID: 2830 RVA: 0x0003B4A7 File Offset: 0x000396A7
	public void ResetDoorOpenedTime()
	{
		this.tLastOpened = Time.time;
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x0003B4B4 File Offset: 0x000396B4
	[PunRPC]
	public void ChangeDoorState(GTDoor.DoorState shouldOpenState)
	{
		this.ChangeDoorStateShared(shouldOpenState);
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x0003B4C0 File Offset: 0x000396C0
	[Rpc]
	public unsafe static void RPC_ChangeDoorState(NetworkRunner runner, GTDoor.DoorState shouldOpenState, int doorId)
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
		}
		else
		{
			if (runner == null)
			{
				throw new ArgumentNullException("runner");
			}
			if (runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			if (runner.HasAnyActiveConnections())
			{
				int num = 8;
				num += 4;
				num += 4;
				SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GTDoor::RPC_ChangeDoorState(Fusion.NetworkRunner,GTDoor/DoorState,System.Int32)")), data);
				*(GTDoor.DoorState*)(data + num2) = shouldOpenState;
				num2 += 4;
				*(int*)(data + num2) = doorId;
				num2 += 4;
				ptr->Offset = num2 * 8;
				ptr->SetStatic();
				runner.SendRpc(ptr);
			}
		}
		GTDoor[] array = global::UnityEngine.Object.FindObjectsOfType<GTDoor>(true);
		if (array == null || array.Length == 0)
		{
			return;
		}
		foreach (GTDoor gtdoor in array)
		{
			if (gtdoor.GTDoorID == doorId)
			{
				gtdoor.ChangeDoorStateShared(shouldOpenState);
			}
		}
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x0003B5F8 File Offset: 0x000397F8
	private void ChangeDoorStateShared(GTDoor.DoorState shouldOpenState)
	{
		switch (shouldOpenState)
		{
		case GTDoor.DoorState.Closed:
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
		case GTDoor.DoorState.HeldOpenLocally:
			break;
		case GTDoor.DoorState.Closing:
			switch (this.currentState)
			{
			case GTDoor.DoorState.Closed:
			case GTDoor.DoorState.Closing:
			case GTDoor.DoorState.OpeningWaitingOnRPC:
			case GTDoor.DoorState.Opening:
			case GTDoor.DoorState.HeldOpenLocally:
				break;
			case GTDoor.DoorState.ClosingWaitingOnRPC:
			case GTDoor.DoorState.Open:
			case GTDoor.DoorState.HeldOpen:
				this.CloseDoor();
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
			break;
		case GTDoor.DoorState.Opening:
			switch (this.currentState)
			{
			case GTDoor.DoorState.Closed:
			case GTDoor.DoorState.OpeningWaitingOnRPC:
				this.OpenDoor();
				return;
			case GTDoor.DoorState.ClosingWaitingOnRPC:
			case GTDoor.DoorState.Closing:
			case GTDoor.DoorState.Open:
			case GTDoor.DoorState.Opening:
			case GTDoor.DoorState.HeldOpen:
			case GTDoor.DoorState.HeldOpenLocally:
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			break;
		case GTDoor.DoorState.HeldOpen:
			switch (this.currentState)
			{
			case GTDoor.DoorState.Closed:
			case GTDoor.DoorState.ClosingWaitingOnRPC:
			case GTDoor.DoorState.OpeningWaitingOnRPC:
			case GTDoor.DoorState.Opening:
			case GTDoor.DoorState.HeldOpen:
				break;
			case GTDoor.DoorState.Closing:
				this.audioSource.GTPlayOneShot(this.openSound, 1f);
				this.currentState = GTDoor.DoorState.HeldOpen;
				return;
			case GTDoor.DoorState.Open:
			case GTDoor.DoorState.HeldOpenLocally:
				this.currentState = GTDoor.DoorState.HeldOpen;
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
			break;
		default:
			throw new ArgumentOutOfRangeException("shouldOpenState", shouldOpenState, null);
		}
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x0003B718 File Offset: 0x00039918
	public void SetupDoorIDs()
	{
		GTDoor[] array = global::UnityEngine.Object.FindObjectsOfType<GTDoor>(true);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GTDoorID = i + 1;
		}
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x0003B77C File Offset: 0x0003997C
	[NetworkRpcStaticWeavedInvoker("System.Void GTDoor::RPC_ChangeDoorState(Fusion.NetworkRunner,GTDoor/DoorState,System.Int32)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ChangeDoorState@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		GTDoor.DoorState doorState = *(GTDoor.DoorState*)(data + num);
		num += 4;
		GTDoor.DoorState doorState2 = doorState;
		int num2 = *(int*)(data + num);
		num += 4;
		int num3 = num2;
		NetworkBehaviourUtils.InvokeRpc = true;
		GTDoor.RPC_ChangeDoorState(runner, doorState2, num3);
	}

	// Token: 0x04000D72 RID: 3442
	[SerializeField]
	private Transform doorTransform;

	// Token: 0x04000D73 RID: 3443
	[SerializeField]
	private Collider[] doorColliders;

	// Token: 0x04000D74 RID: 3444
	[SerializeField]
	private GTDoorTrigger[] doorButtonTriggers;

	// Token: 0x04000D75 RID: 3445
	[SerializeField]
	private GTDoorTrigger[] doorHoldOpenTriggers;

	// Token: 0x04000D76 RID: 3446
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000D77 RID: 3447
	[SerializeField]
	private AudioClip openSound;

	// Token: 0x04000D78 RID: 3448
	[SerializeField]
	private AudioClip closeSound;

	// Token: 0x04000D79 RID: 3449
	[SerializeField]
	private float doorOpenSpeed = 1f;

	// Token: 0x04000D7A RID: 3450
	[SerializeField]
	private float doorCloseSpeed = 1f;

	// Token: 0x04000D7B RID: 3451
	[SerializeField]
	[Range(1.5f, 10f)]
	private float timeUntilDoorCloses = 3f;

	// Token: 0x04000D7C RID: 3452
	private int GTDoorID;

	// Token: 0x04000D7D RID: 3453
	[DebugOption]
	private GTDoor.DoorState currentState;

	// Token: 0x04000D7E RID: 3454
	private float tLastOpened;

	// Token: 0x04000D7F RID: 3455
	private FloatSpring doorSpring;

	// Token: 0x04000D80 RID: 3456
	[DebugOption]
	private bool peopleInHoldOpenVolume;

	// Token: 0x04000D81 RID: 3457
	[DebugOption]
	private bool buttonTriggeredThisFrame;

	// Token: 0x04000D82 RID: 3458
	private float lastChecked;

	// Token: 0x04000D83 RID: 3459
	private float secondsCheck = 1f;

	// Token: 0x020001D8 RID: 472
	public enum DoorState
	{
		// Token: 0x04000D85 RID: 3461
		Closed,
		// Token: 0x04000D86 RID: 3462
		ClosingWaitingOnRPC,
		// Token: 0x04000D87 RID: 3463
		Closing,
		// Token: 0x04000D88 RID: 3464
		Open,
		// Token: 0x04000D89 RID: 3465
		OpeningWaitingOnRPC,
		// Token: 0x04000D8A RID: 3466
		Opening,
		// Token: 0x04000D8B RID: 3467
		HeldOpen,
		// Token: 0x04000D8C RID: 3468
		HeldOpenLocally
	}
}
