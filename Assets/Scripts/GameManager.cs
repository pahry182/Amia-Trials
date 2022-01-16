using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public EnemySpawnManager _enemySpawnManager;
    public MainSceneController _msC;
    public UnitBase _player;
    public new Audio audio = new Audio();
    public Setting setting = new Setting();

    public bool isBattleStarted;
    public bool isEnemyPresent;
    public bool isRespawning;
    public bool isUpdatingVolume;
    public int currentWave;
    public int currentLife;
    public int setLife = 3;
    public int currentCheckpoint = 1;
    public int nextCheckpoint = 2;

    [Header("Statistics")]
    public int normalKillCount;
    public int bossKillCount;
    public float totalDamageDealt;
    public float highestDamageDealt;
    

    private void Awake()
    {
        _enemySpawnManager = GetComponent<EnemySpawnManager>();
        //DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            //Destroy(gameObject);
        }

    }

    void Start()
    {
        SetupAudio();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVolume();
    }

    public void StatisticTrackDamageDealt(float amount, GameObject _go)
    {
        if (_go.tag == "Player")
        {
            GameManager.Instance.totalDamageDealt += amount;
            if (GameManager.Instance.highestDamageDealt < amount)
            {
                GameManager.Instance.highestDamageDealt = amount;
            }
        }
    }

    public void StatisticTrackKill(UnitBase target, GameObject _go)
    {
        if (_go.tag == "Player")
        {
            if (target.isBoss)
            {
                GameManager.Instance.bossKillCount++;
            }
            else
            {
                GameManager.Instance.normalKillCount++;
            }
        }
    }

    public void CheckDeath(GameObject _target)
    {
        if (_target.tag == "Enemy")
        {
            isEnemyPresent = false;
        }
        else if (_target.tag == "Player")
        {
            if (currentLife >= 1)
            {
                print("defeated");
            }
            else
            {
                //isPlayerDefeated = true;
                _msC.RespawnButton.interactable = false;
            }
            StartCoroutine(_msC.GameOver());
        }
    }

    public void ResetState()
    {
        _player.ReviveUnit();
        _enemySpawnManager.RemoveEnemy();
        _msC.RespawnButton.interactable = true;
        isEnemyPresent = false;
        isBattleStarted = false;
        currentWave = 0;
        currentLife = setLife;
        currentCheckpoint = 1;
        nextCheckpoint = 2;
        normalKillCount = 0;
        bossKillCount = 0;
        totalDamageDealt = 0;
        highestDamageDealt = 0;
    }

    public void RespawnCheckpoint()
    {
        _player.ReviveUnit();
        currentLife--;
        currentWave = currentCheckpoint;
        isRespawning = true;
        _enemySpawnManager.RemoveEnemy();
    }

    public void SetCheckpoint()
    {
        if (currentWave == nextCheckpoint)
        {
            nextCheckpoint += currentCheckpoint;
            currentCheckpoint = currentWave;
        }
    }

    public void PlaySfx(string name)
    {
        Sound sfx = Array.Find(audio.soundEffects, sound => sound.name == name);
        if (sfx == null)
        {
            print("Audio " + name + " not found!!");
            return;
        }

        sfx.audioSource.Play();
    }

    public void PlayBgm(string name)
    {
        Sound bgm = Array.Find(audio.backgroundMusics, sound => sound.name == name);
        if (bgm == null)
        {
            print("Audio " + name + " not found!!");
            return;
        }

        for (int i = 0; i < audio.backgroundMusics.Length; i++)
        {
            audio.backgroundMusics[i].audioSource.Stop();
        }

        bgm.audioSource.Play();
    }

    private void UpdateVolume()
    {
        if (!isUpdatingVolume) return;
        print("pong");
        for (int i = 0; i < audio.activeSfx.Count; i++)
        {
            audio.activeSfx[i].volume = setting.SfxVolume;
        }

        for (int i = 0; i < audio.activeBgm.Count; i++)
        {
            audio.activeBgm[i].volume = setting.BgmVolume;
        }
    }

    private void SetupAudio()
    {
        for (int i = 0; i < audio.soundEffects.Length; i++)
        {
            audio.soundEffects[i].audioSource = gameObject.AddComponent<AudioSource>();
            audio.soundEffects[i].audioSource.clip = audio.soundEffects[i].clip;
            audio.soundEffects[i].audioSource.volume = audio.soundEffects[i].volume;
            audio.soundEffects[i].audioSource.pitch = audio.soundEffects[i].pitch;
            audio.soundEffects[i].audioSource.loop = audio.soundEffects[i].loop;
            audio.activeSfx.Add(audio.soundEffects[i].audioSource);
        }

        for (int i = 0; i < audio.backgroundMusics.Length; i++)
        {
            audio.backgroundMusics[i].audioSource = gameObject.AddComponent<AudioSource>();
            audio.backgroundMusics[i].audioSource.clip = audio.backgroundMusics[i].clip;
            audio.backgroundMusics[i].audioSource.volume = audio.backgroundMusics[i].volume;
            audio.backgroundMusics[i].audioSource.pitch = audio.backgroundMusics[i].pitch;
            audio.backgroundMusics[i].audioSource.loop = audio.backgroundMusics[i].loop;
            audio.activeBgm.Add(audio.backgroundMusics[i].audioSource);
        }
    }

    [Serializable]
    public class Audio
    {
        public Sound[] soundEffects;
        public Sound[] backgroundMusics;

        [HideInInspector] public List<AudioSource> activeSfx = new List<AudioSource>();
        [HideInInspector] public List<AudioSource> activeBgm = new List<AudioSource>();
    }

    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
        [Range(0.1f, 3f)] public float pitch;
        public bool loop;
        [HideInInspector] public AudioSource audioSource;
    }

    public class Setting
    {
        public float SfxVolume
        {
            get { return PlayerPrefs.GetFloat("sfxVolume"); }
            set { PlayerPrefs.SetFloat("sfxVolume", value); }
        }

        public float BgmVolume
        {
            get { return PlayerPrefs.GetFloat("bgmVolume"); }
            set { PlayerPrefs.SetFloat("bgmVolume", value); }
        }

        public float SfxStatus
        {
            get { return PlayerPrefs.GetInt("sfxStatus"); }
            set { PlayerPrefs.SetInt("sfxStatus", (int)value); }
        }

        public float BgmStatus
        {
            get { return PlayerPrefs.GetInt("bgmStatus"); }
            set { PlayerPrefs.SetInt("bgmStatus", (int)value); }
        }
    }

}
