#region

using UnityEngine;

#endregion

namespace Game.Scripts
{
    public class MovingObstacle : MonoBehaviour
    {
        private bool _target;
        public float limitation = 2f;
        private readonly float delta = 1.5f;


        // Update is called once per frame
        private void Update()
        {
            if (_target == false)
            {
                transform.Translate(-delta * Time.deltaTime, 0, 0, Space.World);
                TargetControl();
            }
            else if (_target)
            {
                transform.Translate(delta * Time.deltaTime, 0, 0, Space.World);
                TargetControl();
            }
        }

        public void TargetControl()
        {
            if (transform.position.x > limitation || transform.position.x < -limitation) _target = !_target;
        }
    }
}