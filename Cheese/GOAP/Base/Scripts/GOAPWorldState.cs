using System.Collections.Generic;

namespace Cheese.GOAP
{
    public class GOAPWorldState
    {
        public GOAPPathRequest request;
        public GOAPWorldState parentState;

        public float timeSinceBegining;
        public bool dead;

        public float value;

        public GOAPWorldState()
        {

        }

        public GOAPWorldState(GOAPWorldState stateToCopy)
        {
            parentState = stateToCopy;
            request = stateToCopy.request;

            timeSinceBegining = stateToCopy.timeSinceBegining;
            dead = stateToCopy.dead;
        }

        public virtual void UpdateValue()
        {
            value = request.goal.GetValue(this);
        }

        public virtual bool IsValidStartPoint()
        {
            return true;
        }

        public virtual bool ExploreFutureStates(ref List<GOAPWorldState> openNodes, float simStepTime)
        {
            return false;
        }

        public virtual void DrawStateDebugGizmo()
        {

        }

        public virtual void StatePathDebugGizmo()
        {

        }
    }
}