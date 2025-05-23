using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000377 RID: 887
[RequireComponent(typeof(OVRSceneAnchor))]
public class VolumeAndPlaneSwitcher : MonoBehaviour
{
	// Token: 0x06001486 RID: 5254 RVA: 0x0006447C File Offset: 0x0006267C
	private void ReplaceAnchor(OVRSceneAnchor prefab, Vector3 position, Quaternion rotation, Vector3 localScale)
	{
		OVRSceneAnchor ovrsceneAnchor = Object.Instantiate<OVRSceneAnchor>(prefab, base.transform.parent);
		ovrsceneAnchor.enabled = false;
		ovrsceneAnchor.InitializeFrom(base.GetComponent<OVRSceneAnchor>());
		ovrsceneAnchor.transform.SetPositionAndRotation(position, rotation);
		foreach (object obj in ovrsceneAnchor.transform)
		{
			((Transform)obj).localScale = localScale;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06001487 RID: 5255 RVA: 0x00064510 File Offset: 0x00062710
	private void Start()
	{
		OVRSemanticClassification component = base.GetComponent<OVRSemanticClassification>();
		if (!component)
		{
			return;
		}
		foreach (VolumeAndPlaneSwitcher.LabelGeometryPair labelGeometryPair in this.desiredSwitches)
		{
			if (component.Contains(labelGeometryPair.label))
			{
				Vector3 zero = Vector3.zero;
				Quaternion identity = Quaternion.identity;
				Vector3 zero2 = Vector3.zero;
				VolumeAndPlaneSwitcher.GeometryType desiredGeometryType = labelGeometryPair.desiredGeometryType;
				if (desiredGeometryType != VolumeAndPlaneSwitcher.GeometryType.Plane)
				{
					if (desiredGeometryType == VolumeAndPlaneSwitcher.GeometryType.Volume)
					{
						OVRScenePlane component2 = base.GetComponent<OVRScenePlane>();
						if (!component2)
						{
							Debug.LogWarning("Ignoring desired plane to volume switch for " + labelGeometryPair.label + " because it is not a plane.");
						}
						else
						{
							Debug.Log(string.Format("IN Plane Position {0}, Dimensions: {1}", base.transform.position, component2.Dimensions));
							this.GetVolumeFromTopPlane(base.transform, component2.Dimensions, base.transform.position.y, out zero, out identity, out zero2);
							Debug.Log(string.Format("OUT Volume Position {0}, Dimensions: {1}", zero, zero2));
							this.ReplaceAnchor(this.volumePrefab, zero, identity, zero2);
						}
					}
				}
				else
				{
					OVRSceneVolume component3 = base.GetComponent<OVRSceneVolume>();
					if (!component3)
					{
						Debug.LogWarning("Ignoring desired volume to plane switch for " + labelGeometryPair.label + " because it is not a volume.");
					}
					else
					{
						Debug.Log(string.Format("IN Volume Position {0}, Dimensions: {1}", base.transform.position, component3.Dimensions));
						this.GetTopPlaneFromVolume(base.transform, component3.Dimensions, out zero, out identity, out zero2);
						Debug.Log(string.Format("OUT Plane Position {0}, Dimensions: {1}", zero, zero2));
						this.ReplaceAnchor(this.planePrefab, zero, identity, zero2);
					}
				}
			}
		}
		Object.Destroy(this);
	}

	// Token: 0x06001488 RID: 5256 RVA: 0x0006471C File Offset: 0x0006291C
	private void GetVolumeFromTopPlane(Transform plane, Vector2 dimensions, float height, out Vector3 position, out Quaternion rotation, out Vector3 localScale)
	{
		position = plane.position;
		rotation = plane.rotation;
		localScale = new Vector3(dimensions.x, dimensions.y, height);
	}

	// Token: 0x06001489 RID: 5257 RVA: 0x00064754 File Offset: 0x00062954
	private void GetTopPlaneFromVolume(Transform volume, Vector3 dimensions, out Vector3 position, out Quaternion rotation, out Vector3 localScale)
	{
		float num = dimensions.y / 2f;
		position = volume.position + Vector3.up * num;
		rotation = Quaternion.LookRotation(Vector3.up, -volume.forward);
		localScale = new Vector3(dimensions.x, dimensions.z, dimensions.y);
	}

	// Token: 0x040016E0 RID: 5856
	public OVRSceneAnchor planePrefab;

	// Token: 0x040016E1 RID: 5857
	public OVRSceneAnchor volumePrefab;

	// Token: 0x040016E2 RID: 5858
	public List<VolumeAndPlaneSwitcher.LabelGeometryPair> desiredSwitches;

	// Token: 0x02000378 RID: 888
	public enum GeometryType
	{
		// Token: 0x040016E4 RID: 5860
		Plane,
		// Token: 0x040016E5 RID: 5861
		Volume
	}

	// Token: 0x02000379 RID: 889
	[Serializable]
	public struct LabelGeometryPair
	{
		// Token: 0x040016E6 RID: 5862
		public string label;

		// Token: 0x040016E7 RID: 5863
		public VolumeAndPlaneSwitcher.GeometryType desiredGeometryType;
	}
}
