#region

using System.Collections;
using UnityEngine;

#endregion

namespace Game.Scripts
{
    public class MapController : MonoBehaviour
    {
        [Header("Map Settings")] [Range(0.0f, 0.01f)]
        public float movingSpeed;

        [Range(0.0f, 0.001f)] public float speedIncrementOverTime = 0.0001f;
        [Range(0.1f, 20.0f)] public float timeInterval = 10.0f;

        public GameObject map;
        public GameObject player;
        public GameObject playerBody;

        public GameObject initialMap;
        // [Range(0.0f, 10.0f)] public float nextMapGenThreshold = 6.0f;
        // [Range(-40.0f, -15.0f)] public float mapDestroyThreshold = -25.0f;

        [Tooltip("If player reaches almost at the top of the screen, this function will keep player in screen.")]
        [Range(0.0f, 10.0f)]
        public float mapScrollBurstThreshold;

        // [Tooltip("The time length for scroll burst")] [Range(0.0f, 1000.0f)]
        // public float mapScrollBurstLength = 240.0f;


        // Use this for initialization
        private void Start()
        {
            StartCoroutine(SpeedTimer());
        }


        private IEnumerator SpeedTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(timeInterval);
                movingSpeed += speedIncrementOverTime;
            }
        }


        private void AllShift(float shift)
        {
            var newPos = map.transform.position;
            newPos.y -= shift;
            map.transform.position = newPos;

            if (player != null)
            {
                newPos = player.transform.position;
                newPos.y -= shift;
                player.transform.position = newPos;
            }
        }

        private void FixedUpdate()
        {
            var newPos = map.transform.position;
            newPos.y -= movingSpeed;
            map.transform.position = newPos;

            if (player != null)
            {
                newPos = player.transform.position;
                newPos.y -= movingSpeed;
                player.transform.position = newPos;
            }

            if (playerBody != null)
                if (playerBody.transform.position.y > mapScrollBurstThreshold)
                    AllShift((playerBody.transform.position.y - mapScrollBurstThreshold) / 240.0f);
        }
    }
}