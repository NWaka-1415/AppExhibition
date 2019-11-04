using System;
using UnityEngine;

namespace Controllers
{
    public class AudioController : MonoBehaviour
    {
        //めんどくさいのでアタッチで
        [SerializeField] AudioClip[] audioClips = new AudioClip[0];

        [SerializeField] private AudioSource audioSource = null;

        public enum AudioPattern
        {
            Open,
            Move,
            Select,
            Cancel
        }

        private static AudioController _instance = null;

        public static AudioController Instance => _instance;

        private void Awake()
        {
            if (_instance == null) _instance = this;
            else if (_instance != this) Destroy(gameObject);
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            audioSource.loop = false;
        }

        public void Play(AudioPattern audioPattern)
        {
            switch (audioPattern)
            {
                case AudioPattern.Open:
                    audioSource.clip = audioClips[0];
                    break;
                case AudioPattern.Move:
                    audioSource.clip = audioClips[1];
                    break;
                case AudioPattern.Select:
                    audioSource.clip = audioClips[2];
                    break;
                case AudioPattern.Cancel:
                    audioSource.clip = audioClips[3];
                    break;
            }

            audioSource.Play();
        }
    }
}