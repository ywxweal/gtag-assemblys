using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicSceneManagerHelper;
using UnityEngine;

// Token: 0x02000352 RID: 850
public class DynamicSceneManager : MonoBehaviour
{
	// Token: 0x060013F5 RID: 5109 RVA: 0x00060D72 File Offset: 0x0005EF72
	private void Start()
	{
		SceneManagerHelper.RequestScenePermission();
		base.StartCoroutine(this.UpdateScenePeriodically());
	}

	// Token: 0x060013F6 RID: 5110 RVA: 0x00060D86 File Offset: 0x0005EF86
	private void Update()
	{
		if (OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.Active))
		{
			SceneManagerHelper.RequestSceneCapture();
		}
	}

	// Token: 0x060013F7 RID: 5111 RVA: 0x00060D9B File Offset: 0x0005EF9B
	private IEnumerator UpdateScenePeriodically()
	{
		for (;;)
		{
			yield return new WaitForSeconds(this.UpdateFrequencySeconds);
			this._updateSceneTask = this.UpdateScene();
			yield return new WaitUntil(() => this._updateSceneTask.IsCompleted);
		}
		yield break;
	}

	// Token: 0x060013F8 RID: 5112 RVA: 0x00060DAC File Offset: 0x0005EFAC
	private async Task UpdateScene()
	{
		SceneSnapshot sceneSnapshot = await this.LoadSceneSnapshotAsync();
		SceneSnapshot currentSnapshot = sceneSnapshot;
		List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> list = new SnapshotComparer(this._snapshot, currentSnapshot).Compare();
		await this.UpdateUnityObjects(list, currentSnapshot);
		this._snapshot = currentSnapshot;
	}

	// Token: 0x060013F9 RID: 5113 RVA: 0x00060DF0 File Offset: 0x0005EFF0
	private async Task<SceneSnapshot> LoadSceneSnapshotAsync()
	{
		SceneSnapshot snapshot = new SceneSnapshot();
		List<OVRAnchor> rooms = new List<OVRAnchor>();
		await OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(rooms, OVRSpace.StorageLocation.Local, 1024, 0.0);
		foreach (OVRAnchor room in rooms)
		{
			OVRAnchorContainer ovranchorContainer;
			if (room.TryGetComponent<OVRAnchorContainer>(out ovranchorContainer))
			{
				List<OVRAnchor> children = new List<OVRAnchor>();
				await ovranchorContainer.FetchChildrenAsync(children);
				snapshot.Anchors.Add(room, new SceneSnapshot.Data
				{
					Children = children
				});
				foreach (OVRAnchor ovranchor in children)
				{
					SceneSnapshot.Data data = new SceneSnapshot.Data();
					OVRBounded2D ovrbounded2D;
					if (ovranchor.TryGetComponent<OVRBounded2D>(out ovrbounded2D) && ovrbounded2D.IsEnabled)
					{
						data.Rect = new Rect?(ovrbounded2D.BoundingBox);
					}
					OVRBounded3D ovrbounded3D;
					if (ovranchor.TryGetComponent<OVRBounded3D>(out ovrbounded3D) && ovrbounded3D.IsEnabled)
					{
						data.Bounds = new Bounds?(ovrbounded3D.BoundingBox);
					}
					snapshot.Anchors.Add(ovranchor, data);
				}
				children = null;
				room = default(OVRAnchor);
			}
		}
		List<OVRAnchor>.Enumerator enumerator = default(List<OVRAnchor>.Enumerator);
		return snapshot;
	}

	// Token: 0x060013FA RID: 5114 RVA: 0x00060E2C File Offset: 0x0005F02C
	private async Task UpdateUnityObjects(List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> changes, SceneSnapshot newSnapshot)
	{
		if (changes.Any<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>>())
		{
			UnityObjectUpdater updater = new UnityObjectUpdater();
			List<OVRAnchor> list = this.FilterChanges(changes, SnapshotComparer.ChangeType.New);
			List<OVRAnchor> changesMissing = this.FilterChanges(changes, SnapshotComparer.ChangeType.Missing);
			List<OVRAnchor> changesId = this.FilterChanges(changes, SnapshotComparer.ChangeType.ChangedId);
			List<OVRAnchor> changesBounds = this.FilterChanges(changes, SnapshotComparer.ChangeType.ChangedBounds);
			foreach (OVRAnchor ovranchor in list)
			{
				GameObject gameObject;
				this._sceneGameObjects.TryGetValue(this.GetParentAnchor(ovranchor, newSnapshot), out gameObject);
				Dictionary<OVRAnchor, GameObject> dictionary = this._sceneGameObjects;
				OVRAnchor ovranchor2 = ovranchor;
				GameObject gameObject2 = await updater.CreateUnityObject(ovranchor, gameObject);
				dictionary.Add(ovranchor2, gameObject2);
				dictionary = null;
				ovranchor2 = default(OVRAnchor);
			}
			List<OVRAnchor>.Enumerator enumerator = default(List<OVRAnchor>.Enumerator);
			foreach (OVRAnchor ovranchor3 in changesMissing)
			{
				Object.Destroy(this._sceneGameObjects[ovranchor3]);
				this._sceneGameObjects.Remove(ovranchor3);
			}
			foreach (ValueTuple<OVRAnchor, OVRAnchor> valueTuple in this.FindAnchorPairs(changesId, newSnapshot))
			{
				OVRAnchor item = valueTuple.Item1;
				OVRAnchor item2 = valueTuple.Item2;
				this._sceneGameObjects.Add(item2, this._sceneGameObjects[item]);
				this._sceneGameObjects.Remove(item);
			}
			foreach (OVRAnchor ovranchor4 in changesBounds)
			{
				updater.UpdateUnityObject(ovranchor4, this._sceneGameObjects[ovranchor4]);
			}
		}
	}

	// Token: 0x060013FB RID: 5115 RVA: 0x00060E80 File Offset: 0x0005F080
	private List<OVRAnchor> FilterChanges(List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> changes, SnapshotComparer.ChangeType changeType)
	{
		return (from tuple in changes
			where tuple.Item2 == changeType
			select tuple.Item1).ToList<OVRAnchor>();
	}

	// Token: 0x060013FC RID: 5116 RVA: 0x00060ED8 File Offset: 0x0005F0D8
	private List<ValueTuple<OVRAnchor, OVRAnchor>> FindAnchorPairs(List<OVRAnchor> allAnchors, SceneSnapshot newSnapshot)
	{
		IEnumerable<OVRAnchor> enumerable = allAnchors.Where(new Func<OVRAnchor, bool>(this._snapshot.Contains));
		IEnumerable<OVRAnchor> enumerable2 = allAnchors.Where(new Func<OVRAnchor, bool>(newSnapshot.Contains));
		List<ValueTuple<OVRAnchor, OVRAnchor>> list = new List<ValueTuple<OVRAnchor, OVRAnchor>>();
		foreach (OVRAnchor ovranchor in enumerable)
		{
			foreach (OVRAnchor ovranchor2 in enumerable2)
			{
				if (this.AreAnchorsEqual(this._snapshot.Anchors[ovranchor], newSnapshot.Anchors[ovranchor2]))
				{
					list.Add(new ValueTuple<OVRAnchor, OVRAnchor>(ovranchor, ovranchor2));
					break;
				}
			}
		}
		return list;
	}

	// Token: 0x060013FD RID: 5117 RVA: 0x00060FB8 File Offset: 0x0005F1B8
	private bool AreAnchorsEqual(SceneSnapshot.Data anchor1Data, SceneSnapshot.Data anchor2Data)
	{
		return anchor1Data.Children != null && anchor2Data.Children != null && (anchor1Data.Children.Any(new Func<OVRAnchor, bool>(anchor2Data.Children.Contains)) || anchor2Data.Children.Any(new Func<OVRAnchor, bool>(anchor1Data.Children.Contains)));
	}

	// Token: 0x060013FE RID: 5118 RVA: 0x00061018 File Offset: 0x0005F218
	private OVRAnchor GetParentAnchor(OVRAnchor childAnchor, SceneSnapshot snapshot)
	{
		foreach (KeyValuePair<OVRAnchor, SceneSnapshot.Data> keyValuePair in snapshot.Anchors)
		{
			List<OVRAnchor> children = keyValuePair.Value.Children;
			if (children != null && children.Contains(childAnchor))
			{
				return keyValuePair.Key;
			}
		}
		return OVRAnchor.Null;
	}

	// Token: 0x04001638 RID: 5688
	public float UpdateFrequencySeconds = 5f;

	// Token: 0x04001639 RID: 5689
	private SceneSnapshot _snapshot = new SceneSnapshot();

	// Token: 0x0400163A RID: 5690
	private Dictionary<OVRAnchor, GameObject> _sceneGameObjects = new Dictionary<OVRAnchor, GameObject>();

	// Token: 0x0400163B RID: 5691
	private Task _updateSceneTask;
}
