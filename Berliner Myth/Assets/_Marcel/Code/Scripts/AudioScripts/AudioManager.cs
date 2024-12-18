using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;
//https://www.youtube.com/watch?v=rcBHIOjZDpk&ab_channel=ShapedbyRainStudios

public class AudioManager : MonoBehaviour
{
    public OverallApprovalManager OverallApprovalManagerScript;
    [Header("Area")]
    [SerializeField] private EscalationCurve area;

    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1;
    [Range(0, 1)]
    public float musicVolume = 1;
    [Range(0, 1)]
    public float ambienceVolume = 1;
    [Range(0, 1)]
    public float SFXVolume = 1;

    public string currentParameter;
    public float finalParameter;

    private Bus masterBus;
    private Bus musicBus;
    private Bus ambienceBus;
    private Bus sfxBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private EventInstance ambienceEventInstance;
    private EventInstance ambientEventInstance;
    public EventInstance musicEventInstance;

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        //masterBus = RuntimeManager.GetBus("bus:/");
        //musicBus = RuntimeManager.GetBus("bus:/Music");
        //ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        //sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void Start()
    {
        MusicForThisScene();

        if (SceneManager.GetActiveScene().name == "Game")
        {
            InitializeMusic(FMODEvents.instance.ST_InGame);
        }
    }

    private void Update()
    {
        //masterBus.setVolume(masterVolume);
        //musicBus.setVolume(musicVolume);
        //ambienceBus.setVolume(ambienceVolume);
        //sfxBus.setVolume(SFXVolume);
    }

    private void InitializeAmbient(EventReference ambientEventReference)
    {
        ambientEventInstance = CreateInstance(ambientEventReference);
        ambientEventInstance.start();
    }

    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }

    public void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    public void SetMusicParameter()
    {
        musicEventInstance.setParameterByName("EscalationCurve", finalParameter);
    }

    public void SetMusicArea(EscalationCurve area)
    {
        musicEventInstance.setParameterByName("EscalationCurve", (float)area);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    private void CleanUp()
    {
        // stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        // stop all of the event emitters, because if we don't they may hang around in other scenes
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }

	#region music
	private void MusicForThisScene()
	{
        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            InitializeAmbient(FMODEvents.instance.EndingBackgroundNoise);
            InitializeMusic(FMODEvents.instance.ST_TitleScreen);
        }

        if (SceneManager.GetActiveScene().name == "Game")
        {
            InitializeAmbience(FMODEvents.instance.BackgroundNoise);
        }

        if (SceneManager.GetActiveScene().name == "EndingScene1")
        {
            InitializeMusic(FMODEvents.instance.ST_Ending1);
            InitializeAmbient(FMODEvents.instance.EndingBackgroundNoise);
            instance.PlayOneShot(FMODEvents.instance.VL_Player_Dies, this.transform.position);
        }

        if (SceneManager.GetActiveScene().name == "EndingScene2")
        {
            InitializeMusic(FMODEvents.instance.ST_Ending2);
            InitializeAmbient(FMODEvents.instance.EndingBackgroundNoise);
        }

        if (SceneManager.GetActiveScene().name == "EndingScene3_blue")
        {
            InitializeMusic(FMODEvents.instance.ST_Ending3);
            InitializeAmbient(FMODEvents.instance.EndingBackgroundNoise);
        }

        if (SceneManager.GetActiveScene().name == "EndingScene3_red")
        {
            InitializeMusic(FMODEvents.instance.ST_Ending3);
            InitializeAmbient(FMODEvents.instance.EndingBackgroundNoise);
        }
    }
    #endregion
}