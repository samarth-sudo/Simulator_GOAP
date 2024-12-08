using System.Collections.Generic;
using UnityEngine;

namespace Cheese.GOAP
{
    public class GOAPPathing : MonoBehaviour
    {
        public static GOAPPathing instance;

        private List<GOAPWorldState> openStates; // States we can still search
        private List<GOAPWorldState> closedStates; // States we have already searched
        private List<GOAPPathRequest> requests = new List<GOAPPathRequest>(); // List of pathfinding requests
        private GOAPPathRequest currentRequest;

        public int maxSearchesPerFrame = 10; // Maximum number of states to search per frame
        public int maxSearchDepth = 100; // Maximum number of states to search
        public int totalPathsGenerated { get; private set; } // Number of total paths generated
        public float generationLength { get; private set; } // Time taken to generate the current path

        private void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// Requests the GOAP algorithm to generate a path.
        /// </summary>
        /// <param name="newRequest">The new pathfinding request</param>
        public void RequestPath(GOAPPathRequest newRequest)
        {
            if (newRequest == null)
            {
                Debug.LogError("Request is null!");
                return;
            }
            requests.Add(newRequest);
        }

        /// <summary>
        /// Gets the number of queued requests.
        /// </summary>
        /// <returns>The number of queued requests</returns>
        public int GetQueuedRequestAmmount()
        {
            return requests.Count;
        }

        /// <summary>
        /// Gets all path requests for debugging purposes.
        /// </summary>
        /// <returns>List of path requests</returns>
        public List<GOAPPathRequest> DebugGetPathRequests()
        {
            return requests;
        }

        /// <summary>
        /// Gets all closed states for debugging purposes.
        /// </summary>
        /// <returns>List of closed states</returns>
        public List<GOAPWorldState> DebugGetClosedStates()
        {
            return closedStates;
        }

        private void Start()
        {
            // Initialize the states
            openStates = new List<GOAPWorldState>();
            closedStates = new List<GOAPWorldState>();
        }

        private void Update()
        {
            // If there is a request to process and no current request is being handled
            if (requests.Count > 0 && currentRequest == null)
            {
                ProcessNextRequest();
            }
        }

        /// <summary>
        /// Processes the next request in the queue.
        /// </summary>
        private void ProcessNextRequest()
        {
            currentRequest = requests[0];
            requests.RemoveAt(0);

            Debug.Log($"Processing request for agent: {currentRequest.agent}");

            // Simulate path generation
            GeneratePath();

            // Mark the request as complete
            currentRequest.status = GOAPRequestStatus.Complete;
            totalPathsGenerated++;
        }

        /// <summary>
        /// Generates a path.
        /// </summary>
        private void GeneratePath()
        {
            openStates.Clear();
            closedStates.Clear();

            // Initialize the initial state
            GOAPWorldState initialState = currentRequest.agent.GetCurrentState();
            openStates.Add(initialState);

            // Simulate processing open states
            for (int i = 0; i < maxSearchDepth; i++)
            {
                if (openStates.Count == 0)
                    break;

                GOAPWorldState current = openStates[0];
                openStates.RemoveAt(0);
                closedStates.Add(current);

                // Example logic to expand neighbors (you can replace this with actual neighbor logic)
                foreach (var neighbor in current.GetNeighbors())
                {
                    if (!closedStates.Contains(neighbor) && !openStates.Contains(neighbor))
                    {
                        openStates.Add(neighbor);
                    }
                }
            }

            generationLength = Time.unscaledTime;
            Debug.Log($"Path generated in {generationLength} seconds.");
        }

        /// <summary>
        /// Debug visualization for the GOAP pathing process.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (openStates == null || closedStates == null)
                return;

            // Visualize open states
            Gizmos.color = Color.blue;
            foreach (var state in openStates)
            {
                Gizmos.DrawSphere(state.Position, 0.5f);
            }

            // Visualize closed states
            Gizmos.color = Color.red;
            foreach (var state in closedStates)
            {
                Gizmos.DrawCube(state.Position, Vector3.one);
            }
        }
    }
}
