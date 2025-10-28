using Assets.Scripts.WebDTO;
using UnityEngine;

namespace Assets.Scripts.Jumping.Controllers
{
    public interface ISetResult
    {
        /// <summary>
        /// SetResult is using in Points script to save jump result
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="isShowResult"></param>
        public void SetResult(PointsStat stats, bool isShowResult = true);
    }
}