#region

using UnityEngine;

#endregion

namespace Game.Scripts
{
    public class ScoreSystem : MonoBehaviour
    {
        public float Score { get; private set; }


        public void Reset()
        {
            Score = 0;
        }

        public void AddScore(float increment)
        {
            Score += increment;
        }
    }
}