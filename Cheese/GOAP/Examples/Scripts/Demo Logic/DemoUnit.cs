using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class DemoUnit : MonoBehaviour
    {
        public string unitName;
        [TextArea]
        public string unitDescription;

        public Transform visualCenter;
        public Transform fpv;

        private DemoSequence currentSequence;

        public DemoSequence[] sequences;

        public Vector3 waypointPos;
        public Vector3 waypointNormal;

        public Vector3 spawnPoint;
        public Quaternion spawnRotation;

        private void Awake()
        {
            for (int i = 0; i < sequences.Length; i++)
            {
                sequences[i] = Instantiate(sequences[i]);
                sequences[i].Innit();
            }
        }

        protected virtual void Start()
        {
            DemoManager.instance.RegisterUnit(this);
        }

        public virtual void Respawn()
        {
            Debug.LogError("Not implemented, please override this function!");
        }

        public void StartSequence(DemoSequence newSequence)
        {
            currentSequence = newSequence;
            currentSequence.Setup(this);
            currentSequence.StartSequence();
        }

        private void Update()
        {
            UpdateSequence();
        }

        public void UpdateSequence()
        {
            if (currentSequence == null)
                return;

            DemoSequence.SequenceStatus status = currentSequence.GetSequenceStatus();
            switch (status)
            {
                case DemoSequence.SequenceStatus.Running:
                    currentSequence.UpdateSequence();
                    break;
                case DemoSequence.SequenceStatus.None:
                case DemoSequence.SequenceStatus.Completed:
                case DemoSequence.SequenceStatus.Failed:
                    currentSequence = null;
                    break;
            }
        }

        public DemoSequence DebugGetCurrentSequence()
        {
            return currentSequence;
        }

        public virtual void GetPYRT(out float pitch, out float yaw, out float roll, out float throttle)
        {
            pitch = 0;
            yaw = 0;
            roll = 0;
            throttle = 0;
        }
    }
}