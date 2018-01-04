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

        public Text EngineText;
        public Text CannonText;
        public Text ForgeText;

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
            HP.text = "HP: " + hp.ToString("N0") + "/" + maxHP;
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
            {TankSystem.Cannon, new List<string>() {"Cannon", "Laser", "Repair-Beam", "Rocket", "Machinegun", "Triple", "Drill", "DEATH RAY"} },
            {TankSystem.Forge, new List<string>() {"Forge", "Missiles", "Blink", "Blank", "Wall", "Attack Drone", "Turret", "Town Portal"} },
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

            EngineText.text = PowerupNames[TankSystem.Engine][engineName];

            int cannonName = (SystemGrid[ResourceType.Red][TankSystem.Cannon] ? 1 : 0)
                + (SystemGrid[ResourceType.Green][TankSystem.Cannon] ? 2 : 0)
                + (SystemGrid[ResourceType.Blue][TankSystem.Cannon] ? 4 : 0);

            CannonText.text = PowerupNames[TankSystem.Cannon][cannonName];

            int forgeName = (SystemGrid[ResourceType.Red][TankSystem.Forge] ? 1 : 0)
                + (SystemGrid[ResourceType.Green][TankSystem.Forge] ? 2 : 0)
                + (SystemGrid[ResourceType.Blue][TankSystem.Forge] ? 4 : 0);

            ForgeText.text = PowerupNames[TankSystem.Forge][forgeName];
        }


    }
}

