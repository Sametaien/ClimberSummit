#region

using UnityEngine;

#endregion

namespace Game.Scripts
{
    public class ObstacleHit : MonoBehaviour
    {
        public PlayerAction playerScript;


        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Saw") playerScript.DecreaseHp();
        }
    }
}