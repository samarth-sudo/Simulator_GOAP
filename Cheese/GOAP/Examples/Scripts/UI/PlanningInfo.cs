using UnityEngine;
using UnityEngine.UI;

namespace Cheese.GOAP.Demo
{
    public class PlanningInfo : MonoBehaviour
    {
        public Toggle showGoal;
        public Toggle showPath;
        public Toggle showPlanning;

        public Text totalRequests;
        public Text requestQueue;
        public Text generationLength;

        private void Start()
        {
            if (DebugGOAPVisualiser.instance == null || GOAPPathing.instance == null)
                return;

            UpdateVisualisers();
        }

        private void Update()
        {
            totalRequests.text = $"Total paths: {GOAPPathing.instance.totalPathsGenerated}";
            requestQueue.text = $"Queued requests: {GOAPPathing.instance.GetQueuedRequestAmmount()}";
            generationLength.text = $"Last generation time: {GOAPPathing.instance.generationLength.ToString("F2")}s";
        }

        public void UpdateVisualisers()
        {
            DebugGOAPVisualiser.instance.SetGoalsVisibile(showGoal.isOn);
            DebugGOAPVisualiser.instance.SetPathVisibile(showPath.isOn);
            PlanningVisualiser.instance.ShowPlanning(showPlanning.isOn);
        }
    }
}