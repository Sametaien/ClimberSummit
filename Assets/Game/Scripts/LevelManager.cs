#region

using System;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Game.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Score")] public Text scoreText;
        public ScoreSystem scoreSystem;

        private void Update()
        {
            scoreText.text = "Score: " + Convert.ToInt32(scoreSystem.Score);
        }
    }
}