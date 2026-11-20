using System.Collections;
using UnityEngine;

namespace Testing
{
    public class SoundTest : MonoBehaviour
    {
        [SerializeField] private bool playAudio = false;
        [SerializeField] private int playCount = 2;
        private AudioSource myAudioSource = null;
        private Coroutine myPlayingCoroutine = null;

        void Update()
        {
            if (playAudio)
            {
                if (myPlayingCoroutine == null)
                {
                    myPlayingCoroutine = StartCoroutine(PlayTwice(playCount));
                }
            }
        }

        IEnumerator PlayTwice(int aCount = 1)
        {
            int i = 0;
            while (i < aCount)
            {
                i++;
                myAudioSource = Audio.Sound.PlayFromResource("test", transform.position, aPitchShift: false);
                yield return new WaitWhile(() => myAudioSource == true);
            }
            
            myPlayingCoroutine = null;
            playAudio = false;
        }
    }
}