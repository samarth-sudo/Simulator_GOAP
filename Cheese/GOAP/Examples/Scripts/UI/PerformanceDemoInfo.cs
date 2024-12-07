using UnityEngine;
using UnityEngine.UI;

namespace Cheese.GOAP.Demo
{
    public class PerformanceDemoInfo : MonoBehaviour
    {
        public Text unitCountText;
        public Slider unitCount;
        public float defaultUnitCount;

        public Text searchSpeedText;
        public Slider searchSpeed;
        public float defaultSearchSpeed;

        public Text searchDepthText;
        public Slider searchDepth;
        public float defaultSearchDepth;

        public GroupHelicopterSpawner spawner;

        private void Start()
        {
            defaultUnitCount = spawner.targetHelicopterAmmount;
            defaultSearchSpeed = GOAPPathing.instance.maxSearchesPerFrame;
            defaultSearchDepth = GOAPPathing.instance.maxSearchDepth;

            UpdateSettings();
        }

        public void ResetSettings()
        {
            unitCount.value = defaultUnitCount;
            searchSpeed.value = defaultSearchSpeed;
            searchDepth.value = defaultSearchDepth;

            UpdateSettings();
        }

        public void UpdateSettings()
        {
            unitCountText.text = $"Number of Units: {unitCount.value}";
            spawner.targetHelicopterAmmount = (int)unitCount.value;

            searchSpeedText.text = $"Search Speed: {searchSpeed.value}";
            GOAPPathing.instance.maxSearchesPerFrame = (int)searchSpeed.value;

            searchDepthText.text = $"Search Depth: {searchDepth.value}";
            GOAPPathing.instance.maxSearchDepth = (int)searchDepth.value;
        }
    }
}