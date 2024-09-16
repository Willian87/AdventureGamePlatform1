using UnityEngine.Audio;
using UnityEngine;
using System;


public class SoundManagment : MonoBehaviour
{
    [SerializeField] private Sound[] sounds;

    public static SoundManagment instance;

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds)
        {
            s.audioSource = gameObject.AddComponent<AudioSource>();
            s.audioSource.clip = s.clip;

            s.audioSource.volume = s.volume;
            s.audioSource.pitch = s.pitch;
            s.audioSource.loop = s.loop;
        }
    }
    private void Start()
    {
        Play("BG");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning("Song " + name + " not found!");
            return;      
        }
        s.audioSource.Play();
    }
}

//If anyone is curious on how to stop playing a looping sound (maybe you want to switch bg music when you enter a new area) just use s.source.Stop (); You can just add a method to the Audio Manager like this

// public void StopPlaying(string sound)
//{
//    Sound s = Array.Find(sounds, item => item.name == sound);
//    if (s == null)
//    {
//        Debug.LogWarning("Sound: " + name + " not found!");
//        return;
//    }

//    s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
//    s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

//    s.source.Stop();
//}

//just call it like this (AudioManagerReferenceGoeshere).StopPlaying("sound string name");




//public class SoundManagment : MonoBehaviour
//{
//    public static SoundManagment instance;

//    AudioSource audioSource;

//    [SerializeField] private AudioClip coinSound;


//    private void OnEnable()
//    {
//        Coins.OnCoinCollected += PlayCoinSound;
//    }
//    // Start is called before the first frame update
//    void Start()
//    {
//        audioSource = GetComponent<AudioSource>();
//    }

//    private void OnDisable()
//    {
//        Coins.OnCoinCollected -= PlayCoinSound;
//    }
//    // Update is called once per frame
//    void Update()
//    {

//    }

//    public void PlayCoinSound()
//    {
//        PlayAudioClip(coinSound);
//    }

//    public void PlayAudioClip(AudioClip clip)
//    {
//        audioSource.PlayOneShot(clip);
//    }
//}
