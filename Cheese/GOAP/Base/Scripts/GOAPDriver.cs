using System.Collections.Generic;
using UnityEngine;

namespace Cheese.GOAP
{
    /// <summary>
    /// A driver class for vehicles that mangages requesting and following paths
    /// Drivers for differnt vehicle types can inherit from this class to avoid having to implement
    /// the path following and requesting logic every time.
    /// It's not necessary to use this class, feel free to implement your own.
    /// </summary>
    public class GOAPDriver : MonoBehaviour, IGOAPAgent
    {
        /// <summary>
        /// Modes the driver can be in.
        /// </summary>
        public enum GOAPDriverMode
        {
            Disabled, // This is for when you want some other code to control the vehicle
            Stop, // Come to a complete stop
            GoToPos, // Normal navigation mode, uses the GOAP to navigate, stop at goal.
            GoToGoal, // Normal navigation mode, uses the GOAP to navigate.
            TerminalOnly, // Terminal navigation only, go directly to the goal, ignore obstacles
            Panic // Used for testing the panic behaviour
        }

        public enum DriverUpdateMode
        {
            Update,
            FixedUpdate
        }

        // information about the current state of the agent
        protected IGOAPGoal goal;
        public GOAPGoal_MoveTo defaultMoveGoal;

        protected GOAPPathRequest_Vehicle request;
        protected List<GOAPWorldState_Vehicle> path;
        protected bool pathValid;
        protected bool goalChanged;
        protected GOAPRequestStatus pathType;

        protected GOAPDriverMode mode;

        public Transform tf;

        [Tooltip("Used fixed update for physics based agents.")]
        public DriverUpdateMode updateMode = DriverUpdateMode.FixedUpdate;

        private int waypoint;

        [Tooltip("How far the agent can get from the path before we should generate a new one (meters)")]
        public float maxPathDeviation = 20f;
        [Tooltip("How long to wait since we last generated a path to avoid spamming requests")]
        public float generationCooldown = 1f;
        private float cooldownTimer; // float used for the generation cooldown timer

        [Tooltip("The target position to navigate to")]
        public Vector3 target;

        [Tooltip("How far to look ahead when following the path (seconds)")]
        public float pathLeadTime = 1f;// should be a higher number for less manuverable agents
        [Tooltip("The mimimum distance to look ahead when following the path (meters)")]
        public float minimumLeadDistance = 1f; // needs to be greater than 0 to avoid getting stuck :(
        [Tooltip("The mimimum speed to move at when following a path")]
        public float crawlSpeed = 1f; // needs to be greater than 0 to avoid getting stuck :(

        [Tooltip("How far into the future each step should look (seconds)")]
        public float stepTime;

        [Tooltip("How close to the goal we stop using the GOAP path")]
        public float terminalNavigationRadius = 5f;
        [Tooltip("How intensly to slow down when reaching the end of our path (m/s)")]
        public float endOfPathDeceleration = 3f;

        public VehicleSpecs vehicleSpecs;

        private void Start()
        {
            defaultMoveGoal = Instantiate(defaultMoveGoal);
        }

        /// <summary>
        /// Disables the driving logic
        /// Call this when you want to manually control the vehicle/agent
        /// from some other code
        /// </summary>
        public void DisableDriver()
        {
            mode = GOAPDriverMode.Disabled;
        }

        /// <summary>
        /// Commands the ai to go to a position.
        /// </summary>
        public void GoToPos(Vector3 pos, bool immediate = true)
        {
            mode = GOAPDriverMode.GoToPos;
            target = pos;
            defaultMoveGoal.goalPos = target;
            defaultMoveGoal.startingPos = tf.position;
            goal = defaultMoveGoal;
            if (immediate)
            {
                goalChanged = true;
                RequestPathIfNotAlready();
            }
        }

        /// <summary>
        /// Commands the ai to go to fly on a path to achieve a goal
        /// </summary>
        public void GoToGoal(IGOAPGoal goal, bool immediate = true)
        {
            mode = GOAPDriverMode.GoToGoal;
            this.goal = goal;
            if (immediate)
            {
                goalChanged = true;
                RequestPathIfNotAlready();
            }
        }

        /// <summary>
        /// Commands the AI to go to a position without the GOAP algorithm.
        /// Can be used for more scripted movement.
        /// I use it to make the AI land as the GOAP algorithm wont let it get
        /// too close to the ground.
        /// </summary>
        /// <param name="pos">The position to travel to</param>
        public void GoToPosTerminal(Vector3 pos)
        {
            target = pos;
            mode = GOAPDriverMode.TerminalOnly;
        }

        private void Update()
        {
            if (updateMode == DriverUpdateMode.Update)
            {
                UpdateLogic();
            }
        }

        private void FixedUpdate()
        {
            if (updateMode == DriverUpdateMode.FixedUpdate)
            {
                UpdateLogic();
            }
        }

        /// <summary>
        /// Updates the path generation and following logic
        /// </summary>
        private void UpdateLogic()
        {
            UpdateNavigation();

            cooldownTimer -= Time.deltaTime;

            if (request == null)
                return;

            switch (request.status)
            {
                case GOAPRequestStatus.Complete:
                    // a complete path! we can immediately switch to it!
                    SetPath(request);
                    request = null;
                    break;
                case GOAPRequestStatus.Partial:
                    // this path is only partially complete, switch to it if our current path is no good
                    // or this newer path will take us further than our current path would
                    if (pathValid == false || goalChanged || path == null || request.path.Count > path.Count - waypoint)
                    {
                        SetPath(request);
                    }
                    request = null;
                    break;
                case GOAPRequestStatus.Failed:
                case GOAPRequestStatus.InvalidStart:
                default:
                    // this path was no good, but thats fine, we can keep using our current path if we have one
                    request = null;
                    break;
                case GOAPRequestStatus.Waiting:
                case GOAPRequestStatus.Generating:
                    // we are waiting for the planner to finish generating us a path.
                    break;
            }
        }

        /// <summary>
        /// Sets the current path from the completed request by the GOAP algorithm
        /// </summary>
        /// <param name="requestToConvert">The request to use the path of</param>
        private void SetPath(GOAPPathRequest_Vehicle requestToConvert)
        {
            path = new List<GOAPWorldState_Vehicle>();
            foreach (GOAPWorldState state in requestToConvert.path)
            {
                path.Add(state as GOAPWorldState_Vehicle);
            }
            pathValid = true;
            goalChanged = false;
            pathType = request.status;

            waypoint = 0;
        }

        // variables used by navigation, declared here so they get reused instead of garbage collected
        Vector3 pathVector;
        Vector3 pathOffset;
        Vector3 pathPos;
        Vector3 lead;
        Vector3 targetPoint;
        Vector3 targetOffset;
        Vector3 targetVel;
        private void UpdateNavigation()
        {
            switch (mode)
            {
                case GOAPDriverMode.Disabled:
                    break;
                case GOAPDriverMode.Stop:
                    MoveVelocity(Vector3.zero);
                    break;
                case GOAPDriverMode.GoToPos:
                case GOAPDriverMode.GoToGoal:
                    // if we are close enough, ignore the path and go straigh to the goal 
                    if ((target - tf.position).magnitude < terminalNavigationRadius && mode == GOAPDriverMode.GoToPos)
                    {
                        TerminalNavigate();
                    }
                    else if (pathValid && path != null)
                    {
                        // if we are halfway along the path, generate a new one
                        if (goalChanged || (pathType == GOAPRequestStatus.Partial && waypoint > ((float)path.Count) / 2f))
                        {
                            RequestPathIfNotAlready();
                        }

                        if (waypoint + 1 < path.Count)
                        {
                            // This code is very difficult to read, I apologise for that.
                            // I will try and refactor it when I get time.
                            pathVector = path[waypoint + 1].position - path[waypoint].position;
                            pathOffset = tf.position - path[waypoint].position;
                            float pathDistance = Vector3.Dot(pathVector.normalized, pathOffset);
                            pathPos = path[waypoint].position + pathVector.normalized * pathDistance;

                            lead = pathVector.normalized * Mathf.Max(pathLeadTime * GetVelocity().magnitude, minimumLeadDistance);
                            targetPoint = pathPos + lead;
                            targetOffset = targetPoint - tf.position;
                            float pathT = (pathDistance + lead.magnitude) / pathVector.magnitude;

                            targetVel = targetOffset.normalized * Mathf.Lerp(path[waypoint].velocity.magnitude, path[waypoint + 1].velocity.magnitude, pathT);
                            float velMagnitude = targetVel.magnitude;
                            if (pathType == GOAPRequestStatus.Partial)
                            {
                                // If the path is only partial, it will not come to a complete stop at the end.
                                // We should slow down to avoid overshooting the end of the path and crashing.
                                velMagnitude = Mathf.Min(velMagnitude, CalculateMaxSpeed(pathT));
                            }
                            velMagnitude = Mathf.Max(velMagnitude, vehicleSpecs.minSpeed, crawlSpeed);
                            MoveVelocity(targetVel.normalized * velMagnitude);

                            if ((pathPos - tf.position).magnitude > maxPathDeviation)
                            {
                                RequestPathIfNotAlready();

                                if ((pathPos - tf.position).magnitude > maxPathDeviation * 4f)
                                {
                                    pathValid = false;
                                }
                            }
                            if (pathT > 1f || pathVector.magnitude <= 0.01f)
                            {
                                MoveVelocity(path[waypoint + 1].velocity);
                                waypoint++;
                            }
                        }
                        else
                        {
                            path = null;
                            RequestPathIfNotAlready();

                            //we have reached the end of the path, panic until we find a new path
                            Panic();
                        }
                    }
                    else
                    {
                        //our path is invalid or null, panic until we find a new path
                        RequestPathIfNotAlready();
                        Panic();
                    }
                    break;
                case GOAPDriverMode.TerminalOnly:
                    TerminalNavigate();
                    break;
                case GOAPDriverMode.Panic:
                    Panic();
                    break;
            }
        }

        /// <summary>
        /// Calculate how long we have left before we reach the end of our path.
        /// </summary>
        /// <param name="t">The current t along the waypoint</param>
        /// <returns>How long left until we reach the end of the path (seconds)</returns>
        private float CalculateTimeLeftInPath(float t)
        {
            // The t variable is confusing, please remove it when refactoring
            if (path == null || waypoint >= path.Count)
            {
                return 0f;
            }
            else
            {
                return ((path.Count - waypoint) + (1 - t)) * stepTime;
            }
        }

        /// <summary>
        /// Calculate the maximim speed we can travel along this path
        /// and safely stop at the end.
        /// </summary>
        /// <param name="t">The current t along the waypoint</param>
        /// <returns>The maximum speed (m/s)</returns>
        private float CalculateMaxSpeed(float t)
        {
            // The t variable is confusing, please remove it when refactoring
            return endOfPathDeceleration * CalculateTimeLeftInPath(t);
        }

        /// <summary>
        /// Returns the current path of the agent.
        /// Useful for debugging.
        /// Do not use this for anything else!
        /// </summary>
        /// <returns>The current path</returns>
        public List<GOAPWorldState_Vehicle> DebugGetPath()
        {
            return path;
        }

        protected void RequestPathIfNotAlready()
        {
            if (cooldownTimer <= 0f && request == null)
            {
                cooldownTimer = generationCooldown;
                RequestPath();
            }
        }

        /// <summary>
        /// Requests the a new path from the GOAPPathing system.
        /// </summary>
        private void RequestPath()
        {
            request = GetRequest();

            GOAPPathing.instance.RequestPath(request);
        }

        /// <summary>
        /// Override this function to return the path request for your agent type.
        /// </summary>
        /// <returns>A path request</returns>
        protected virtual GOAPPathRequest_Vehicle GetRequest()
        {
            Debug.LogError($"{GetType().Name} class needs to override GetRequest", this);
            return null;
        }

        /// <summary>
        /// Override to return the current velocity of the agent.
        /// On a physics based agent, this will usually just be Rigidbody.velocity
        /// But you may want to implement something else.
        /// </summary>
        /// <returns>The velocity of the agent</returns>
        protected virtual Vector3 GetVelocity()
        {
            Debug.LogError($"{GetType().Name} class needs to override GetRequest", this);
            return Vector3.zero;
        }

        /// <summary>
        /// Override this function to implement the behaviour to control the agent.
        /// The agent must try and match this velocity
        /// </summary>
        /// <param name="velocity">The velocity the agent should travel at.</param>
        protected virtual void MoveVelocity(Vector3 velocity)
        {
            Debug.LogError($"{GetType().Name} class needs to override GetRequest", this);
        }

        protected virtual void TerminalNavigate()
        {
            Debug.LogError($"{GetType().Name} class needs to override GetRequest", this);
        }

        /// <summary>
        /// Simple context menu option to test the panic mode
        /// The AI only relies on panic mode for short periods of time when needing to
        /// escape from a situation the GOAP can't solve or hasn't found a path yet.
        /// </summary>
        [ContextMenu("Test Panic Mode")]
        public void TestPanicMode()
        {
            mode = GOAPDriverMode.Panic;
        }

        /// <summary>
        /// Override this function to implement the agents panic behaviour.
        /// The panic behaviour is used as a backup when the AI doesn't have a path right now.
        /// This can happen because the GOAP can't find a valid path
        /// or because its generating one and hasn't finished yet.
        /// </summary>
        protected virtual void Panic()
        {
            Debug.LogError($"{GetType().Name} class needs to override GetRequest", this);
        }

        public virtual GOAPWorldState GetCurrentState()
        {
            Debug.LogError($"{GetType().Name} class needs to override GetRequest", this);
            return null;
        }

        public virtual bool IsAlive()
        {
            Debug.LogError($"{GetType().Name} class needs to override IsAlive", this);
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            // draws the current path, lovely
            if (path != null && path.Count > 1)
            {
                Gizmos.color = Color.yellow;
                path[path.Count - 1].StatePathDebugGizmo();
            }
        }
    }
}