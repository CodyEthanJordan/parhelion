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

        internal void ToggleIndicator(string name, bool on)
        {
            if (name == "BlueEngine")
            {
                if (on)
                {
                    BlueEngine.color = Color.blue;
                }
                else
                {
                    BlueEngine.color = Color.white;
                }
            }
            else if (name == "RedEngine")
            {
                if (on)
                {
                    RedEngine.color = Color.red;
                }
                else
                {
                    RedEngine.color = Color.white;
                }
            }
            else if (name == "GreenEngine")
            {
                if (on)
                {
                    GreenEngine.color = Color.green;
                }
                else
                {
                    GreenEngine.color = Color.white;
                }
            }
            else if (name == "BlueCannon")
            {
                if (on)
                {
                    BlueCannon.color = Color.blue;
                }
                else
                {
                    BlueCannon.color = Color.white;
                }
            }
            else if (name == "RedCannon")
            {
                if (on)
                {
                    RedCannon.color = Color.red;
                }
                else
                {
                    RedCannon.color = Color.white;
                }
            }
            else if (name == "GreenCannon")
            {
                if (on)
                {
                    GreenCannon.color = Color.green;
                }
                else
                {
                    GreenCannon.color = Color.white;
                }
            }
            else if (name == "BlueForge")
            {
                if (on)
                {
                    BlueForge.color = Color.blue;
                }
                else
                {
                    BlueForge.color = Color.white;
                }
            }
            else if (name == "RedForge")
            {
                if (on)
                {
                    RedForge.color = Color.red;
                }
                else
                {
                    RedForge.color = Color.white;
                }
            }
            else if (name == "GreenForge")
            {
                if (on)
                {
                    GreenForge.color = Color.green;
                }
                else
                {
                    GreenForge.color = Color.white;
                }
            }
        }
    }
}

