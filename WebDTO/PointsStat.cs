using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.WebDTO
{
    [Serializable]
    public class PointsStat
    {
        public float[] pointsJury;
        public float wind;
        public float dist;
        public float distPoints;
        public float sum;
    }
}
