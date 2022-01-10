#region

using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Game.Scripts
{
    public class LoadScane : MonoBehaviour
    {
        public void RestartGame()
        {
            SceneManager.LoadScene("Game");
        }

        public void StartGame()
        {
            SceneManager.LoadScene("Game");
        }
    }
}