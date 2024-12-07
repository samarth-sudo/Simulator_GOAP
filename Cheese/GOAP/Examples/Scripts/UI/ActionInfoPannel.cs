using UnityEngine;
using UnityEngine.UI;

namespace Cheese.GOAP.Demo
{
    public class ActionInfoPannel : MonoBehaviour
    {
        public Text sequenceNameText;
        public Text sequenceDescriptionText;

        public Text actionNameText;

        private int sequenceID;

        private void Update()
        {
            string temp = "Awaiting orders.";

            DemoSequence demoSequence = DemoManager.instance.demoUnit?.DebugGetCurrentSequence();
            if (demoSequence != null)
            {
                switch (demoSequence.GetSequenceStatus())
                {
                    case DemoSequence.SequenceStatus.None:
                    case DemoSequence.SequenceStatus.Completed:
                        break;
                    case DemoSequence.SequenceStatus.Running:
                        temp = demoSequence.actions[demoSequence.currentActionId].name;
                        break;
                    case DemoSequence.SequenceStatus.Failed:
                        temp = $"Failed sequence: {demoSequence.seqeuenceName}";
                        break;
                }
            }

            actionNameText.text = temp;
        }

        public void UpdateUnitInfo()
        {
            sequenceID = 0;
            UpdateSequenceInfo();
        }

        private void UpdateSequenceInfo()
        {
            if (DemoManager.instance.demoUnit == null)
                return;

            sequenceNameText.text = DemoManager.instance.demoUnit.sequences[sequenceID].seqeuenceName;
            sequenceDescriptionText.text = DemoManager.instance.demoUnit.sequences[sequenceID].sequenceDescription;
        }

        public void NextSequence()
        {
            SetSequence(sequenceID + 1);
        }

        public void PreviousSequence()
        {
            SetSequence(sequenceID - 1);
        }

        private void SetSequence(int sequenceID)
        {
            if (sequenceID >= DemoManager.instance.demoUnit.sequences.Length)
            {
                sequenceID = 0;
            }
            if (sequenceID < 0)
            {
                sequenceID = DemoManager.instance.demoUnit.sequences.Length - 1;
            }

            this.sequenceID = sequenceID;
            UpdateSequenceInfo();
        }

        public void BeginSequence()
        {
            DemoManager.instance.demoUnit.StartSequence(DemoManager.instance.demoUnit.sequences[sequenceID]);
        }
    }
}