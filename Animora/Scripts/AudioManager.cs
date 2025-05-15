using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    
    [Range(0f, 1f)]
    public float volume = 1f;
    
    [Range(0.1f, 3f)]
    public float pitch = 1f;
    
    public bool loop = false;
    
    [Range(0f, 1f)]
    public float spatialBlend = 0f; // 0 = 2D, 1 = 3D
    
    [HideInInspector]
    public AudioSource source;
    
    public Sound()
    {
        name = "New Sound";
        volume = 1f;
        pitch = 1f;
        loop = false;
        spatialBlend = 0f;
    }
}

[System.Serializable]
public class MusicTrack
{
    public string name;
    public AudioClip clip;
    
    [Range(0f, 1f)]
    public float volume = 0.5f;
    
    public bool loop = true;
    
    [Range(0f, 10f)]
    public float fadeInTime = 2f;
    
    [Range(0f, 10f)]
    public float fadeOutTime = 2f;
    
    [HideInInspector]
    public AudioSource source;
}

[System.Serializable]
public class LocationMusic
{
    public string locationName;
    public string dayMusicTrackName;
    public string nightMusicTrackName;
}

[System.Serializable]
public class WeatherAmbience
{
    public Weather weatherType;
    public string ambienceTrackName;
    public float ambienceVolume = 0.3f;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource ambienceSource;
    public AudioSource uiSoundSource;
    public Transform soundPoolParent;
    
    [Header("Audio Mixers")]
    public AudioMixer audioMixer;
    public string masterVolumeParam = "MasterVolume";
    public string musicVolumeParam = "MusicVolume";
    public string sfxVolumeParam = "SFXVolume";
    public string ambienceVolumeParam = "AmbienceVolume";
    public string uiVolumeParam = "UIVolume";
    
    [Header("Sound Effects")]
    public List<Sound> sounds = new List<Sound>();
    
    [Header("Music")]
    public List<MusicTrack> musicTracks = new List<MusicTrack>();
    public List<LocationMusic> locationMusic = new List<LocationMusic>();
    
    [Header("Ambience")]
    public List<Sound> ambienceSounds = new List<Sound>();
    public List<WeatherAmbience> weatherAmbience = new List<WeatherAmbience>();
    
    [Header("UI Sounds")]
    public Sound buttonClickSound;
    public Sound notificationSound;
    public Sound successSound;
    public Sound failureSound;
    public Sound purchaseSound;
    
    [Header("Pet Sounds")]
    public List<Sound> dogSounds = new List<Sound>();
    public List<Sound> catSounds = new List<Sound>();
    public List<Sound> dragonSounds = new List<Sound>();
    public List<Sound> unicornSounds = new List<Sound>();
    
    [Header("Settings")]
    public bool playMusicOnStart = true;
    public string defaultMusicTrack = "MainTheme";
    public float minTimeBetweenSameSounds = 0.1f;
    
    // Private variables
    private Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();
    private Dictionary<string, MusicTrack> musicDictionary = new Dictionary<string, MusicTrack>();
    private Dictionary<PetType, List<Sound>> petSoundDictionary = new Dictionary<PetType, List<Sound>>();
    private Dictionary<string, float> lastPlayTimes = new Dictionary<string, float>();
    
    private MusicTrack currentMusicTrack;
    private Coroutine musicFadeCoroutine;
    private Coroutine ambienceFadeCoroutine;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Create sound pool parent if not assigned
        if (soundPoolParent == null)
        {
            GameObject poolObj = new GameObject("SoundPool");
            poolObj.transform.SetParent(transform);
            soundPoolParent = poolObj.transform;
        }
        
        // Initialize audio sources if not assigned
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.spatialBlend = 0f; // 2D sound
        }
        
        if (ambienceSource == null)
        {
            GameObject ambienceObj = new GameObject("AmbienceSource");
            ambienceObj.transform.SetParent(transform);
            ambienceSource = ambienceObj.AddComponent<AudioSource>();
            ambienceSource.playOnAwake = false;
            ambienceSource.loop = true;
            ambienceSource.spatialBlend = 0f; // 2D sound
        }
        
        if (uiSoundSource == null)
        {
            GameObject uiObj = new GameObject("UISoundSource");
            uiObj.transform.SetParent(transform);
            uiSoundSource = uiObj.AddComponent<AudioSource>();
            uiSoundSource.playOnAwake = false;
            uiSoundSource.loop = false;
            uiSoundSource.spatialBlend = 0f; // 2D sound
        }
        
        // Initialize dictionaries
        InitializeSoundDictionaries();
        
        // Set up audio mixer if available
        if (audioMixer != null)
        {
            // Load saved volume settings
            LoadVolumeSettings();
        }
    }
    
    private void Start()
    {
        // Subscribe to game events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLocationChanged += OnLocationChanged;
            GameManager.Instance.OnTimeOfDayChanged += OnTimeOfDayChanged;
            GameManager.Instance.OnWeatherChanged += OnWeatherChanged;
        }
        
        // Play default music
        if (playMusicOnStart && !string.IsNullOrEmpty(defaultMusicTrack))
        {
            PlayMusic(defaultMusicTrack);
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLocationChanged -= OnLocationChanged;
            GameManager.Instance.OnTimeOfDayChanged -= OnTimeOfDayChanged;
            GameManager.Instance.OnWeatherChanged -= OnWeatherChanged;
        }
    }
    
    private void InitializeSoundDictionaries()
    {
        // Initialize sound dictionary
        soundDictionary.Clear();
        foreach (Sound sound in sounds)
        {
            if (!string.IsNullOrEmpty(sound.name) && sound.clip != null)
            {
                soundDictionary[sound.name] = sound;
            }
        }
        
        // Initialize music dictionary
        musicDictionary.Clear();
        foreach (MusicTrack track in musicTracks)
        {
            if (!string.IsNullOrEmpty(track.name) && track.clip != null)
            {
                musicDictionary[track.name] = track;
            }
        }
        
        // Initialize pet sound dictionary
        petSoundDictionary.Clear();
        petSoundDictionary[PetType.Dog] = dogSounds;
        petSoundDictionary[PetType.Cat] = catSounds;
        petSoundDictionary[PetType.Dragon] = dragonSounds;
        petSoundDictionary[PetType.Unicorn] = unicornSounds;
        
        // Add UI sounds to dictionary
        if (buttonClickSound != null && buttonClickSound.clip != null)
        {
            soundDictionary["ButtonClick"] = buttonClickSound;
        }
        
        if (notificationSound != null && notificationSound.clip != null)
        {
            soundDictionary["Notification"] = notificationSound;
        }
        
        if (successSound != null && successSound.clip != null)
        {
            soundDictionary["Success"] = successSound;
        }
        
        if (failureSound != null && failureSound.clip != null)
        {
            soundDictionary["Failure"] = failureSound;
        }
        
        if (purchaseSound != null && purchaseSound.clip != null)
        {
            soundDictionary["Purchase"] = purchaseSound;
        }
        
        // Initialize ambience sounds
        foreach (Sound ambience in ambienceSounds)
        {
            if (!string.IsNullOrEmpty(ambience.name) && ambience.clip != null)
            {
                soundDictionary[ambience.name] = ambience;
            }
        }
    }
    
    // Play a sound by name
    public AudioSource PlaySound(string soundName, Vector3 position = default, Transform parent = null)
    {
        if (!soundDictionary.ContainsKey(soundName))
        {
            Debug.LogWarning($"Sound '{soundName}' not found!");
            return null;
        }
        
        // Check if the sound was played very recently
        if (lastPlayTimes.ContainsKey(soundName))
        {
            float timeSinceLastPlay = Time.time - lastPlayTimes[soundName];
            if (timeSinceLastPlay < minTimeBetweenSameSounds)
            {
                return null;
            }
        }
        
        Sound sound = soundDictionary[soundName];
        
        // Create a new GameObject for the sound
        GameObject soundObj = new GameObject($"Sound_{soundName}");
        
        // Set position and parent
        if (position != default)
        {
            soundObj.transform.position = position;
        }
        
        if (parent != null)
        {
            soundObj.transform.SetParent(parent);
        }
        else
        {
            soundObj.transform.SetParent(soundPoolParent);
        }
        
        // Add AudioSource component
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        
        // Configure AudioSource
        audioSource.clip = sound.clip;
        audioSource.volume = sound.volume;
        audioSource.pitch = sound.pitch;
        audioSource.loop = sound.loop;
        audioSource.spatialBlend = sound.spatialBlend;
        
        // Set output mixer group if available
        if (audioMixer != null)
        {
            audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        }
        
        // Play the sound
        audioSource.Play();
        
        // Record the play time
        lastPlayTimes[soundName] = Time.time;
        
        // Destroy the GameObject when the sound is done playing
        if (!sound.loop)
        {
            Destroy(soundObj, sound.clip.length + 0.1f);
        }
        
        return audioSource;
    }
    
    // Play a UI sound
    public void PlayUISound(string soundName)
    {
        if (!soundDictionary.ContainsKey(soundName))
        {
            Debug.LogWarning($"UI sound '{soundName}' not found!");
            return;
        }
        
        Sound sound = soundDictionary[soundName];
        
        // Configure UI sound source
        uiSoundSource.clip = sound.clip;
        uiSoundSource.volume = sound.volume;
        uiSoundSource.pitch = sound.pitch;
        
        // Set output mixer group if available
        if (audioMixer != null)
        {
            uiSoundSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("UI")[0];
        }
        
        // Play the sound
        uiSoundSource.Play();
    }
    
    // Play a pet sound
    public void PlayPetSound(PetType petType, Vector3 position)
    {
        if (!petSoundDictionary.ContainsKey(petType) || petSoundDictionary[petType].Count == 0)
        {
            return;
        }
        
        List<Sound> petSounds = petSoundDictionary[petType];
        
        // Pick a random sound
        Sound sound = petSounds[Random.Range(0, petSounds.Count)];
        
        // Play the sound
        PlaySound(sound.name, position);
    }
    
    // Play music
    public void PlayMusic(string trackName, bool fadeIn = true)
    {
        if (!musicDictionary.ContainsKey(trackName))
        {
            Debug.LogWarning($"Music track '{trackName}' not found!");
            return;
        }
        
        MusicTrack track = musicDictionary[trackName];
        
        // If the same track is already playing, do nothing
        if (currentMusicTrack != null && currentMusicTrack.name == trackName && musicSource.isPlaying)
        {
            return;
        }
        
        // Stop any current fade coroutine
        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
        }
        
        // Fade out current music and fade in new music
        if (fadeIn && musicSource.isPlaying)
        {
            float fadeOutTime = currentMusicTrack != null ? currentMusicTrack.fadeOutTime : 1f;
            musicFadeCoroutine = StartCoroutine(CrossfadeMusicTracks(track, fadeOutTime, track.fadeInTime));
        }
        else
        {
            // Stop current music
            musicSource.Stop();
            
            // Configure music source
            musicSource.clip = track.clip;
            musicSource.volume = track.volume;
            musicSource.loop = track.loop;
            
            // Set output mixer group if available
            if (audioMixer != null)
            {
                musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
            }
            
            // Play the music
            musicSource.Play();
        }
        
        currentMusicTrack = track;
    }
    
    // Play ambience
    public void PlayAmbience(string ambienceName, bool fadeIn = true)
    {
        if (!soundDictionary.ContainsKey(ambienceName))
        {
            Debug.LogWarning($"Ambience sound '{ambienceName}' not found!");
            return;
        }
        
        Sound ambience = soundDictionary[ambienceName];
        
        // If the same ambience is already playing, do nothing
        if (ambienceSource.clip == ambience.clip && ambienceSource.isPlaying)
        {
            return;
        }
        
        // Stop any current fade coroutine
        if (ambienceFadeCoroutine != null)
        {
            StopCoroutine(ambienceFadeCoroutine);
        }
        
        // Fade out current ambience and fade in new ambience
        if (fadeIn && ambienceSource.isPlaying)
        {
            ambienceFadeCoroutine = StartCoroutine(CrossfadeAmbience(ambience, 2f, 2f));
        }
        else
        {
            // Stop current ambience
            ambienceSource.Stop();
            
            // Configure ambience source
            ambienceSource.clip = ambience.clip;
            ambienceSource.volume = ambience.volume;
            ambienceSource.loop = true;
            
            // Set output mixer group if available
            if (audioMixer != null)
            {
                ambienceSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Ambience")[0];
            }
            
            // Play the ambience
            ambienceSource.Play();
        }
    }
    
    // Crossfade between music tracks
    private IEnumerator CrossfadeMusicTracks(MusicTrack newTrack, float fadeOutTime, float fadeInTime)
    {
        // Store the current volume
        float startVolume = musicSource.volume;
        
        // Fade out current music
        float timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeOutTime);
            yield return null;
        }
        
        // Stop current music
        musicSource.Stop();
        
        // Configure music source for new track
        musicSource.clip = newTrack.clip;
        musicSource.volume = 0f;
        musicSource.loop = newTrack.loop;
        
        // Set output mixer group if available
        if (audioMixer != null)
        {
            musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
        }
        
        // Play the new music
        musicSource.Play();
        
        // Fade in new music
        timer = 0f;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, newTrack.volume, timer / fadeInTime);
            yield return null;
        }
        
        // Ensure final volume is set correctly
        musicSource.volume = newTrack.volume;
        
        musicFadeCoroutine = null;
    }
    
    // Crossfade between ambience sounds
    private IEnumerator CrossfadeAmbience(Sound newAmbience, float fadeOutTime, float fadeInTime)
    {
        // Store the current volume
        float startVolume = ambienceSource.volume;
        
        // Fade out current ambience
        float timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            ambienceSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeOutTime);
            yield return null;
        }
        
        // Stop current ambience
        ambienceSource.Stop();
        
        // Configure ambience source for new sound
        ambienceSource.clip = newAmbience.clip;
        ambienceSource.volume = 0f;
        ambienceSource.loop = true;
        
        // Set output mixer group if available
        if (audioMixer != null)
        {
            ambienceSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Ambience")[0];
        }
        
        // Play the new ambience
        ambienceSource.Play();
        
        // Fade in new ambience
        timer = 0f;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            ambienceSource.volume = Mathf.Lerp(0f, newAmbience.volume, timer / fadeInTime);
            yield return null;
        }
        
        // Ensure final volume is set correctly
        ambienceSource.volume = newAmbience.volume;
        
        ambienceFadeCoroutine = null;
    }
    
    // Event handlers
    
    private void OnLocationChanged(string locationName)
    {
        // Find the appropriate music for this location
        LocationMusic locMusic = locationMusic.Find(lm => lm.locationName == locationName);
        
        if (locMusic != null)
        {
            // Play day or night music based on current time of day
            bool isNight = GameManager.Instance.currentTimeOfDay == TimeOfDay.Night;
            string trackName = isNight ? locMusic.nightMusicTrackName : locMusic.dayMusicTrackName;
            
            if (!string.IsNullOrEmpty(trackName))
            {
                PlayMusic(trackName);
            }
        }
    }
    
    private void OnTimeOfDayChanged(TimeOfDay timeOfDay)
    {
        // Update music based on current location and new time of day
        string currentLocation = GameManager.Instance.currentLocation;
        LocationMusic locMusic = locationMusic.Find(lm => lm.locationName == currentLocation);
        
        if (locMusic != null)
        {
            // Play day or night music based on new time of day
            bool isNight = timeOfDay == TimeOfDay.Night;
            string trackName = isNight ? locMusic.nightMusicTrackName : locMusic.dayMusicTrackName;
            
            if (!string.IsNullOrEmpty(trackName))
            {
                PlayMusic(trackName);
            }
        }
    }
    
    private void OnWeatherChanged(Weather weather)
    {
        // Find the appropriate ambience for this weather
        WeatherAmbience weatherAmb = weatherAmbience.Find(wa => wa.weatherType == weather);
        
        if (weatherAmb != null && !string.IsNullOrEmpty(weatherAmb.ambienceTrackName))
        {
            PlayAmbience(weatherAmb.ambienceTrackName);
            
            // Adjust ambience volume
            ambienceSource.volume = weatherAmb.ambienceVolume;
        }
        else
        {
            // Stop ambience if no specific track for this weather
            ambienceSource.Stop();
        }
    }
    
    // Volume control
    
    public void SetMasterVolume(float volume)
    {
        if (audioMixer != null)
        {
            // Convert from linear (0-1) to decibels (-80-0)
            float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
            audioMixer.SetFloat(masterVolumeParam, dB);
            
            // Save setting
            PlayerPrefs.SetFloat("MasterVolume", volume);
            PlayerPrefs.Save();
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
        {
            // Convert from linear (0-1) to decibels (-80-0)
            float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
            audioMixer.SetFloat(musicVolumeParam, dB);
            
            // Save setting
            PlayerPrefs.SetFloat("MusicVolume", volume);
            PlayerPrefs.Save();
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        if (audioMixer != null)
        {
            // Convert from linear (0-1) to decibels (-80-0)
            float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
            audioMixer.SetFloat(sfxVolumeParam, dB);
            
            // Save setting
            PlayerPrefs.SetFloat("SFXVolume", volume);
            PlayerPrefs.Save();
        }
    }
    
    public void SetAmbienceVolume(float volume)
    {
        if (audioMixer != null)
        {
            // Convert from linear (0-1) to decibels (-80-0)
            float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
            audioMixer.SetFloat(ambienceVolumeParam, dB);
            
            // Save setting
            PlayerPrefs.SetFloat("AmbienceVolume", volume);
            PlayerPrefs.Save();
        }
    }
    
    public void SetUIVolume(float volume)
    {
        if (audioMixer != null)
        {
            // Convert from linear (0-1) to decibels (-80-0)
            float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
            audioMixer.SetFloat(uiVolumeParam, dB);
            
            // Save setting
            PlayerPrefs.SetFloat("UIVolume", volume);
            PlayerPrefs.Save();
        }
    }
    
    private void LoadVolumeSettings()
    {
        if (audioMixer != null)
        {
            // Load saved volume settings or use defaults
            float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
            float ambienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 0.7f);
            float uiVolume = PlayerPrefs.GetFloat("UIVolume", 0.8f);
            
            // Apply settings
            SetMasterVolume(masterVolume);
            SetMusicVolume(musicVolume);
            SetSFXVolume(sfxVolume);
            SetAmbienceVolume(ambienceVolume);
            SetUIVolume(uiVolume);
        }
    }
    
    // Utility methods
    
    public bool IsMusicPlaying()
    {
        return musicSource != null && musicSource.isPlaying;
    }
    
    public bool IsAmbiencePlaying()
    {
        return ambienceSource != null && ambienceSource.isPlaying;
    }
    
    public void StopAllSounds()
    {
        // Stop music
        if (musicSource != null)
        {
            musicSource.Stop();
        }
        
        // Stop ambience
        if (ambienceSource != null)
        {
            ambienceSource.Stop();
        }
        
        // Stop UI sounds
        if (uiSoundSource != null)
        {
            uiSoundSource.Stop();
        }
        
        // Destroy all sound objects in the pool
        foreach (Transform child in soundPoolParent)
        {
            Destroy(child.gameObject);
        }
    }
    
    public void PauseAllSounds()
    {
        // Pause music
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
        
        // Pause ambience
        if (ambienceSource != null && ambienceSource.isPlaying)
        {
            ambienceSource.Pause();
        }
        
        // Pause all sound objects in the pool
        foreach (Transform child in soundPoolParent)
        {
            AudioSource source = child.GetComponent<AudioSource>();
            if (source != null && source.isPlaying)
            {
                source.Pause();
            }
        }
    }
    
    public void UnpauseAllSounds()
    {
        // Unpause music
        if (musicSource != null && !musicSource.isPlaying && musicSource.clip != null)
        {
            musicSource.UnPause();
        }
        
        // Unpause ambience
        if (ambienceSource != null && !ambienceSource.isPlaying && ambienceSource.clip != null)
        {
            ambienceSource.UnPause();
        }
        
        // Unpause all sound objects in the pool
        foreach (Transform child in soundPoolParent)
        {
            AudioSource source = child.GetComponent<AudioSource>();
            if (source != null && !source.isPlaying && source.clip != null)
            {
                source.UnPause();
            }
        }
    }
}