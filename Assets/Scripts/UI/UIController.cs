using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    class UIController : MonoBehaviour
    {
        public Text HP;
        public Text Etherium;
        public Text Green;
        public Text Red;
        public Text Blue;

        public void HPUpdate(float hp, float maxHP)
        {
            HP.text = "HP: " + hp + "/" + maxHP;
        }

        public void ResourceUpdate(Dictionary<ResourceType, int> r, int tankSize)
        {
            Etherium.text = "Etherium: " + r[ResourceType.Etherium] + "/" + tankSize;
            Green.text = "Green: " + r[ResourceType.Green] + "/" + tankSize;
            Red.text = "Red: " + r[ResourceType.Red] + "/" + tankSize;
            Blue.text = "Blue: " + r[ResourceType.Blue] + "/" + tankSize;
        }

    }
}
