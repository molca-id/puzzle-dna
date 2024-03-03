using UnityEngine;
using UnityEngine.Scripting;

public class IncrementalGCController : MonoBehaviour
{
    private void Awake()
    {
        GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
    }

#if DISABLE_GARBAGE_COLLECTION
    private void Start()
    {
        Unity.Jobs.LowLevel.Unsafe.JobsUtility.JobWorkerCount = 1;
        Debug.Log("Incremental GC disabled.");
    }
#endif
}