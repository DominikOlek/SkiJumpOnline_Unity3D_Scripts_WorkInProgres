using UnityEngine;

namespace Assets.Scripts.Jumping.Controllers
{
    public interface IToBeat : IGetDifficulty
    {
        /// <summary>
        /// Return how many points player lost(bool = false) or same distance(bool = true)
        /// </summary>
        /// <returns></returns>
        public (float, bool) DiffToBeat();
    }
}