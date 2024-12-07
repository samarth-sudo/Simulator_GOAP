using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class GroupHelicopterSpawner : MonoBehaviour
    {
        public GroupHelicopterTester groupHelicopter;

        public GameObject helicopterPrefab;

        public int targetHelicopterAmmount;

        private void Update()
        {
            if (groupHelicopter.helicopters.Count < targetHelicopterAmmount)
            {
                GameObject helicopterGo = Instantiate(helicopterPrefab, groupHelicopter.goal.transform.position, Quaternion.identity, groupHelicopter.transform);
                GOAPDriver_DemoHelicopter helicopter = helicopterGo.GetComponent<GOAPDriver_DemoHelicopter>();
                groupHelicopter.helicopters.Add(helicopter);

                helicopter.GoToPos(groupHelicopter.goal.position, true);
            }
            if (groupHelicopter.helicopters.Count > targetHelicopterAmmount)
            {
                GOAPDriver_DemoHelicopter helicopter = groupHelicopter.helicopters[groupHelicopter.helicopters.Count - 1];
                groupHelicopter.helicopters.Remove(helicopter);
                Destroy(helicopter.gameObject);
            }
        }
    }
}