using System;
using System.Threading.Tasks;
using UnityEngine;

namespace DynamicSceneManagerHelper
{
	// Token: 0x02000BC2 RID: 3010
	internal class UnityObjectUpdater
	{
		// Token: 0x06004A66 RID: 19046 RVA: 0x0016247C File Offset: 0x0016067C
		public async Task<GameObject> CreateUnityObject(OVRAnchor anchor, GameObject parent)
		{
			OVRRoomLayout ovrroomLayout;
			GameObject gameObject;
			OVRLocatable locatable;
			if (anchor.TryGetComponent<OVRRoomLayout>(out ovrroomLayout))
			{
				gameObject = new GameObject(string.Format("Room-{0}", anchor.Uuid));
			}
			else if (!anchor.TryGetComponent<OVRLocatable>(out locatable))
			{
				gameObject = null;
			}
			else
			{
				await locatable.SetEnabledAsync(true, 0.0);
				string text = "other";
				OVRSemanticLabels ovrsemanticLabels;
				if (anchor.TryGetComponent<OVRSemanticLabels>(out ovrsemanticLabels))
				{
					text = ovrsemanticLabels.Labels;
				}
				GameObject gameObject2 = new GameObject(text);
				if (parent != null)
				{
					gameObject2.transform.SetParent(parent.transform);
				}
				SceneManagerHelper sceneManagerHelper = new SceneManagerHelper(gameObject2);
				sceneManagerHelper.SetLocation(locatable, null);
				OVRBounded2D ovrbounded2D;
				if (anchor.TryGetComponent<OVRBounded2D>(out ovrbounded2D) && ovrbounded2D.IsEnabled)
				{
					sceneManagerHelper.CreatePlane(ovrbounded2D);
				}
				OVRBounded3D ovrbounded3D;
				if (anchor.TryGetComponent<OVRBounded3D>(out ovrbounded3D) && ovrbounded3D.IsEnabled)
				{
					sceneManagerHelper.CreateVolume(ovrbounded3D);
				}
				OVRTriangleMesh ovrtriangleMesh;
				if (anchor.TryGetComponent<OVRTriangleMesh>(out ovrtriangleMesh) && ovrtriangleMesh.IsEnabled)
				{
					sceneManagerHelper.CreateMesh(ovrtriangleMesh);
				}
				gameObject = gameObject2;
			}
			return gameObject;
		}

		// Token: 0x06004A67 RID: 19047 RVA: 0x001624C8 File Offset: 0x001606C8
		public void UpdateUnityObject(OVRAnchor anchor, GameObject gameObject)
		{
			SceneManagerHelper sceneManagerHelper = new SceneManagerHelper(gameObject);
			OVRLocatable ovrlocatable;
			if (anchor.TryGetComponent<OVRLocatable>(out ovrlocatable))
			{
				sceneManagerHelper.SetLocation(ovrlocatable, null);
			}
			OVRBounded2D ovrbounded2D;
			if (anchor.TryGetComponent<OVRBounded2D>(out ovrbounded2D) && ovrbounded2D.IsEnabled)
			{
				sceneManagerHelper.UpdatePlane(ovrbounded2D);
			}
			OVRBounded3D ovrbounded3D;
			if (anchor.TryGetComponent<OVRBounded3D>(out ovrbounded3D) && ovrbounded3D.IsEnabled)
			{
				sceneManagerHelper.UpdateVolume(ovrbounded3D);
			}
			OVRTriangleMesh ovrtriangleMesh;
			if (anchor.TryGetComponent<OVRTriangleMesh>(out ovrtriangleMesh) && ovrtriangleMesh.IsEnabled)
			{
				sceneManagerHelper.UpdateMesh(ovrtriangleMesh);
			}
		}
	}
}
