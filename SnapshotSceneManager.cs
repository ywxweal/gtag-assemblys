using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000363 RID: 867
public class SnapshotSceneManager : MonoBehaviour
{
	// Token: 0x06001438 RID: 5176 RVA: 0x000629AA File Offset: 0x00060BAA
	private void Start()
	{
		SceneManagerHelper.RequestScenePermission();
		base.StartCoroutine(this.UpdateScenePeriodically());
	}

	// Token: 0x06001439 RID: 5177 RVA: 0x00060D86 File Offset: 0x0005EF86
	private void Update()
	{
		if (OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.Active))
		{
			SceneManagerHelper.RequestSceneCapture();
		}
	}

	// Token: 0x0600143A RID: 5178 RVA: 0x000629BE File Offset: 0x00060BBE
	private IEnumerator UpdateScenePeriodically()
	{
		for (;;)
		{
			yield return new WaitForSeconds(this.UpdateFrequencySeconds);
			this.UpdateScene();
		}
		yield break;
	}

	// Token: 0x0600143B RID: 5179 RVA: 0x000629D0 File Offset: 0x00060BD0
	private async void UpdateScene()
	{
		SnapshotSceneManager.SceneSnapshot sceneSnapshot = await this.LoadSceneSnapshotAsync();
		SnapshotSceneManager.SceneSnapshot currentSnapshot = sceneSnapshot;
		List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>> list = await new SnapshotSceneManager.SnapshotComparer(this._snapshot, currentSnapshot).Compare();
		StringBuilder stringBuilder = new StringBuilder();
		if (list.Count > 0)
		{
			stringBuilder.AppendLine("---- SCENE SNAPSHOT ----");
			foreach (ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType> valueTuple in list)
			{
				OVRAnchor item = valueTuple.Item1;
				stringBuilder.AppendLine(string.Format("{0}: {1}", valueTuple.Item2, this.AnchorInfo(item)));
			}
			Debug.Log(stringBuilder.ToString());
		}
		this._snapshot = currentSnapshot;
	}

	// Token: 0x0600143C RID: 5180 RVA: 0x00062A08 File Offset: 0x00060C08
	private async Task<SnapshotSceneManager.SceneSnapshot> LoadSceneSnapshotAsync()
	{
		SnapshotSceneManager.SceneSnapshot snapshot = new SnapshotSceneManager.SceneSnapshot();
		List<OVRAnchor> rooms = new List<OVRAnchor>();
		await OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(rooms, OVRSpace.StorageLocation.Local, 1024, 0.0);
		foreach (OVRAnchor room in rooms)
		{
			OVRAnchorContainer ovranchorContainer;
			if (room.TryGetComponent<OVRAnchorContainer>(out ovranchorContainer))
			{
				List<OVRAnchor> children = new List<OVRAnchor>();
				await ovranchorContainer.FetchChildrenAsync(children);
				snapshot.Anchors.Add(room);
				snapshot.Anchors.AddRange(children);
				children = null;
				room = default(OVRAnchor);
			}
		}
		List<OVRAnchor>.Enumerator enumerator = default(List<OVRAnchor>.Enumerator);
		return snapshot;
	}

	// Token: 0x0600143D RID: 5181 RVA: 0x00062A44 File Offset: 0x00060C44
	private string AnchorInfo(OVRAnchor anchor)
	{
		OVRRoomLayout ovrroomLayout;
		if (anchor.TryGetComponent<OVRRoomLayout>(out ovrroomLayout) && ovrroomLayout.IsEnabled)
		{
			return string.Format("{0} - ROOM", anchor.Uuid);
		}
		OVRSemanticLabels ovrsemanticLabels;
		if (anchor.TryGetComponent<OVRSemanticLabels>(out ovrsemanticLabels) && ovrsemanticLabels.IsEnabled)
		{
			return string.Format("{0} - {1}", anchor.Uuid, ovrsemanticLabels.Labels);
		}
		return string.Format("{0}", anchor.Uuid);
	}

	// Token: 0x0400168F RID: 5775
	public float UpdateFrequencySeconds = 5f;

	// Token: 0x04001690 RID: 5776
	private SnapshotSceneManager.SceneSnapshot _snapshot = new SnapshotSceneManager.SceneSnapshot();

	// Token: 0x02000364 RID: 868
	private class SceneSnapshot
	{
		// Token: 0x17000242 RID: 578
		// (get) Token: 0x0600143F RID: 5183 RVA: 0x00062AE2 File Offset: 0x00060CE2
		public List<OVRAnchor> Anchors { get; } = new List<OVRAnchor>();
	}

	// Token: 0x02000365 RID: 869
	private class SnapshotComparer
	{
		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06001441 RID: 5185 RVA: 0x00062AFD File Offset: 0x00060CFD
		public SnapshotSceneManager.SceneSnapshot BaseSnapshot { get; }

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x06001442 RID: 5186 RVA: 0x00062B05 File Offset: 0x00060D05
		public SnapshotSceneManager.SceneSnapshot NewSnapshot { get; }

		// Token: 0x06001443 RID: 5187 RVA: 0x00062B0D File Offset: 0x00060D0D
		public SnapshotComparer(SnapshotSceneManager.SceneSnapshot baseSnapshot, SnapshotSceneManager.SceneSnapshot newSnapshot)
		{
			this.BaseSnapshot = baseSnapshot;
			this.NewSnapshot = newSnapshot;
		}

		// Token: 0x06001444 RID: 5188 RVA: 0x00062B24 File Offset: 0x00060D24
		public async Task<List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>> Compare()
		{
			List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>> changes = new List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>();
			foreach (OVRAnchor ovranchor in this.BaseSnapshot.Anchors)
			{
				if (!this.NewSnapshot.Anchors.Contains(ovranchor))
				{
					changes.Add(new ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>(ovranchor, SnapshotSceneManager.SnapshotComparer.ChangeType.Missing));
				}
			}
			foreach (OVRAnchor ovranchor2 in this.NewSnapshot.Anchors)
			{
				if (!this.BaseSnapshot.Anchors.Contains(ovranchor2))
				{
					changes.Add(new ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>(ovranchor2, SnapshotSceneManager.SnapshotComparer.ChangeType.New));
				}
			}
			await this.CheckRoomChanges(changes);
			return changes;
		}

		// Token: 0x06001445 RID: 5189 RVA: 0x00062B68 File Offset: 0x00060D68
		private async Task CheckRoomChanges(List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>> changes)
		{
			for (int i = 0; i < changes.Count; i++)
			{
				ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType> valueTuple = changes[i];
				OVRAnchor anchor = valueTuple.Item1;
				SnapshotSceneManager.SnapshotComparer.ChangeType change = valueTuple.Item2;
				OVRRoomLayout ovrroomLayout;
				if (anchor.TryGetComponent<OVRRoomLayout>(out ovrroomLayout) && ovrroomLayout.IsEnabled && change != SnapshotSceneManager.SnapshotComparer.ChangeType.Changed)
				{
					List<OVRAnchor> childAnchors = new List<OVRAnchor>();
					OVRTask<bool>.Awaiter awaiter = ovrroomLayout.FetchLayoutAnchorsAsync(childAnchors).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						await awaiter;
						OVRTask<bool>.Awaiter awaiter2;
						awaiter = awaiter2;
						awaiter2 = default(OVRTask<bool>.Awaiter);
					}
					if (awaiter.GetResult())
					{
						List<OVRAnchor> list = ((change == SnapshotSceneManager.SnapshotComparer.ChangeType.New) ? this.BaseSnapshot.Anchors : this.NewSnapshot.Anchors);
						using (List<OVRAnchor>.Enumerator enumerator = childAnchors.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (list.Contains(enumerator.Current))
								{
									changes[i] = new ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>(anchor, SnapshotSceneManager.SnapshotComparer.ChangeType.Changed);
								}
							}
						}
						anchor = default(OVRAnchor);
						childAnchors = null;
					}
				}
			}
		}

		// Token: 0x02000366 RID: 870
		public enum ChangeType
		{
			// Token: 0x04001695 RID: 5781
			New,
			// Token: 0x04001696 RID: 5782
			Missing,
			// Token: 0x04001697 RID: 5783
			Changed
		}
	}
}
