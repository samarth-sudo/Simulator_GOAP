using UnityEngine;
using UnityEngine.UI;

namespace Cheese.GOAP.Demo
{
    public class UnitInfoPannel : MonoBehaviour
    {
        public Text unitNameText;
        public Text unitDescriptionText;

        public void UpdateUnitInfo()
        {
            if (DemoManager.instance.demoUnit == null)
                return;

            unitNameText.text = DemoManager.instance.demoUnit.unitName;
            unitDescriptionText.text = DemoManager.instance.demoUnit.unitDescription;
        }

        public void NextUnit()
        {
            DemoManager.instance.NextUnit();
        }

        public void PreviousUnit()
        {
            DemoManager.instance.PreviousUnit();
        }
    }
}