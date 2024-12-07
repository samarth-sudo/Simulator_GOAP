using System.Collections.Generic;

namespace Cheese.GOAP
{
    public enum GOAPRequestStatus
    {
        None,
        Waiting,
        Generating,
        Complete,
        Partial,
        Failed,
        InvalidStart
    }

    public class GOAPPathRequest
    {
        public GOAPPathRequest(
            IGOAPAgent agent,
            IGOAPGoal goal,
            float simStepTime)
        {
            this.agent = agent;
            this.goal = goal;
            this.simStepTime = simStepTime;
        }

        public IGOAPAgent agent;
        public IGOAPGoal goal;
        public GOAPRequestStatus status = GOAPRequestStatus.Waiting;

        public float simStepTime;

        public List<GOAPWorldState> path;
    }
}