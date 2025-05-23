using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DynamicSceneManagerHelper
{
	// Token: 0x02000BBF RID: 3007
	internal class SnapshotComparer
	{
		// Token: 0x1700072E RID: 1838
		// (get) Token: 0x06004A59 RID: 19033 RVA: 0x00161DBB File Offset: 0x0015FFBB
		public SceneSnapshot BaseSnapshot { get; }

		// Token: 0x1700072F RID: 1839
		// (get) Token: 0x06004A5A RID: 19034 RVA: 0x00161DC3 File Offset: 0x0015FFC3
		public SceneSnapshot NewSnapshot { get; }

		// Token: 0x06004A5B RID: 19035 RVA: 0x00161DCB File Offset: 0x0015FFCB
		public SnapshotComparer(SceneSnapshot baseSnapshot, SceneSnapshot newSnapshot)
		{
			this.BaseSnapshot = baseSnapshot;
			this.NewSnapshot = newSnapshot;
		}

		// Token: 0x06004A5C RID: 19036 RVA: 0x00161DE4 File Offset: 0x0015FFE4
		public List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> Compare()
		{
			List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> list = new List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>>();
			foreach (OVRAnchor ovranchor in this.BaseSnapshot.Anchors.Keys)
			{
				if (!this.NewSnapshot.Contains(ovranchor))
				{
					list.Add(new ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>(ovranchor, SnapshotComparer.ChangeType.Missing));
				}
			}
			foreach (OVRAnchor ovranchor2 in this.NewSnapshot.Anchors.Keys)
			{
				if (!this.BaseSnapshot.Contains(ovranchor2))
				{
					list.Add(new ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>(ovranchor2, SnapshotComparer.ChangeType.New));
				}
			}
			this.CheckRoomChanges(list);
			this.CheckBoundsChanges(list);
			return list;
		}

		// Token: 0x06004A5D RID: 19037 RVA: 0x00161ECC File Offset: 0x001600CC
		private void CheckRoomChanges(List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> changes)
		{
			for (int i = 0; i < changes.Count; i++)
			{
				ValueTuple<OVRAnchor, SnapshotComparer.ChangeType> valueTuple = changes[i];
				OVRAnchor item = valueTuple.Item1;
				SnapshotComparer.ChangeType item2 = valueTuple.Item2;
				OVRRoomLayout ovrroomLayout;
				if (item.TryGetComponent<OVRRoomLayout>(out ovrroomLayout) && ovrroomLayout.IsEnabled && item2 != SnapshotComparer.ChangeType.ChangedId)
				{
					bool flag = this.NewSnapshot.Contains(item);
					bool flag2 = this.BaseSnapshot.Contains(item);
					if (flag || flag2)
					{
						List<OVRAnchor> list = (flag ? this.NewSnapshot.Anchors[item].Children : this.BaseSnapshot.Anchors[item].Children);
						SceneSnapshot sceneSnapshot = ((item2 == SnapshotComparer.ChangeType.New) ? this.BaseSnapshot : this.NewSnapshot);
						foreach (OVRAnchor ovranchor in list)
						{
							if (sceneSnapshot.Contains(ovranchor))
							{
								changes[i] = new ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>(item, SnapshotComparer.ChangeType.ChangedId);
							}
						}
					}
				}
			}
		}

		// Token: 0x06004A5E RID: 19038 RVA: 0x00161FE8 File Offset: 0x001601E8
		private void CheckBoundsChanges(List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> changes)
		{
			using (Dictionary<OVRAnchor, SceneSnapshot.Data>.KeyCollection.Enumerator enumerator = this.BaseSnapshot.Anchors.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					OVRAnchor baseAnchor = enumerator.Current;
					OVRAnchor ovranchor = this.NewSnapshot.Anchors.Keys.FirstOrDefault((OVRAnchor newAnchor) => newAnchor.Uuid == baseAnchor.Uuid);
					if (ovranchor.Uuid == baseAnchor.Uuid)
					{
						SceneSnapshot.Data data = this.BaseSnapshot.Anchors[baseAnchor];
						SceneSnapshot.Data data2 = this.NewSnapshot.Anchors[ovranchor];
						bool flag = this.Has2DBounds(data, data2) && this.Are2DBoundsDifferent(data, data2);
						bool flag2 = this.Has3DBounds(data, data2) && this.Are3DBoundsDifferent(data, data2);
						if (flag || flag2)
						{
							changes.Add(new ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>(baseAnchor, SnapshotComparer.ChangeType.ChangedBounds));
						}
					}
				}
			}
		}

		// Token: 0x06004A5F RID: 19039 RVA: 0x001620FC File Offset: 0x001602FC
		private bool Has2DBounds(SceneSnapshot.Data data1, SceneSnapshot.Data data2)
		{
			return data1.Rect != null && data2.Rect != null;
		}

		// Token: 0x06004A60 RID: 19040 RVA: 0x00162118 File Offset: 0x00160318
		private bool Are2DBoundsDifferent(SceneSnapshot.Data data1, SceneSnapshot.Data data2)
		{
			Vector2? vector = ((data1.Rect != null) ? new Vector2?(data1.Rect.GetValueOrDefault().min) : null);
			if (!(vector != ((data2.Rect != null) ? new Vector2?(data2.Rect.GetValueOrDefault().min) : null)))
			{
				Vector2? vector2 = ((data1.Rect != null) ? new Vector2?(data1.Rect.GetValueOrDefault().max) : null);
				return vector2 != ((data2.Rect != null) ? new Vector2?(data2.Rect.GetValueOrDefault().max) : null);
			}
			return true;
		}

		// Token: 0x06004A61 RID: 19041 RVA: 0x00162242 File Offset: 0x00160442
		private bool Has3DBounds(SceneSnapshot.Data data1, SceneSnapshot.Data data2)
		{
			return data1.Bounds != null && data2.Bounds != null;
		}

		// Token: 0x06004A62 RID: 19042 RVA: 0x00162260 File Offset: 0x00160460
		private bool Are3DBoundsDifferent(SceneSnapshot.Data data1, SceneSnapshot.Data data2)
		{
			Vector3? vector = ((data1.Bounds != null) ? new Vector3?(data1.Bounds.GetValueOrDefault().min) : null);
			if (!(vector != ((data2.Bounds != null) ? new Vector3?(data2.Bounds.GetValueOrDefault().min) : null)))
			{
				Vector3? vector2 = ((data1.Bounds != null) ? new Vector3?(data1.Bounds.GetValueOrDefault().max) : null);
				return vector2 != ((data2.Bounds != null) ? new Vector3?(data2.Bounds.GetValueOrDefault().max) : null);
			}
			return true;
		}

		// Token: 0x02000BC0 RID: 3008
		public enum ChangeType
		{
			// Token: 0x04004D1E RID: 19742
			New,
			// Token: 0x04004D1F RID: 19743
			Missing,
			// Token: 0x04004D20 RID: 19744
			ChangedId,
			// Token: 0x04004D21 RID: 19745
			ChangedBounds
		}
	}
}
