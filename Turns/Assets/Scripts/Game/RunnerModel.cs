    using UnityEngine;

    public class RunnerModel: MonoBehaviour
    {
        public Transform Holder;
        public GameObject idleEffectsHolder;
        public GameObject activeEffectsHolder;

        public void Reset()
        {
            idleEffectsHolder.SetActive(false);
            activeEffectsHolder.SetActive(false);
        }
        
        public void SetForMenu()
        {
            idleEffectsHolder.SetActive(true);
            activeEffectsHolder.SetActive(false);
        }

        public void SetForRun()
        {
            idleEffectsHolder.SetActive(true);
            activeEffectsHolder.SetActive(true);
        }
    }
