using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000CB RID: 203
public class GhostLab : MonoBehaviour, IBuildValidation
{
	// Token: 0x060004FC RID: 1276 RVA: 0x0001CA36 File Offset: 0x0001AC36
	private void Awake()
	{
		this.relState = Object.FindFirstObjectByType<GhostLabReliableState>();
		this.doorState = GhostLab.EntranceDoorsState.BothClosed;
		this.doorOpen = new bool[this.slidingDoor.Length];
		this.toggleDoorsParent.GetComponentsInChildren<GhostLabButton>();
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x0001CA6C File Offset: 0x0001AC6C
	public bool BuildValidationCheck()
	{
		if (this.entranceDoorScanner == null)
		{
			Debug.LogError("door scanner missing", base.gameObject);
			return false;
		}
		if (this.outerDoor == null || this.innerDoor == null)
		{
			Debug.LogError("sliding doors missing", base.gameObject);
			return false;
		}
		if (this.toggleDoorsParent == null)
		{
			Debug.LogError("missing reference to parent of toggleable doors", base.gameObject);
			return false;
		}
		List<int> list = new List<int>();
		GhostLabButton[] componentsInChildren = this.toggleDoorsParent.GetComponentsInChildren<GhostLabButton>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!list.Contains(componentsInChildren[i].buttonIndex))
			{
				list.Add(componentsInChildren[i].buttonIndex);
			}
		}
		if (list.Count != this.slidingDoor.Length)
		{
			Debug.LogError("slidingDoor array not set to the correct length", base.gameObject);
			return false;
		}
		for (int j = 0; j < list.Count; j++)
		{
			if (!list.Contains(j))
			{
				Debug.LogError("door indices not continuous", base.gameObject);
				return false;
			}
		}
		return true;
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x0001CB74 File Offset: 0x0001AD74
	public void DoorButtonPress(int buttonIndex, bool forSingleDoor)
	{
		if (!forSingleDoor)
		{
			this.UpdateEntranceDoorsState(buttonIndex);
			return;
		}
		this.UpdateDoorState(buttonIndex);
		this.relState.UpdateSingleDoorState(buttonIndex);
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x0001CB94 File Offset: 0x0001AD94
	public void UpdateDoorState(int buttonIndex)
	{
		if ((this.doorOpen[buttonIndex] && this.slidingDoor[buttonIndex].localPosition == this.singleDoorTravelDistance) || (!this.doorOpen[buttonIndex] && this.slidingDoor[buttonIndex].localPosition == Vector3.zero))
		{
			this.doorOpen[buttonIndex] = !this.doorOpen[buttonIndex];
		}
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x0001CBFC File Offset: 0x0001ADFC
	public void UpdateEntranceDoorsState(int buttonIndex)
	{
		if (this.doorState == GhostLab.EntranceDoorsState.BothClosed)
		{
			if (!(this.innerDoor.localPosition != Vector3.zero) && !(this.outerDoor.localPosition != Vector3.zero))
			{
				if (buttonIndex == 0 || buttonIndex == 1)
				{
					this.doorState = GhostLab.EntranceDoorsState.OuterDoorOpen;
				}
				if (buttonIndex == 2 || buttonIndex == 3)
				{
					this.doorState = GhostLab.EntranceDoorsState.InnerDoorOpen;
				}
			}
		}
		else if (this.innerDoor.localPosition == this.doorTravelDistance || this.outerDoor.localPosition == this.doorTravelDistance)
		{
			this.doorState = GhostLab.EntranceDoorsState.BothClosed;
		}
		this.relState.UpdateEntranceDoorsState(this.doorState);
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x0001CCA8 File Offset: 0x0001AEA8
	public void Update()
	{
		this.SynchStates();
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		switch (this.doorState)
		{
		case GhostLab.EntranceDoorsState.InnerDoorOpen:
			zero2 = this.doorTravelDistance;
			break;
		case GhostLab.EntranceDoorsState.OuterDoorOpen:
			zero = this.doorTravelDistance;
			break;
		}
		this.outerDoor.localPosition = Vector3.MoveTowards(this.outerDoor.localPosition, zero, this.doorMoveSpeed * Time.deltaTime);
		this.innerDoor.localPosition = Vector3.MoveTowards(this.innerDoor.localPosition, zero2, this.doorMoveSpeed * Time.deltaTime);
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < this.slidingDoor.Length; i++)
		{
			if (this.doorOpen[i])
			{
				vector = this.singleDoorTravelDistance;
			}
			else
			{
				vector = Vector3.zero;
			}
			this.slidingDoor[i].localPosition = Vector3.MoveTowards(this.slidingDoor[i].localPosition, vector, this.singleDoorMoveSpeed * Time.deltaTime);
		}
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x0001CDA8 File Offset: 0x0001AFA8
	private void SynchStates()
	{
		this.doorState = this.relState.doorState;
		for (int i = 0; i < this.doorOpen.Length; i++)
		{
			this.doorOpen[i] = this.relState.singleDoorOpen[i];
		}
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x0001CDF0 File Offset: 0x0001AFF0
	public bool IsDoorMoving(bool singleDoor, int index)
	{
		if (singleDoor)
		{
			return (this.doorOpen[index] && this.slidingDoor[index].localPosition != this.singleDoorTravelDistance) || (!this.doorOpen[index] && this.slidingDoor[index].localPosition != Vector3.zero);
		}
		if (index == 0 || index == 1)
		{
			return (this.doorState == GhostLab.EntranceDoorsState.OuterDoorOpen && this.outerDoor.localPosition != this.doorTravelDistance) || (this.doorState != GhostLab.EntranceDoorsState.OuterDoorOpen && this.outerDoor.localPosition != Vector3.zero);
		}
		return (this.doorState == GhostLab.EntranceDoorsState.InnerDoorOpen && this.innerDoor.localPosition != this.doorTravelDistance) || (this.doorState != GhostLab.EntranceDoorsState.InnerDoorOpen && this.innerDoor.localPosition != Vector3.zero);
	}

	// Token: 0x040005D9 RID: 1497
	public IDCardScanner entranceDoorScanner;

	// Token: 0x040005DA RID: 1498
	public Transform outerDoor;

	// Token: 0x040005DB RID: 1499
	public Transform innerDoor;

	// Token: 0x040005DC RID: 1500
	public Vector3 doorTravelDistance;

	// Token: 0x040005DD RID: 1501
	public float doorMoveSpeed;

	// Token: 0x040005DE RID: 1502
	public float singleDoorMoveSpeed;

	// Token: 0x040005DF RID: 1503
	public GhostLab.EntranceDoorsState doorState;

	// Token: 0x040005E0 RID: 1504
	public GhostLabReliableState relState;

	// Token: 0x040005E1 RID: 1505
	public Transform toggleDoorsParent;

	// Token: 0x040005E2 RID: 1506
	public Transform[] slidingDoor;

	// Token: 0x040005E3 RID: 1507
	public Vector3 singleDoorTravelDistance;

	// Token: 0x040005E4 RID: 1508
	private bool[] doorOpen;

	// Token: 0x020000CC RID: 204
	public enum EntranceDoorsState
	{
		// Token: 0x040005E6 RID: 1510
		BothClosed,
		// Token: 0x040005E7 RID: 1511
		InnerDoorOpen,
		// Token: 0x040005E8 RID: 1512
		OuterDoorOpen
	}
}
