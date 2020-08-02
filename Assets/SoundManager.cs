using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEvent
{
    Stop,
    Start,
    Pause,
    IsComplete
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioClip[] bgmClips, sfxClips;
    private int flag;
    public delegate void SoundEventFunction(SoundEvent evt, AudioSource src);
    public static event SoundEventFunction OnSoundChanged;
    public static event SFXFunction OnSFXStarted;
    public static event SFXFunction OnSFXEnded;
    bool pause=false;
    public delegate void SFXFunction(AudioSource src);
    public float curtime;
    public List<GameObject> players = new List<GameObject>();
    private List<GameObject> bgs = new List<GameObject>();
    private List<float> bgvolumes = new List<float>();
    private int count = 0;

    static float _BGMVolumeMultiplier = 1;
    public static float BGMVolumeMultiplier {
        get
        {
            return _BGMVolumeMultiplier;
        }
        set
        {
            _BGMVolumeMultiplier = value;
            for (int i = 0; i < instance.bgs.Count; ++i)
            {
                instance.bgs[i].GetComponent<AudioSource>().volume = instance.bgvolumes[i] * _BGMVolumeMultiplier;
            }
        }
    }
    public static float SFXVolumeMultiplier = 1;
    GameObject bgm;
    void Start()
    {
        flag = 0;
        curtime = 0;
        //this.PlayBGM(bgmClips[0],0.22f);
    }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    void Update()
    {
        if (players.Count >= 0)
        {
            for(int i=0;i<= players.Count - 1; i++)
            {
                if (!players[i].GetComponent<AudioSource>().isPlaying)
                {
                    OnSFXEnded?.Invoke(players[i].GetComponent<AudioSource>());
                    Destroy(players[i]);
                    players.Remove(players[i]);
                }
            }
        }
        if (curtime != 0)
        {
            curtime = Time.time;
        }
    }
    public void PlayBGM()
    {
        if (bgm != null)
        {
            if (pause)
            {
                bgm.GetComponent<AudioSource>().UnPause();
                pause = false;
            }
            else
                bgm.GetComponent<AudioSource>().Play();
        }
    }
    public void PlayBGM(AudioClip bgm, float volume = 1, bool looping = true,float pitch = 1f, float pan = 0,float time=0)
    {
        flag = 1;
        GameObject bgmplayer = new GameObject("bgmplayer"+(count));
        count++;
        AudioSource source = create2DAudioSource(bgmplayer,bgm);
        source.volume = volume * BGMVolumeMultiplier;
        source.panStereo = pan;
        source.loop = looping;
        source.pitch = pitch;
        source.clip = bgm;
        source.time = time;
        source.Play();
        OnSoundChanged?.Invoke(SoundEvent.Start, source);
        bgs.Add(bgmplayer);
        bgvolumes.Add(volume);
    }
 
    public void StopBGM(int count)
    {
        if (bgs[count] != null)
        {
            bgs[count].GetComponent<AudioSource>().Stop();
        }
        
    }
    public void PauseBGM(int count)
    {
        bgs[count].GetComponent<AudioSource>().Pause();
        //pause = true;
    }

    public void PlaySFX2D(AudioClip clip, float volume = 1,bool looping = false, float pitch = 1f, float pan = 0)
    {
        flag = 2;
        GameObject sfxplayer = new GameObject("sfx");
        AudioSource source = sfxplayer.AddComponent<AudioSource>();
        /*yield return new WaitForSeconds(delay);
        source.Play();
        yield return new WaitForSeconds(sfx.length);*/
        source.clip = clip;
        source.Play();
        source.volume = volume * SFXVolumeMultiplier;
        source.loop = looping;
        source.pitch = pitch;
        source.panStereo = pan;
        if (OnSFXStarted != null)
            OnSFXStarted(source);
        sfxplayer.transform.parent = this.transform;
        players.Add(sfxplayer);
    }
    public void PlaySFX3D(Vector3 position, AudioClip clip, float volume = 1, bool looping = false, float pitch = 1f, float pan = 1)
    {
        flag = 3;
        GameObject sfxplayer = new GameObject("sfx");
        sfxplayer.transform.position = position;
        AudioSource s = sfxplayer.AddComponent<AudioSource>();
        s.volume = volume * SFXVolumeMultiplier;
        s.loop = looping;
        s.pitch = pitch;
        s.panStereo = pan;
        s.clip = clip;
        s.Play();
        sfxplayer.transform.parent = this.transform;
        players.Add(sfxplayer);
    }
    public void SetSFXVolume(float volume)
    {
        foreach (GameObject s in players)
        {
            s.GetComponent<AudioSource>().volume = volume * SFXVolumeMultiplier;
        }
    }
    public void SetBGMVolume(float volume,int count)
    {
        bgvolumes[count] = volume;
        bgs[count].GetComponent<AudioSource>().volume = bgvolumes[count] * BGMVolumeMultiplier;
    }
    public float GetBGMVolume(int count)
    {
        return bgs[count].GetComponent<AudioSource>().volume;
    }
    public float GetBGMTime(int count)
    {
        return bgs[count].GetComponent<AudioSource>().time;
    }
    AudioSource create2DAudioSource(GameObject bgm,AudioClip audioClip, float volume = 1, bool loop = false)
    {
        /*AudioSource src = gameObject.AddComponent<AudioSource>();

        src.clip = audioClip;
        src.volume = vol;
        src.loop = true;

        return src;*/
        if (curtime == 0)
        {
            curtime = Time.time;
        }
        AudioSource s;
        if (bgm == null)
            bgm = new GameObject("bgmplayer");
        bgm.transform.SetParent(this.transform);
        if (bgm.GetComponent<AudioSource>() == null)
        {
            s = bgm.AddComponent<AudioSource>();
        }
        else
        {
            s = bgm.GetComponent<AudioSource>();
        }
        s.clip = audioClip;
        s.loop = true;
        s.volume = volume;
        return s;
    }
}
