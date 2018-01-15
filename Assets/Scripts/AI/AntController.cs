using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.AI
{
    class AntController : Unit
    {
        private Tilemap tm;

        public override void TakeDamage(float damage)
        {
            throw new NotImplementedException();
        }

        protected override void OnChangedHealth(float currentHealth)
        {
            throw new NotImplementedException();
        }

        private void Update()
        {
            var c = tm.cellLayout;
        }
    }
}
