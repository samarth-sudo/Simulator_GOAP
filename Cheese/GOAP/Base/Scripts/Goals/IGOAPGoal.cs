namespace Cheese.GOAP
{
    public interface IGOAPGoal
    {

        /// <summary>
        /// Returns the value of a goal.
        /// Higher values are closer to the goal,
        /// the planning algorithm will search for higher values.
        /// </summary>
        /// <param name="state">The state to get the value of</param>
        /// <returns>The value of this state</returns>
        float GetValue(GOAPWorldState state);

        /// <summary>
        /// Finds out if a state has reached a goal.
        /// This is used by the planning algorithm to
        /// determine if it can stop searching.
        /// </summary>
        /// <param name="state">The state to check</param>
        /// <returns>Is this state at a goal?</returns>
        bool GetIsGoal(GOAPWorldState state);
    }
}
