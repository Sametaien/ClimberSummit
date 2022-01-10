#region

using UnityEngine;

#endregion

namespace Game.Scripts
{
    public sealed class RockBase : MonoBehaviour
    {
        private const int HPincrement = 5;

        // Use this for initialization
        public void RockStart()
        {
        }

        private void Start()
        {
            RockStart();
        }

        // Update is called once per frame
        public void RockUpdate()
        {
        }

        private void Update()
        {
            RockUpdate();
        }

        public void RockFixedUpdate()
        {
        }

        private void FixedUpdate()
        {
            RockFixedUpdate();
        }

        public void OnHoldEventHandler(GameObject player, int triggerLimb)
        {
            player.GetComponent<PlayerAction>().IncreaseHp(HPincrement);
        }
    }
}