using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using FMOD;
using FMODUnity;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    public int testMode;
    private float time;
    private bool once = false;
    private int idx=0;
    public GameObject dd;
    private string song;
    public GameObject txt;
    public List<string> slist;
    private int cnt = 0;
    void Start()
    {
        slist = new List<string>();
        if (testMode == 1)
        {
            SoundManager.instance.PlayBGM(SoundManager.instance.bgmClips[0], 1f);
            SoundManager.instance.PlayBGM(SoundManager.instance.bgmClips[1], 0f);
        }
        else if(testMode == 2)
        {
            SoundManager.instance.PlayBGM(SoundManager.instance.bgmClips[0], 1f,true,1f,0f,38f);
        }
        else if (testMode == 3)
        {
            for(int i = 0; i < 100; i++)
            {
                if (i == 0)
                {
                    SoundManager.instance.PlayBGM(SoundManager.instance.bgmClips[0], 1f);
                }
                else
                {
                    SoundManager.instance.PlayBGM(SoundManager.instance.bgmClips[0], 0f);
                }
            }
            InvokeRepeating("jump", 0, 0.6f);
        }
        else if (testMode == 4)
        {
            SoundManager.instance.PlayBGM(SoundManager.instance.bgmClips[0], 1f);
        }
        else if (testMode == 5)
        {
            var musicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/ppp");
            musicInstance.start();
        }
        time = Time.time;
    }
    private void FixedUpdate()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time - time >= 40&&once==false&&testMode==1)
        {
            InvokeRepeating("changeBGM", 0, 0.01f);
            once = true;
        }
        else if (Time.time - time >= 40 && once == false && testMode == 4)
        {
            float tmp = SoundManager.instance.GetBGMTime(0);
            SoundManager.instance.PlayBGM(SoundManager.instance.bgmClips[0], 1f, true, 1f, 0f, tmp);
            SoundManager.instance.SetBGMVolume(0, 0);
            once = true;
        }
        else if (testMode == 6)
        {
            int tmp=dd.GetComponent<Dropdown>().value;
            song = dd.GetComponent<Dropdown>().options[tmp].text;
        }
    }
    public void playSong()
    {
        if (testMode == 6)
        {
            var musicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/" + song);
            musicInstance.start();
        }
        else
        {
            song = txt.GetComponent<InputField>().text;
            string[] sArray = Regex.Split(song, " ", RegexOptions.IgnoreCase);
            foreach (string i in sArray)
            {
                slist.Add(i);
            }
            InvokeRepeating("playPiano", 0, 0.6f);
        }
    }
    void playPiano()
    {
        var musicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/" + slist[cnt++]);
        musicInstance.start();
        if (cnt == slist.Count) CancelInvoke();
    }
    void changeBGM()
    {
        if (testMode == 1)
        {
            float m = SoundManager.instance.GetBGMVolume(0);
            m -= 0.01f;
            float n = SoundManager.instance.GetBGMVolume(1);
            n += 0.01f;
            SoundManager.instance.SetBGMVolume(m, 0);
            SoundManager.instance.SetBGMVolume(n, 1);
        }
    }
    void jump()
    {
        SoundManager.instance.SetBGMVolume(0, idx);
        idx++;
        if (idx == 100) idx = 0;
        SoundManager.instance.SetBGMVolume(1, idx);
    }
}
