using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Audio
{
    public static class Sound
    {
        private static List<AudioReleaser> globalAudioSourcePool = new();

        #region Playing

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Plays one instance of given audioclip.
        /// </summary>
        /// <param name="aClip">The clip name or further path. Ex: 'BirdChirp' or 'Birds/BlueBirdChirp'</param>
        /// <param name="aPos">A World position to play the audio at.</param>
        /// <param name="aIsThreeDimensional">If the audiosource uses directional audio</param>
        /// <param name="aFollowParent">A parent for audiosource to follow</param>
        /// <param name="aVolume"></param>
        /// <param name="aPitch"></param>
        /// <param name="aPitchShift"></param>
        /// <param name="aPitchVariance"></param>
        /// <param name="aMixerGroup"></param>
        /// <param name="aStoppable">If the audio source should be able to be paused or stopped</param>
        /// <returns></returns>
        public static AudioSource Play
        (
            AudioClip aClip,
            Vector3 aPos,
            bool aIsThreeDimensional = true,
            Transform aFollowParent = null,
            float aVolume = 1f,
            float aPitch = 1f,
            bool aPitchShift = true,
            float aPitchVariance = 0.10f,
            AudioMixerGroup aMixerGroup = null,
            bool aStoppable = false
        )
        {
            if (aClip != null)
            {
                AudioSource source = GetOrCreateFreeAudioSource();

                source.volume = aVolume;
                source.clip = aClip;

                if (aPitchShift)
                {
                    source.pitch = Random.Range(aPitch - aPitchVariance, aPitch + aPitchVariance);
                }
                else source.pitch = aPitch;

                if (!aFollowParent)
                {
                    source.transform.parent = aFollowParent;
                }

                if (aIsThreeDimensional)
                {
                    source.spatialBlend = 1f;
                }
                else source.spatialBlend = 0f;

                if (aMixerGroup)
                {
                    source.outputAudioMixerGroup = aMixerGroup;
                }
                else
                {
                    AudioMixer baseAudioMixer = null;

                    baseAudioMixer = Resources.Load<AudioMixer>("Mixer/AudioMixer");


                    if (baseAudioMixer)
                    {
                        try
                        {
                            AudioMixerGroup baseMixerGroup = baseAudioMixer.FindMatchingGroups("General")[0];
                            source.outputAudioMixerGroup = baseMixerGroup;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Sound.cs was unable to load AudioMixer from Play(...) argument, Was able to find AudioMixer from Resources but unable to find MixerGroup 'General'.");
                            Debug.LogError(e);
                            throw;
                        }
                    }
                    else
                    {
                        Debug.LogError("Sound.cs was unable to load a AudioMixer from Play(...) argument or Resources/Mixer/AudioMixer.mixer");
                    }
                }

                source.transform.position = aPos;

                if (aStoppable)
                {
                    source.PlayOneShot(aClip);
                }
                else
                {
                    source.Play();
                }

                // source.gameObject.TryGetComponent<AudioReleaser>(out AudioReleaser releaser);
                // if (!releaser)
                // {
                //     releaser = source.AddComponent<AudioReleaser>();
                // }
                //
                // releaser.SetToUsed();
                source.gameObject.TryGetComponent<AudioDestroyer>(out AudioDestroyer destroyer);
                if (!destroyer)
                {
                    destroyer = source.AddComponent<AudioDestroyer>();
                }
                

                return source;
            }
            else
            {
                Debug.LogError("Sound.Play was not able to play clip, Clip was null!");
                return null;
            }
        }

        /// <summary>
        /// Play from files in any 'Resources' folder inside unity.
        /// </summary>
        /// <param name="aClipName">The clip name or further path. Ex: 'BirdChirp' or 'Birds/BlueBirdChirp'</param>
        /// <param name="aPos">A World position to play the audio at.</param>
        /// <param name="aIsThreeDimensional">If the audiosource uses directional audio</param>
        /// <param name="aFollow">A parent for audiosource to follow</param>
        /// <param name="aVolume"></param>
        /// <param name="aPitch"></param>
        /// <param name="aPitchShift"></param>
        /// <param name="aPitchVariance"></param>
        /// <param name="aMixerGroup"></param>
        /// <param name="aStoppable">If the audio source should be able to be paused or stopped</param>
        /// <returns></returns>
        public static AudioSource PlayFromResource
        (
            string aClipName, Vector3 aPos,
            bool aIsThreeDimensional = true,
            Transform aFollow = null,
            float aVolume = 1f,
            float aPitch = 1f,
            bool aPitchShift = true,
            float aPitchVariance = 0.10f,
            AudioMixerGroup aMixerGroup = null,
            bool aStoppable = false
        )
        {
            return Play(Resources.Load<AudioClip>("Sfx/" + aClipName), aPos, aIsThreeDimensional, aFollow, aVolume, aPitch, aPitchShift, aPitchVariance, aMixerGroup, aStoppable);
        }

        #endregion

        private static AudioSource GetOrCreateFreeAudioSource()
        {
            // for (int i = 0; i < globalAudioSourcePool.Count; i++)
            // {
            //     if (globalAudioSourcePool[i].isFree)
            //     {
            //         return globalAudioSourcePool[i].MySource;
            //     }
            // }

            AudioSource newSource = new GameObject("Audio", typeof(AudioSource)).GetComponent<AudioSource>();
            globalAudioSourcePool.Add(newSource.gameObject.AddComponent<AudioReleaser>());
            return newSource;
        }

        private class AudioDestroyer : MonoBehaviour
        {
            private AudioSource mySource = null;

            private void Start()
            {
                mySource = gameObject.GetComponent<AudioSource>();
            }

            private void Update()
            {
                if (!mySource.isPlaying) Destroy(gameObject);
            }
        }

        private class AudioReleaser : MonoBehaviour
        {
            public AudioSource MySource { get; private set; } = null;
            public bool isFree = false;

            private void Start()
            {
                MySource = gameObject.GetComponent<AudioSource>();
            }

            private void Update()
            {
                if (!MySource.isPlaying) isFree = true;
            }

            public void SetToUsed()
            {
                isFree = false;
            }
        }
    }
}