using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cheese.GOAP
{
    public class GOAPPathing : MonoBehaviour
    {
        public static GOAPPathing instance;

        private int searchCount; // how many states we have searched so far
        [Tooltip("Maximum number of states to seach into the future.")]
        public int maxSearchDepth = 100;
        [Tooltip("Maximum number of states to seach per frame")]
        public int maxSearchesPerFrame = 10;

        private GOAPWorldState rootState; // the starting state we are searching from

        private List<GOAPWorldState> openStates; // states we can still search
        private List<GOAPWorldState> closedStates; // states that we have already searched

        private List<GOAPPathRequest> requests;
        private GOAPPathRequest currentRequest;

        [Header("Debug settings")]
        public bool drawGizmos;

        [Tooltip("Draw the open states (states that we can serach)")]
        public bool drawOpenNodes;
        [Tooltip("Colour to draw the open states (states that we can serach)")]
        public Color openNodesColour = Color.blue;

        [Tooltip("Draw the close states (states that we already serached)")]
        public bool drawClosedNodes;
        [Tooltip("Colour to draw the close states (states that we already serached)")]
        public Color closedNodesColour = Color.cyan;

        [Tooltip("Draw the best path we have found so far")]
        public bool drawPath;
        [Tooltip("Colour to draw the best path we have found so far")]
        public Color pathColour = Color.yellow;

        // various info about the pathfinder its nice to know
        public int totalPathsGenerated { get; private set; }
        private float generationStartTime;
        public float generationLength { get; private set; }

        private void Awake()
        {
            instance = this;
            requests = new List<GOAPPathRequest>();
        }

        /// <summary>
        /// Requests the GOAP algorithm to generate a path.
        /// Make sure to check the request status to see when its comepleted.
        /// </summary>
        /// <param name="newRequest">Information about the path request</param>
        public void RequestPath(GOAPPathRequest newRequest)
        {
            if (newRequest == null)
            {
                Debug.LogError("Request is null!");
                return;
            }
            if (requests.Any(r => r.agent == newRequest.agent))
            {
                Debug.LogError("Agent has already requested a path! Wait for the existing request to be processed!");
                return;
            }
            requests.Add(newRequest);
        }

        /// <summary>
        /// Starts solving a GOAP request
        /// </summary>
        private void StartGoap()
        {
            currentRequest = requests[0];
            requests.Remove(currentRequest);

            generationStartTime = Time.unscaledTime;

            if (currentRequest.agent != null && currentRequest.agent.IsAlive())
            {
                currentRequest.status = GOAPRequestStatus.Generating;
                currentRequest.path = null;

                searchCount = 0;

                rootState = currentRequest.agent.GetCurrentState();
                rootState.request = currentRequest;
                rootState.UpdateValue();

                if (!rootState.IsValidStartPoint())
                {
                    StopGoap(GOAPRequestStatus.InvalidStart);
                }

                openStates = new List<GOAPWorldState>();
                closedStates = new List<GOAPWorldState>();
                openStates.Add(rootState);
            }
            else
            {
                StopGoap(GOAPRequestStatus.Failed);
            }
        }

        /// <summary>
        /// Finishes processing a GOAP request
        /// </summary>
        /// <param name="result">The result of the GOAP request</param>
        private void StopGoap(GOAPRequestStatus result)
        {
            currentRequest.path = new List<GOAPWorldState>();

            // fin the best node, and follow it backwards to find the sequences of states that lead to the best state
            GOAPWorldState bestNode = GetHighestValueOpenState();

            while (bestNode != null)
            {
                currentRequest.path.Add(bestNode);
                bestNode = bestNode.parentState;
            }

            // reverse the list to get a list of states that lead to the best state
            currentRequest.path.Reverse();

            currentRequest.status = result;
            currentRequest = null;

            totalPathsGenerated++;
            generationLength = Time.unscaledTime - generationStartTime;
        }

        private void Update()
        {
            // try to begin a new path
            if (currentRequest == null && requests.Count > 0)
            {
                StartGoap();
            }

            //update existing path
            if (currentRequest != null)
            {
                for (int i = 0; i < maxSearchesPerFrame; i++)
                {
                    if (searchCount < maxSearchDepth)
                    {
                        if (openStates.Count > 0)
                        {
                            GOAPWorldState bestNode = GetHighestValueOpenState();

                            openStates.Remove(bestNode);
                            closedStates.Add(bestNode);

                            if (bestNode.ExploreFutureStates(ref openStates, currentRequest.simStepTime))
                            {
                                StopGoap(GOAPRequestStatus.Complete);
                                break;
                            }
                            searchCount++;
                        }
                        else
                        {
                            // we have no more option to search!
                            // we have probably run into a dead end,
                            // and every action will result in our death...
                            // agents should either panic or follow their old path in this situation
                            StopGoap(GOAPRequestStatus.Failed);
                            break;
                        }
                    }
                    else
                    {
                        StopGoap(GOAPRequestStatus.Partial);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the highest value open state
        /// This is used to decide which states to search next
        /// </summary>
        /// <returns>The state with the highest value</returns>
        private GOAPWorldState GetHighestValueOpenState()
        {
            if (openStates == null)
            {
                return null;
            }

            GOAPWorldState highestValueNode = null;
            float highestValue = Mathf.NegativeInfinity;
            foreach (GOAPWorldState node in openStates)
            {
                if (node.value > highestValue)
                {
                    highestValue = node.value;
                    highestValueNode = node;
                }
            }
            return highestValueNode;
        }

        private void OnDrawGizmos()
        {
            // gizmos don't apear to be very cheap to draw
            // and we need a lot of them to draw all the places we've searched
            // so you can turn them all on and off individually
            if (!drawGizmos)
                return;

            if (drawOpenNodes && openStates != null)
            {
                Gizmos.color = openNodesColour;
                foreach (GOAPWorldState state in openStates)
                {
                    state.DrawStateDebugGizmo();
                }
            }

            if (drawClosedNodes && closedStates != null)
            {
                Gizmos.color = closedNodesColour;
                foreach (GOAPWorldState state in closedStates)
                {
                    state.DrawStateDebugGizmo();
                }
            }

            if (drawPath)
            {
                Gizmos.color = pathColour;
                GOAPWorldState bestNode = GetHighestValueOpenState();

                if (bestNode != null)
                {
                    bestNode.StatePathDebugGizmo();
                }
            }
        }

        /// <summary>
        /// Gets the number of requests in the queue
        /// </summary>
        /// <returns>The number of queued requests</returns>
        public int GetQueuedRequestAmmount()
        {
            return requests.Count;
        }

        /// <summary>
        /// Returns all the open states (states that can still be searched).
        /// Do not use this for anything other than debugging!
        /// </summary>
        /// <returns>the list of open states</returns>
        public List<GOAPWorldState> DebugGetOpenStates()
        {
            return openStates;
        }

        /// <summary>
        /// Returns all the close states (states that have been searched).
        /// Do not use this for anything other than debugging!
        /// </summary>
        /// <returns>the list of close states</returns>
        public List<GOAPWorldState> DebugGetClosedStates()
        {
            return closedStates;
        }

        /// <summary>
        /// Returns all the path requests.
        /// Do not use this for anything other than debugging!
        /// </summary>
        /// <returns>the list of path request</returns>
        public List<GOAPPathRequest> DebugGetPathRequests()
        {
            return requests;
        }
    }
}