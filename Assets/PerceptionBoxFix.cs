using System.Collections;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;

public class PerceptionBoxFix : MonoBehaviour
{
    IEnumerator Start()
    {
        // Give HDRP a couple of frames to fully boot up its render graph passes
        yield return null;
        yield return null;

        var perceptionCamera = GetComponent<PerceptionCamera>();
        if (perceptionCamera != null)
        {
            // Find the BoundingBox3DLabeler safely without breaking background threads
            foreach (var labeler in perceptionCamera.labelers)
            {
                if (labeler is UnityEngine.Perception.GroundTruth.Labelers.BoundingBox3DLabeler boxLabeler)
                {
                    boxLabeler.enabled = false;
                    yield return null;
                    boxLabeler.enabled = true;
                    break;
                }
            }
        }
    }
}