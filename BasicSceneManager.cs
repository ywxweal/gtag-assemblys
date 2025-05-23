using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x0200034C RID: 844
public class BasicSceneManager : MonoBehaviour
{
	// Token: 0x060013E6 RID: 5094 RVA: 0x000605AB File Offset: 0x0005E7AB
	private void Start()
	{
		SceneManagerHelper.RequestScenePermission();
		this.LoadSceneAsync();
	}

	// Token: 0x060013E7 RID: 5095 RVA: 0x000605B8 File Offset: 0x0005E7B8
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
				List<OVRAnchor> children = new List<OVRAnchor>();
				await ovranchorContainer.FetchChildrenAsync(children);
				await this.CreateSceneAnchors(roomObject, children);
			}
		}).ToList<Task>());
	}

	// Token: 0x060013E8 RID: 5096 RVA: 0x000605F0 File Offset: 0x0005E7F0
	private async Task CreateSceneAnchors(GameObject roomGameObject, List<OVRAnchor> anchors)
	{
		BasicSceneManager.<>c__DisplayClass2_0 CS$<>8__locals1 = new BasicSceneManager.<>c__DisplayClass2_0();
		CS$<>8__locals1.roomGameObject = roomGameObject;
		await Task.WhenAll(anchors.Select(delegate(OVRAnchor anchor)
		{
			BasicSceneManager.<>c__DisplayClass2_0.<<CreateSceneAnchors>b__0>d <<CreateSceneAnchors>b__0>d;
			<<CreateSceneAnchors>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<CreateSceneAnchors>b__0>d.<>4__this = CS$<>8__locals1;
			<<CreateSceneAnchors>b__0>d.anchor = anchor;
			<<CreateSceneAnchors>b__0>d.<>1__state = -1;
			<<CreateSceneAnchors>b__0>d.<>t__builder.Start<BasicSceneManager.<>c__DisplayClass2_0.<<CreateSceneAnchors>b__0>d>(ref <<CreateSceneAnchors>b__0>d);
			return <<CreateSceneAnchors>b__0>d.<>t__builder.Task;
		}).ToList<Task>());
	}
}
