namespace Cheese.GOAP
{
    /// <summary>
    /// Interface for a GOAP agent to inherit from
    /// </summary>
    public interface IGOAPAgent
    {
        /// <summary>
        /// Override this to provide the world state of your agent.
        /// This will be used as the starting point of the GOAP algorithm.
        /// </summary>
        /// <returns>The starting state to be used by the GOAP algorithm</returns>
        GOAPWorldState GetCurrentState();

        /// <summary>
        /// Override to return if the agent are still alive.
        /// This will be used by the GOAP pathing to
        /// avoid wasting time on agents that have been killed
        /// since requesting a path.
        /// </summary>
        /// <returns>Is the agent still alive?</returns>
        bool IsAlive();
    }
}