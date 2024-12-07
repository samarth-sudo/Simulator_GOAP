using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class GroupHelicopterTester : MonoBehaviour
    {
        public List<GOAPDriver_DemoHelicopter> helicopters;

        public Transform[] spawnPoints;
        public float spawnRadius;

        public Transform goal;
        public float completionRadius;

        private void Start()
        {
            helicopters = GetComponentsInChildren<GOAPDriver_DemoHelicopter>().ToList();
            foreach (GOAPDriver_DemoHelicopter helicopter in helicopters)
            {
                helicopter.GoToPos(goal.position, true);
            }
        }

        private void Update()
        {
            foreach (GOAPDriver_DemoHelicopter helicopter in helicopters)
            {
                if ((helicopter.transform.position - goal.position).magnitude < completionRadius)
                {
                    helicopter.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position + Random.insideUnitSphere * spawnRadius;
                    helicopter.transform.rotation = Quaternion.identity;

                    helicopter.pilot.helicopter.rb.velocity = Vector3.zero;
                    helicopter.pilot.helicopter.rb.angularVelocity = Vector3.zero;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, spawnRadius);
                }
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(goal.position, completionRadius);
        }
    }
}