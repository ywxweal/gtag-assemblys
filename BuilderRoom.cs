using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200050E RID: 1294
public class BuilderRoom : MonoBehaviour
{
	// Token: 0x0400232E RID: 9006
	public List<GameObject> disableColliderRoots;

	// Token: 0x0400232F RID: 9007
	public List<GameObject> disableRenderRoots;

	// Token: 0x04002330 RID: 9008
	public List<GameObject> disableGameObjectsForScene;

	// Token: 0x04002331 RID: 9009
	public List<GameObject> disableObjectsForPersistent;

	// Token: 0x04002332 RID: 9010
	public List<MeshRenderer> disabledRenderersForPersistent;

	// Token: 0x04002333 RID: 9011
	public List<Collider> disabledCollidersForScene;
}
