using UnityEngine;
using TMPro;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveCounter : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI gui;
        public void SetString(string value)
        {
            while (value.Length < 2)
            {
                value = "0" + value;
            }
            gui.text = value;
        }
    }
}