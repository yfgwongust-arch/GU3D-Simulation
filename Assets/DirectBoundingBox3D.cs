using UnityEngine;

public class DirectBoundingBox3D : MonoBehaviour
{
    [Header("Filtering")]
    [Tooltip("Select which layers should display bounding boxes.")]
    public LayerMask targetLayers = ~0;

    void OnDrawGizmos()
    {
        Matrix4x4 originalMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.green;

        MeshRenderer[] renderers = FindObjectsOfType<MeshRenderer>();

        foreach (var rend in renderers)
        {
            if (!rend.enabled || !rend.gameObject.activeInHierarchy) continue;
            if (rend.gameObject == gameObject) continue;
            if (rend.gameObject.hideFlags != HideFlags.None) continue;
            if (((1 << rend.gameObject.layer) & targetLayers.value) == 0) continue;

            // 1. If object was statically batched at runtime, draw its pre-transformed world bounds directly
            if (rend.isPartOfStaticBatch)
            {
                Gizmos.DrawWireCube(rend.bounds.center, rend.bounds.size);
                continue;
            }

            // 2. Otherwise handle unbatched meshes using local bounds corner transformation
            MeshFilter mf = rend.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) continue;

            DrawWorldOrientedBox(mf.sharedMesh.bounds, rend.transform);
        }

        Gizmos.matrix = originalMatrix;
    }

    void DrawWorldOrientedBox(Bounds localBounds, Transform targetTransform)
    {
        Vector3 c = localBounds.center;
        Vector3 e = localBounds.extents;

        Vector3[] localCorners = new Vector3[8]
        {
            c + new Vector3(-e.x, -e.y, -e.z),
            c + new Vector3( e.x, -e.y, -e.z),
            c + new Vector3( e.x, -e.y,  e.z),
            c + new Vector3(-e.x, -e.y,  e.z),
            c + new Vector3(-e.x,  e.y, -e.z),
            c + new Vector3( e.x,  e.y, -e.z),
            c + new Vector3( e.x,  e.y,  e.z),
            c + new Vector3(-e.x,  e.y,  e.z)
        };

        Vector3[] w = new Vector3[8];
        for (int i = 0; i < 8; i++)
        {
            w[i] = targetTransform.TransformPoint(localCorners[i]);
        }

        // Bottom Face
        Gizmos.DrawLine(w[0], w[1]);
        Gizmos.DrawLine(w[1], w[2]);
        Gizmos.DrawLine(w[2], w[3]);
        Gizmos.DrawLine(w[3], w[0]);

        // Top Face
        Gizmos.DrawLine(w[4], w[5]);
        Gizmos.DrawLine(w[5], w[6]);
        Gizmos.DrawLine(w[6], w[7]);
        Gizmos.DrawLine(w[7], w[4]);

        // Side Pillars
        Gizmos.DrawLine(w[0], w[4]);
        Gizmos.DrawLine(w[1], w[5]);
        Gizmos.DrawLine(w[2], w[6]);
        Gizmos.DrawLine(w[3], w[7]);
    }
}