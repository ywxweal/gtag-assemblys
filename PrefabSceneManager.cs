using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000359 RID: 857
public class PrefabSceneManager : MonoBehaviour
{
	// Token: 0x06001412 RID: 5138 RVA: 0x000619AE File Offset: 0x0005FBAE
	private void Start()
	{
		SceneManagerHelper.RequestScenePermission();
		this.LoadSceneAsync();
		base.StartCoroutine(this.UpdateAnchorsPeriodically());
	}

	// Token: 0x06001413 RID: 5139 RVA: 0x000619C8 File Offset: 0x0005FBC8
	private async void LoadSceneAsync()
	{
		List<OVRAnchor> rooms = new List<OVRAnchor>();
		await OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(rooms, OVRSpace.StorageLocation.Local, 1024, 0.0);
		if (rooms.Count == 0)
		{
			TaskAwaiter<bool> taskAwaiter = SceneManagerHelper.RequestSceneCapture().GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult())
			{
				return;
			}
			await OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(rooms, OVRSpace.StorageLocation.Local, 1024, 0.0);
		}
		await Task.WhenAll(rooms.Select(async delegate(OVRAnchor room)
		{
			GameObject roomObject = new GameObject(string.Format("Room-{0}", room.Uuid));
			OVRAnchorContainer ovranchorContainer;
			if (room.TryGetComponent<OVRAnchorContainer>(out ovranchorContainer))
			{
				OVRRoomLayout roomLayout;
				if (room.TryGetComponent<OVRRoomLayout>(out roomLayout))
				{
					List<OVRAnchor> children = new List<OVRAnchor>();
					await ovranchorContainer.FetchChildrenAsync(children);
					await this.CreateSceneAnchors(roomObject, roomLayout, children);
				}
			}
		}).ToList<Task>());
	}

	// Token: 0x06001414 RID: 5140 RVA: 0x00061A00 File Offset: 0x0005FC00
	private async Task CreateSceneAnchors(GameObject roomGameObject, OVRRoomLayout roomLayout, List<OVRAnchor> anchors)
	{
		PrefabSceneManager.<>c__DisplayClass8_0 CS$<>8__locals1 = new PrefabSceneManager.<>c__DisplayClass8_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.roomGameObject = roomGameObject;
		roomLayout.TryGetRoomLayout(out CS$<>8__locals1.ceilingUuid, out CS$<>8__locals1.floorUuid, out CS$<>8__locals1.wallUuids);
		await Task.WhenAll(anchors.Select(delegate(OVRAnchor anchor)
		{
			PrefabSceneManager.<>c__DisplayClass8_0.<<CreateSceneAnchors>b__0>d <<CreateSceneAnchors>b__0>d;
			<<CreateSceneAnchors>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<CreateSceneAnchors>b__0>d.<>4__this = CS$<>8__locals1;
			<<CreateSceneAnchors>b__0>d.anchor = anchor;
			<<CreateSceneAnchors>b__0>d.<>1__state = -1;
			<<CreateSceneAnchors>b__0>d.<>t__builder.Start<PrefabSceneManager.<>c__DisplayClass8_0.<<CreateSceneAnchors>b__0>d>(ref <<CreateSceneAnchors>b__0>d);
			return <<CreateSceneAnchors>b__0>d.<>t__builder.Task;
		}).ToList<Task>());
	}

	// Token: 0x06001415 RID: 5141 RVA: 0x00061A5B File Offset: 0x0005FC5B
	private IEnumerator UpdateAnchorsPeriodically()
	{
		for (;;)
		{
			foreach (ValueTuple<GameObject, OVRLocatable> valueTuple in this._locatableObjects)
			{
				GameObject item = valueTuple.Item1;
				OVRLocatable item2 = valueTuple.Item2;
				new SceneManagerHelper(item).SetLocation(item2, null);
			}
			yield return new WaitForSeconds(this.UpdateFrequencySeconds);
		}
		yield break;
	}

	// Token: 0x0400165D RID: 5725
	public GameObject WallPrefab;

	// Token: 0x0400165E RID: 5726
	public GameObject CeilingPrefab;

	// Token: 0x0400165F RID: 5727
	public GameObject FloorPrefab;

	// Token: 0x04001660 RID: 5728
	public GameObject FallbackPrefab;

	// Token: 0x04001661 RID: 5729
	public float UpdateFrequencySeconds = 5f;

	// Token: 0x04001662 RID: 5730
	private List<ValueTuple<GameObject, OVRLocatable>> _locatableObjects = new List<ValueTuple<GameObject, OVRLocatable>>();
}
