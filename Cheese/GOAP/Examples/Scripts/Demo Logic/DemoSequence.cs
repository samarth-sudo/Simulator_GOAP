using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [CreateAssetMenu(fileName = "Sequence", menuName = "Demo/DemoSequence")]
    public class DemoSequence : ScriptableObject
    {
        public enum SequenceStatus
        {
            None,
            Running,
            Completed,
            Failed
        }

        private SequenceStatus status;

        public DemoAction[] actions;
        public int currentActionId;

        public string seqeuenceName;
        [TextArea]
        public string sequenceDescription;

        public void Innit()
        {
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i] = Instantiate(actions[i]);
            }
        }

        public void Setup(DemoUnit unit)
        {
            foreach (DemoAction action in actions)
            {
                action.SetUp(unit);
            }
        }

        public void StartSequence()
        {
            status = SequenceStatus.Running;
            currentActionId = 0;
            actions[currentActionId].StartAction();
        }

        public void UpdateSequence()
        {
            DemoAction currentAction = actions[currentActionId];

            DemoSequence.SequenceStatus actionStatus = currentAction.GetSequenceStatus();
            switch (actionStatus)
            {
                case SequenceStatus.Running:
                    currentAction.UpdateAction();
                    break;
                case SequenceStatus.Completed:
                    Debug.Log($"{currentAction.name} is completed!");
                    currentActionId++;
                    if (currentActionId < actions.Length)
                    {
                        actions[currentActionId].StartAction();
                    }
                    else
                    {
                        status = SequenceStatus.Completed;
                    }
                    break;
                case SequenceStatus.None:
                case SequenceStatus.Failed:
                    Debug.Log($"{currentAction.name} failed! :(");
                    status = SequenceStatus.Failed;
                    break;
            }
        }

        public SequenceStatus GetSequenceStatus()
        {
            return status;
        }
    }
}