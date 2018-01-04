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
        public Slider Etherium;
        public Slider Blue;
        public Slider Red;
        public Slider Green;

        public Image BlueEngine;
        public Image RedEngine;
        public Image GreenEngine;
        public Image BlueCannon;
        public Image RedCannon;
        public Image GreenCannon;
        public Image BlueForge;
        public Image RedForge;
        public Image GreenForge;

        public void HPUpdate(float hp, float maxHP)
        {
            HP.text = "HP: " + hp + "/" + maxHP;
        }

        public void ResourceUpdate(Dictionary<ResourceType, float> r, float tankSize)
        {
            Blue.value = r[ResourceType.Blue] / tankSize;
            Red.value = r[ResourceType.Red] / tankSize;
            Green.value = r[ResourceType.Green] / tankSize;
            Etherium.value = r[ResourceType.Etherium] / tankSize;
        }

        public static Dictionary<TankSystem, List<string>> PowerupNames = new Dictionary<TankSystem, List<string>>
        {
            {TankSystem.Engine, new List<string>() {"Engine", "Speed", "Repair", "Sponson", "Sensor", "Lightning", "Blank", "Hovertank"} },
            {TankSystem.Cannon, new List<string>() {} },
            {TankSystem.Forge, new List<string>() {} },
        };

        public void UpdateSystemDisplay(Dictionary<ResourceType, Dictionary<TankSystem, bool>> SystemGrid)
        {
            BlueEngine.color = SystemGrid[ResourceType.Blue][TankSystem.Engine] ? Color.blue : Color.white;
            RedEngine.color = SystemGrid[ResourceType.Red][TankSystem.Engine] ? Color.red : Color.white;
            GreenEngine.color = SystemGrid[ResourceType.Green][TankSystem.Engine] ? Color.green : Color.white;
            BlueCannon.color = SystemGrid[ResourceType.Blue][TankSystem.Cannon] ? Color.blue : Color.white;
            RedCannon.color = SystemGrid[ResourceType.Red][TankSystem.Cannon] ? Color.red : Color.white;
            GreenCannon.color = SystemGrid[ResourceType.Green][TankSystem.Cannon] ? Color.green : Color.white;
            BlueForge.color = SystemGrid[ResourceType.Blue][TankSystem.Forge] ? Color.blue : Color.white;
            RedForge.color = SystemGrid[ResourceType.Red][TankSystem.Forge] ? Color.red : Color.white;
            GreenForge.color = SystemGrid[ResourceType.Green][TankSystem.Forge] ? Color.green : Color.white;

            int engineName = (SystemGrid[ResourceType.Red][TankSystem.Engine] ? 1 : 0) 
                + (SystemGrid[ResourceType.Green][TankSystem.Engine] ? 2 : 0)
                + (SystemGrid[ResourceType.Blue][TankSystem.Engine] ? 4 : 0);

            

        }

    
    }
}

