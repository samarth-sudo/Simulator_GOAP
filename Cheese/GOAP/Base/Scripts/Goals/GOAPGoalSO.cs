using UnityEngine;

namespace Cheese.GOAP
{
    public class GOAPGoalSO : ScriptableObject, IGOAPGoal
    {
        public GOAPGoalSO[] subGoals;

        public virtual bool GetIsGoal(GOAPWorldState state)
        {
            return false;
        }

        public virtual float GetValue(GOAPWorldState state)
        {
            return SumSubGoals(state);
        }

        protected float SumSubGoals(GOAPWorldState state) 
        {
            float value = 0;
            foreach (GOAPGoalSO subGoal in subGoals)
            {
                value += subGoal.GetValue(state);
            }
            return value;
        }
    }
}
