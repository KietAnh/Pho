using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioManager : SingletonTemplate<AudioManager>
{
    public float globalSoundVolume = 1f;
    public float globalMusicVolume = 1f;
    public float globalVibra = 1f;

    public bool isInit;

    private GameObject soundGroup;

    private long _idxNow;
    private Dictionary<long, AudioData> _soundPlayingMap = new Dictionary<long, AudioData>();

    private const int MAX_CACHE_SOURCE = 10;
    private Stack<AudioSource> _sourceCache = new Stack<AudioSource>();
    public void Init()
    {
        isInit = true;

        soundGroup = new GameObject();
        soundGroup.name = "[SoundGroup]";
        GameObject.DontDestroyOnLoad(soundGroup);

        SetGlobalVolume(null);
        SetGlobalVibra(null);

        GED.ResED.addListener(EventID.ResTurnMusic, SetGlobalVolume);
        GED.ResED.addListener(EventID.ResTurnSound, SetGlobalVolume);
        GED.ResED.addListener(EventID.ResTurnVibra, SetGlobalVibra);
    }
    public void OnDestroy()      // refactor, never call
    {
        GED.ResED.removeListener(EventID.ResTurnMusic, SetGlobalVolume);
        GED.ResED.removeListener(EventID.ResTurnSound, SetGlobalVolume);
        GED.ResED.removeListener(EventID.ResTurnVibra, SetGlobalVibra);
    }
    public void SetGlobalVolume(GameEvent gameEvent)
    {
        //bool isMusicOn = UserDataService.GetData<bool>(PREF_KEY.MusicOn, true);
        //globalMusicVolume = isMusicOn ? 1f : 0f;
        //bool isSoundOn = UserDataService.GetData<bool>(PREF_KEY.SoundOn, true);
        //globalSoundVolume = isSoundOn ? 1f : 0f;

        //foreach (var sound in _soundPlayingMap)
        //{
        //    AudioData soundData = sound.Value;
        //    if (soundData.sndType == SoundType.Music || soundData.sndType == SoundType.Music3D)
        //    {
        //        soundData.source.volume = soundData.baseVolume * globalMusicVolume;
        //    }
        //    else
        //    {
        //        soundData.source.volume = soundData.baseVolume * globalSoundVolume;
        //    }
        //}
    }
    public void SetGlobalVibra(GameEvent gameEvent)
    {
        //bool isVibraOn = UserDataService.GetData<bool>(PREF_KEY.VibraOn, true);
        //globalVibra = isVibraOn? 1f : 0f;
    }

    public long PlayMusic(string name, float volume = 1f)
    {
        return PlaySound(name, SoundType.Music, volume, globalMusicVolume * volume, true);
    }
    public long PlayUIAudio(string name, float volume = 1f)
    {
        return PlaySound(name, SoundType.UIAudio, volume, globalSoundVolume * volume, false);
    }
    public long PlayEffect(string name, float volume = 1f)
    {
        return PlaySound(name, SoundType.Effect, volume, globalSoundVolume * volume, false);
    }
    private long PlaySound(string name, SoundType sndType, float baseVolume, float volume, bool isLoop)
    {
        long id = _NewId();
        AudioData audioData = new AudioData();
        audioData.playing = true;
        audioData.sndType = sndType;
        audioData.baseVolume = baseVolume;
        audioData.source = GetAudioSource(name);
        audioData.source.clip = GetClip(name, sndType);
        audioData.source.volume = volume;
        audioData.source.loop = isLoop;

        audioData.source.gameObject.SetActive(true);

        _soundPlayingMap.Add(id, audioData);

        audioData.source.Play();
        if (!isLoop)
        {
            CoroutineManager.Singleton.delayedCall(audioData.source.clip.length - audioData.source.time + 0.1f, () =>
            {
                Stop(id);
            });
        }

        return id;
    }

    private AudioSource GetAudioSource(string name)
    {
        AudioSource audio = null;
        if (_sourceCache.Count > 0)
        {
            audio = _sourceCache.Pop();
        }
        else
        {
            GameObject audioObj = new GameObject();
            audioObj.name = name;
            audioObj.transform.SetParent(soundGroup.transform);
            audio = audioObj.AddComponent<AudioSource>();
            audio.playOnAwake = false;
        }
        return audio;
    }
    private AudioClip GetClip(string name, SoundType sndType)
    {
        AudioClip audioClip = null;
        //if (sndType == SoundType.Music)
        //{
        //    string abName = string.Format("aud_{0}", name);
        //    audioClip = AssetLoadManager.LoadAsset<AudioClip>(abName, name);      // refactor, constant
        //}
        //else
        //{
        //    audioClip = AssetLoadManager.LoadAsset<AudioClip>("aud_s_clip", name);      // refactor, constant
        //}

        return audioClip;
    }
    public void Stop(long id)
    {
        AudioSource source = _soundPlayingMap[id].source;
        if (source == null)
            return;

        if (source.isPlaying)
        {
            source.Stop();
        }
        source.gameObject.SetActive(false);
        _soundPlayingMap.Remove(id);

        if (_sourceCache.Count < MAX_CACHE_SOURCE) 
        {
            _sourceCache.Push(source);
        }
        else
        {
            GameObject.Destroy(source.gameObject);
        }
    }

    private long _NewId()
    {
        while (true)
        {
            _idxNow++;
            if (_idxNow < 0)        // case overflow kieu long
                _idxNow = 1;
            if (!_soundPlayingMap.ContainsKey(_idxNow))
                return _idxNow;
        }
    }
}

public class AudioData
{
    public bool playing;
    public AudioSource source;
    public Action callback;
    public SoundType sndType = SoundType.Music;
    public float baseVolume;
}

public enum SoundType
{
    Music = 1,
    UIAudio,
    Effect,         
    Voice,          
    Music3D,       
    Effect3D,       
}