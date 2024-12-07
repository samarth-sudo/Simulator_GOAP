using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Cheese.GOAP.Demo
{
    public class DemoManager : MonoBehaviour
    {
        public static DemoManager instance;

        public DemoUnit demoUnit;
        public List<DemoUnit> allDemoUnits;

        public UnityEvent onUnitChange;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            SetUnit(demoUnit);
        }

        public void RegisterUnit(DemoUnit unit)
        {
            allDemoUnits.Add(unit);
        }

        public void UnRegisterUnit(DemoUnit unit)
        {
            allDemoUnits.Remove(unit);
        }

        public void SetUnit(DemoUnit unit)
        {
            demoUnit = unit;

            onUnitChange.Invoke();
        }

        public void NextUnit()
        {
            SetUnit(allDemoUnits.IndexOf(demoUnit) + 1);
        }

        public void PreviousUnit()
        {
            SetUnit(allDemoUnits.IndexOf(demoUnit) - 1);
        }

        public void SetUnit(int id)
        {
            if (id >= allDemoUnits.Count)
            {
                id = 0;
            }
            if (id < 0)
            {
                id = allDemoUnits.Count - 1;
            }

            SetUnit(allDemoUnits[id]);
        }

        private void Update()
        {
            if (demoUnit == null && allDemoUnits.Count > 0)
            {
                SetUnit(0);
            }
        }
    }
}