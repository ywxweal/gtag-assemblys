using System;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering;

// Token: 0x02000360 RID: 864
public class SceneManagerHelper
{
	// Token: 0x17000241 RID: 577
	// (get) Token: 0x06001428 RID: 5160 RVA: 0x0006240C File Offset: 0x0006060C
	public GameObject AnchorGameObject { get; }

	// Token: 0x06001429 RID: 5161 RVA: 0x00062414 File Offset: 0x00060614
	public SceneManagerHelper(GameObject gameObject)
	{
		this.AnchorGameObject = gameObject;
	}

	// Token: 0x0600142A RID: 5162 RVA: 0x00062424 File Offset: 0x00060624
	public void SetLocation(OVRLocatable locatable, Camera camera = null)
	{
		OVRLocatable.TrackingSpacePose trackingSpacePose;
		if (!locatable.TryGetSceneAnchorPose(out trackingSpacePose))
		{
			return;
		}
		Camera camera2 = ((camera == null) ? Camera.main : camera);
		Vector3? vector = trackingSpacePose.ComputeWorldPosition(camera2);
		Quaternion? quaternion = trackingSpacePose.ComputeWorldRotation(camera2);
		if (vector != null && quaternion != null)
		{
			this.AnchorGameObject.transform.SetPositionAndRotation(vector.Value, quaternion.Value);
		}
	}

	// Token: 0x0600142B RID: 5163 RVA: 0x00062494 File Offset: 0x00060694
	public void CreatePlane(OVRBounded2D bounds)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject.name = "Plane";
		gameObject.transform.SetParent(this.AnchorGameObject.transform, false);
		gameObject.transform.localScale = new Vector3(bounds.BoundingBox.size.x, bounds.BoundingBox.size.y, 0.01f);
		gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
	}

	// Token: 0x0600142C RID: 5164 RVA: 0x00062520 File Offset: 0x00060720
	public void UpdatePlane(OVRBounded2D bounds)
	{
		Transform transform = this.AnchorGameObject.transform.Find("Plane");
		if (transform == null)
		{
			this.CreatePlane(bounds);
			return;
		}
		transform.transform.localScale = new Vector3(bounds.BoundingBox.size.x, bounds.BoundingBox.size.y, 0.01f);
	}

	// Token: 0x0600142D RID: 5165 RVA: 0x00062594 File Offset: 0x00060794
	public void CreateVolume(OVRBounded3D bounds)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject.name = "Volume";
		gameObject.transform.SetParent(this.AnchorGameObject.transform, false);
		gameObject.transform.localPosition = new Vector3(0f, 0f, -bounds.BoundingBox.size.z / 2f);
		gameObject.transform.localScale = bounds.BoundingBox.size;
		gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
	}

	// Token: 0x0600142E RID: 5166 RVA: 0x00062634 File Offset: 0x00060834
	public void UpdateVolume(OVRBounded3D bounds)
	{
		Transform transform = this.AnchorGameObject.transform.Find("Volume");
		if (transform == null)
		{
			this.CreateVolume(bounds);
			return;
		}
		transform.transform.localPosition = new Vector3(0f, 0f, -bounds.BoundingBox.size.z / 2f);
		transform.transform.localScale = bounds.BoundingBox.size;
	}

	// Token: 0x0600142F RID: 5167 RVA: 0x000626B8 File Offset: 0x000608B8
	public void CreateMesh(OVRTriangleMesh mesh)
	{
		int num;
		int num2;
		if (!mesh.TryGetCounts(out num, out num2))
		{
			return;
		}
		using (NativeArray<Vector3> nativeArray = new NativeArray<Vector3>(num, Allocator.Temp, NativeArrayOptions.ClearMemory))
		{
			using (NativeArray<int> nativeArray2 = new NativeArray<int>(num2 * 3, Allocator.Temp, NativeArrayOptions.ClearMemory))
			{
				if (mesh.TryGetMesh(nativeArray, nativeArray2))
				{
					Mesh mesh2 = new Mesh();
					mesh2.indexFormat = IndexFormat.UInt32;
					mesh2.SetVertices<Vector3>(nativeArray);
					mesh2.SetTriangles(nativeArray2.ToArray(), 0);
					GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
					gameObject.name = "Mesh";
					gameObject.transform.SetParent(this.AnchorGameObject.transform, false);
					gameObject.GetComponent<MeshFilter>().sharedMesh = mesh2;
					gameObject.GetComponent<MeshCollider>().sharedMesh = mesh2;
					gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
				}
			}
		}
	}

	// Token: 0x06001430 RID: 5168 RVA: 0x000627B4 File Offset: 0x000609B4
	public void UpdateMesh(OVRTriangleMesh mesh)
	{
		Transform transform = this.AnchorGameObject.transform.Find("Mesh");
		if (transform != null)
		{
			Object.Destroy(transform);
		}
		this.CreateMesh(mesh);
	}

	// Token: 0x06001431 RID: 5169 RVA: 0x000627F0 File Offset: 0x000609F0
	public static async Task<bool> RequestSceneCapture()
	{
		bool flag;
		if (SceneManagerHelper.SceneCaptureRunning)
		{
			flag = false;
		}
		else
		{
			SceneManagerHelper.SceneCaptureRunning = true;
			bool waiting = true;
			Action<ulong, bool> onCaptured = delegate(ulong id, bool success)
			{
				waiting = false;
			};
			flag = await Task.Run<bool>(delegate
			{
				OVRManager.SceneCaptureComplete += onCaptured;
				ulong num;
				if (!OVRPlugin.RequestSceneCapture("", out num))
				{
					OVRManager.SceneCaptureComplete -= onCaptured;
					SceneManagerHelper.SceneCaptureRunning = false;
					return false;
				}
				while (waiting)
				{
					Task.Delay(200);
				}
				OVRManager.SceneCaptureComplete -= onCaptured;
				SceneManagerHelper.SceneCaptureRunning = false;
				return true;
			});
		}
		return flag;
	}

	// Token: 0x06001432 RID: 5170 RVA: 0x0006282B File Offset: 0x00060A2B
	public static void RequestScenePermission()
	{
		if (!Permission.HasUserAuthorizedPermission("com.oculus.permission.USE_SCENE"))
		{
			Permission.RequestUserPermission("com.oculus.permission.USE_SCENE");
		}
	}

	// Token: 0x04001689 RID: 5769
	private static bool SceneCaptureRunning;
}
