using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200036C RID: 876
[RequireComponent(typeof(OVRSceneAnchor))]
[DefaultExecutionOrder(30)]
public class FurnitureSpawner : MonoBehaviour
{
	// Token: 0x06001454 RID: 5204 RVA: 0x000634A3 File Offset: 0x000616A3
	private void Start()
	{
		this._sceneAnchor = base.GetComponent<OVRSceneAnchor>();
		this._classification = base.GetComponent<OVRSemanticClassification>();
		this.AddRoomLight();
		this.SpawnSpawnable();
	}

	// Token: 0x06001455 RID: 5205 RVA: 0x000634CC File Offset: 0x000616CC
	private void SpawnSpawnable()
	{
		SimpleResizable simpleResizable;
		if (!this.FindValidSpawnable(out simpleResizable))
		{
			return;
		}
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		Vector3 localScale = base.transform.localScale;
		OVRScenePlane component = this._sceneAnchor.GetComponent<OVRScenePlane>();
		OVRSceneVolume component2 = this._sceneAnchor.GetComponent<OVRSceneVolume>();
		Vector3 vector = (component2 ? component2.Dimensions : Vector3.one);
		if (this._classification && component)
		{
			vector = component.Dimensions;
			vector.z = 1f;
			if (this._classification.Contains("TABLE") || this._classification.Contains("COUCH"))
			{
				this.GetVolumeFromTopPlane(base.transform, component.Dimensions, base.transform.position.y, out position, out rotation, out localScale);
				vector = localScale;
				position.y += localScale.y / 2f;
			}
			if (this._classification.Contains("WALL_FACE") || this._classification.Contains("CEILING") || this._classification.Contains("FLOOR"))
			{
				vector.z = 0.01f;
			}
		}
		GameObject gameObject = new GameObject("Root");
		gameObject.transform.parent = base.transform;
		gameObject.transform.SetPositionAndRotation(position, rotation);
		new SimpleResizer().CreateResizedObject(vector, gameObject, simpleResizable);
	}

	// Token: 0x06001456 RID: 5206 RVA: 0x00063658 File Offset: 0x00061858
	private bool FindValidSpawnable(out SimpleResizable currentSpawnable)
	{
		currentSpawnable = null;
		if (!this._classification)
		{
			return false;
		}
		if (!Object.FindObjectOfType<OVRSceneManager>())
		{
			return false;
		}
		foreach (Spawnable spawnable in this.SpawnablePrefabs)
		{
			if (this._classification.Contains(spawnable.ClassificationLabel))
			{
				currentSpawnable = spawnable.ResizablePrefab;
				return true;
			}
		}
		if (this.FallbackPrefab != null)
		{
			currentSpawnable = this.FallbackPrefab;
			return true;
		}
		return false;
	}

	// Token: 0x06001457 RID: 5207 RVA: 0x00063700 File Offset: 0x00061900
	private void AddRoomLight()
	{
		if (!this.RoomLightPrefab)
		{
			return;
		}
		if (this._classification && this._classification.Contains("CEILING") && !FurnitureSpawner._roomLightRef)
		{
			FurnitureSpawner._roomLightRef = Object.Instantiate<GameObject>(this.RoomLightPrefab, this._sceneAnchor.transform);
		}
	}

	// Token: 0x06001458 RID: 5208 RVA: 0x00063764 File Offset: 0x00061964
	private void GetVolumeFromTopPlane(Transform plane, Vector2 dimensions, float height, out Vector3 position, out Quaternion rotation, out Vector3 localScale)
	{
		float num = height / 2f;
		position = plane.position - Vector3.up * num;
		rotation = Quaternion.LookRotation(-plane.up, Vector3.up);
		localScale = new Vector3(dimensions.x, num * 2f, dimensions.y);
	}

	// Token: 0x040016B7 RID: 5815
	[Tooltip("Add a point at ceiling.")]
	public GameObject RoomLightPrefab;

	// Token: 0x040016B8 RID: 5816
	[Tooltip("This prefab will be used if the label is not in the SpawnablesPrefabs")]
	public SimpleResizable FallbackPrefab;

	// Token: 0x040016B9 RID: 5817
	public List<Spawnable> SpawnablePrefabs;

	// Token: 0x040016BA RID: 5818
	private OVRSceneAnchor _sceneAnchor;

	// Token: 0x040016BB RID: 5819
	private OVRSemanticClassification _classification;

	// Token: 0x040016BC RID: 5820
	private static GameObject _roomLightRef;

	// Token: 0x040016BD RID: 5821
	private int _frameCounter;
}
