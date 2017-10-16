using UnityEngine;

namespace DevicesSyncUnity.Examples.LeanTouchExamples
{
    public class LeanTouchSyncSubscriber : MonoBehaviour
    {
        // Editor fields

        [SerializeField]
        private LeanTouchSync leanTouchSync;

        // Properties

        public LeanTouchSync LeanTouchSync { get { return leanTouchSync; } set { leanTouchSync = value; } }
    }
}
