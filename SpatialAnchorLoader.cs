using System;
using UnityEngine;

// Token: 0x0200037F RID: 895
public class SpatialAnchorLoader : MonoBehaviour
{
	// Token: 0x060014BE RID: 5310 RVA: 0x00065260 File Offset: 0x00063460
	public void LoadAnchorsByUuid()
	{
		if (!PlayerPrefs.HasKey("numUuids"))
		{
			PlayerPrefs.SetInt("numUuids", 0);
		}
		int @int = PlayerPrefs.GetInt("numUuids");
		SpatialAnchorLoader.Log(string.Format("Attempting to load {0} saved anchors.", @int));
		if (@int == 0)
		{
			return;
		}
		Guid[] array = new Guid[@int];
		for (int i = 0; i < @int; i++)
		{
			string @string = PlayerPrefs.GetString("uuid" + i.ToString());
			SpatialAnchorLoader.Log("QueryAnchorByUuid: " + @string);
			array[i] = new Guid(@string);
		}
		this.Load(new OVRSpatialAnchor.LoadOptions
		{
			Timeout = 0.0,
			StorageLocation = OVRSpace.StorageLocation.Local,
			Uuids = array
		});
	}

	// Token: 0x060014BF RID: 5311 RVA: 0x0006531F File Offset: 0x0006351F
	private void Awake()
	{
		this._onLoadAnchor = new Action<OVRSpatialAnchor.UnboundAnchor, bool>(this.OnLocalized);
	}

	// Token: 0x060014C0 RID: 5312 RVA: 0x00065333 File Offset: 0x00063533
	private void Load(OVRSpatialAnchor.LoadOptions options)
	{
		OVRSpatialAnchor.LoadUnboundAnchors(options, delegate(OVRSpatialAnchor.UnboundAnchor[] anchors)
		{
			if (anchors == null)
			{
				SpatialAnchorLoader.Log("Query failed.");
				return;
			}
			foreach (OVRSpatialAnchor.UnboundAnchor unboundAnchor in anchors)
			{
				if (unboundAnchor.Localized)
				{
					this._onLoadAnchor(unboundAnchor, true);
				}
				else if (!unboundAnchor.Localizing)
				{
					unboundAnchor.Localize(this._onLoadAnchor, 0.0);
				}
			}
		});
	}

	// Token: 0x060014C1 RID: 5313 RVA: 0x00065348 File Offset: 0x00063548
	private void OnLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success)
	{
		if (!success)
		{
			SpatialAnchorLoader.Log(string.Format("{0} Localization failed!", unboundAnchor));
			return;
		}
		Pose pose = unboundAnchor.Pose;
		OVRSpatialAnchor ovrspatialAnchor = Object.Instantiate<OVRSpatialAnchor>(this._anchorPrefab, pose.position, pose.rotation);
		unboundAnchor.BindTo(ovrspatialAnchor);
		Anchor anchor;
		if (ovrspatialAnchor.TryGetComponent<Anchor>(out anchor))
		{
			anchor.ShowSaveIcon = true;
		}
	}

	// Token: 0x060014C2 RID: 5314 RVA: 0x000653A7 File Offset: 0x000635A7
	private static void Log(string message)
	{
		Debug.Log("[SpatialAnchorsUnity]: " + message);
	}

	// Token: 0x04001713 RID: 5907
	[SerializeField]
	private OVRSpatialAnchor _anchorPrefab;

	// Token: 0x04001714 RID: 5908
	private Action<OVRSpatialAnchor.UnboundAnchor, bool> _onLoadAnchor;
}
