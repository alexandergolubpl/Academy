using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class SoundButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Text buttonText;

        private string id;
        private Action<string> onPlaySoundCallback;

        public void Setup(string soundId, Action<string> callback)
        {
            buttonText.text = soundId;
            id = soundId;
            onPlaySoundCallback = callback;
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnSoundButtonClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnSoundButtonClicked);
        }

        private void OnSoundButtonClicked()
        {
            onPlaySoundCallback?.Invoke(id);
        }
    }

}