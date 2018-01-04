using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine; 

namespace Assets.Scripts
{
    class CollectionHopper : MonoBehaviour
    {
        public BaseController bc;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            bc.TankAtHopper(collision.gameObject);    
        }
    }
}
