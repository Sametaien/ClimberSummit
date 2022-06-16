#region

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

#endregion

namespace Game.Scripts
{
    public class PlayerAction : MonoBehaviour
    {
        [Header("Map related settings")] [Tooltip("Specifies Layer of Rocks")]
        public LayerMask rockLayer = -1;

        [Tooltip("The maximum distance from released limb position to nearest rock")] [Range(0.5f, 5.0f)]
        public float errorRange;

        [Tooltip("The whole body game object")]
        public GameObject wholeBody;

        [Header("Control related settings")] [Tooltip("Specifies Layer of limb control points here")]
        public LayerMask limbsLayer = -1;

        [Tooltip("Control points game object.\nA sphere collider is required in each control points.")]
        public GameObject[] controlPoints;

        [Tooltip("Limbs from human model. It is required each of them MATCHES the control points.")]
        public GameObject[] limbs;

        public GameObject[] warning;
        public GameObject[] error;

        [Tooltip("Feet for calculating new body position, can be either control points or model itself.")]
        public GameObject[] feet;

        [Tooltip("The maximum range between body and limbs.")] [Range(1.0f, 5.0f)]
        public float rangeLimit;

        public GameObject bodyHeightReference;


        [Header("Player Related Settings")] [Range(10.0f, 200.0f)]
        public int maxHealth;

        public int decreaseHpRate = 5;

        [Range(10.0f, 200.0f)] public int maxPower;

        [Header("UI Related Settings")] public Text healthText;

        [Header("Game System Related")] public ScoreSystem scoreSystem;
        [Range(-10.0f, -2.0f)] public float gameOverMenuThreshold = -3.0f;
        public GameObject rock2GrabIndicator;


        [Range(-1.0f, 0)] public float droppingGravity = -0.01f;

        [FormerlySerializedAs("MessageOutput")] [Header("Developer Settings")]
        public bool messageOutput;

        private int _limbselI; //index of limbs or -1 means no selection

        private Vector3 _originalPos;

        private int _limbNotHold;

        private float _healthi;

        private float _heightDecented;
        private float _droppingSpeed;

        private Vector3 _referenceP;

        // Use this for initialization
        private void Start()
        {
            _limbselI = -1;

            _healthi = maxHealth;
            foreach (var t in feet)
            {
                var index = Array.FindIndex(limbs, x => x == t);
                if (index < 0) index = Array.FindIndex(controlPoints, x => x == t);
            }


            _originalPos = new Vector3();
            _limbNotHold = -1;
            _heightDecented = 0;
            healthText.text = "HP: " + _healthi.ToString("0") + "/" + maxHealth.ToString("0");
        }


        
        private void FixedUpdate()
        {
            Vector3 userpos;
            bool pressing;

            _referenceP = bodyHeightReference.transform.position;

            if (bodyHeightReference.transform.position.y < gameOverMenuThreshold) _healthi = 0;

            if (_healthi <= 0)
            {
                Vector3 newpos;

                newpos = wholeBody.transform.position;
                newpos.y += _droppingSpeed;
                wholeBody.transform.position = newpos;

                _droppingSpeed += droppingGravity;

                if (newpos.y < gameOverMenuThreshold) SceneManager.LoadScene("OGame");

                return;
            }


            userpos = new Vector3();
            if (Input.touchSupported)
            {
                //Touch screen
                if (Input.touchCount > 0)
                {
                    userpos = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.0f);
                    pressing = true;
                }
                else
                {
                    pressing = false;
                }
            }
            else
            {
                //Mouse
                userpos = Input.mousePosition;
                userpos.z = 0;
                if (Input.GetMouseButton(0)) //Left mouse button down
                    pressing = true;
                else
                    pressing = false;
            }

            if (pressing)
            {
                if (Camera.main != null)
                {
                    var ray = Camera.main.ScreenPointToRay(userpos);
                    if (Physics.Raycast(ray, out var rayhit, Mathf.Infinity, limbsLayer.value))
                    {
                        var newpos = Camera.main.ScreenToWorldPoint(new Vector3(userpos.x, userpos.y, 11));
                        newpos.z = 0;
                        //Finding which limb is selected
                        if (_limbselI == -1)
                        {
                            for (var i = 0; i < 4; i++)
                                if (Vector3.Distance(controlPoints[i].transform.position, rayhit.transform.position) ==
                                    0)
                                {
                                    _limbselI = i;
                                    break;
                                }

                            if (_limbNotHold != -1 && _limbselI != _limbNotHold)
                                _limbselI = -1;
                            else
                                _limbNotHold = -1;
                            if (_limbselI != -1) _originalPos = controlPoints[_limbselI].transform.position;
                        }

                        if (_limbselI != -1)
                        {
                            controlPoints[_limbselI].transform.position = newpos;
                            var position = wholeBody.transform.position;
                            var dist2Body =
                                Vector3.Distance(
                                    new Vector3(position.x, bodyHeightReference.transform.position.y,
                                        position.z), newpos);
                            warning[_limbselI].SetActive(dist2Body > rangeLimit);
                        }
                    }
                }
            }
            else
            {
                //pressing
                //User is not pressing
                if (_limbselI != -1)
                {
                    //User just release control
                    var nearRocks = Physics.OverlapSphere(controlPoints[_limbselI].transform.position, errorRange,
                        rockLayer.value);
                    if (messageOutput)
                    {
                        print("You've just released control");
                        print("Previous limbselI: " + _limbselI);
                        print(nearRocks.Length + " rocks found nearby");
                    }

                    //Finding nearest rock
                    var mindist = errorRange;
                    var nearestPos = _originalPos;
                    var nearestIndex = -1;
                    for (var i = 0; i < nearRocks.Length; i++)
                    {
                        var dist = Vector3.Distance(controlPoints[_limbselI].transform.position,
                            nearRocks[i].transform.position);
                        var position = wholeBody.transform.position;
                        var dist2Body =
                            Vector3.Distance(
                                new Vector3(position.x, _referenceP.y, position.z),
                                nearRocks[i].transform.position);
                        if (dist < mindist && dist2Body < rangeLimit)
                        {
                            mindist = dist;
                            nearestPos =
                                new Vector3(
                                    nearRocks[i].transform.position.x + nearRocks[i].transform.localScale.x * 0f,
                                    nearRocks[i].transform.position.y + nearRocks[i].transform.localScale.y * 0, 0.0f);
                            nearestIndex = i;
                        }
                    }

                    controlPoints[_limbselI].transform.position = nearestIndex < 0 ? _originalPos : nearestPos;
                    warning[_limbselI].SetActive(false);
                    // Triggers OnHold Handler
                    if (nearestIndex >= 0)
                    {
                        var rock = nearRocks[nearestIndex].gameObject.GetComponent<RockBase>();
                        if (rock != null) rock.OnHoldEventHandler(gameObject, _limbselI);
                    }


                    _limbselI = -1;
                } //limbselI == -1
            } //pressing

            var bodyNewPos = new Vector3();
            for (var i = 0; i < controlPoints.Length; i++)
                if (_limbNotHold != i)
                {
                    if (Vector3.Distance(controlPoints[i].transform.position, _referenceP) > rangeLimit)
                        limbs[i].transform.position = controlPoints[i].transform.position *
                                                      (rangeLimit / Vector3.Distance(
                                                          controlPoints[i].transform.position,
                                                          _referenceP));
                    else
                        limbs[i].transform.position = controlPoints[i].transform.position;
                    error[i].SetActive(false);
                }
                else
                {
                    error[i].SetActive(true);
                    controlPoints[i].transform.position = limbs[i].transform.position;
                }

            foreach (var obj in feet)
            {
                var position = obj.transform.position;
                bodyNewPos.x += position.x / 2;
                bodyNewPos.z += position.z / 2;
            }

            bodyNewPos.y = feet[0].transform.position.y < feet[1].transform.position.y
                ? feet[0].transform.position.y
                : feet[1].transform.position.y;

            #region Score Calculation

            var heightDiff = bodyNewPos.y - wholeBody.transform.position.y;
            if (heightDiff > 0)
            {
                if (heightDiff + _heightDecented > 0)
                {
                    scoreSystem.AddScore((heightDiff + _heightDecented) * 100);
                    _heightDecented = 0;
                }
                else
                {
                    _heightDecented = heightDiff + _heightDecented;
                }
            }
            else
            {
                _heightDecented += heightDiff;
            }

            #endregion


            wholeBody.transform.position = bodyNewPos;

            if (pressing)
            {
                var nearRocks =
                    Physics.OverlapSphere(controlPoints[_limbselI].transform.position, errorRange, rockLayer.value);
                var mindist = errorRange;
                var nearestIndex = -1;
                for (var i = 0; i < nearRocks.Length; i++)
                {
                    var dist = Vector3.Distance(controlPoints[_limbselI].transform.position,
                        nearRocks[i].transform.position);
                    var position = wholeBody.transform.position;
                    var dist2Body =
                        Vector3.Distance(
                            new Vector3(position.x, _referenceP.y, position.z),
                            nearRocks[i].transform.position);
                    if (dist < mindist && dist2Body < rangeLimit)
                    {
                        mindist = dist;
                        nearestIndex = i;
                    }
                }

                if (nearestIndex >= 0)
                {
                    rock2GrabIndicator.SetActive(true);
                    rock2GrabIndicator.transform.position = new Vector3(nearRocks[nearestIndex].transform.position.x,
                        nearRocks[nearestIndex].transform.position.y, 0.0f);
                }
                else
                {
                    rock2GrabIndicator.SetActive(false);
                }
            }
            else
            {
                rock2GrabIndicator.SetActive(false);
            }
        } //FixedUpdate

        public void IncreaseHp(int increment)
        {
            _healthi += increment;
            if (_healthi > maxHealth) _healthi = maxHealth;
            if (_healthi <= 0)
            {
            }
        }

        public void DecreaseHp()
        {
            _healthi -= decreaseHpRate * Time.deltaTime;

            if (_healthi == 0) SceneManager.LoadScene("OGame");
            healthText.text = "HP: " + _healthi.ToString("0") + "/" + maxHealth.ToString("0");
        }
    }
}