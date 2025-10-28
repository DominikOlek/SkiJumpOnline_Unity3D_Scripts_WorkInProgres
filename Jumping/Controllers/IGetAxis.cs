using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Jumping.Controllers
{
    internal interface IGetAxis
    {
        /// <summary>
        /// Get curent axis value in order X,Y
        /// </summary>
        /// <returns></returns>
        public (float, float) getAxis();
    }
}
