using UnityEngine;

public class MusicSequencer : MonoBehaviour
{
    [Header("Clips de Audio")]
    public AudioClip introClip;
    public AudioClip loopClip;

    [Header("Configuración de Tiempo")]
    [Tooltip("Escribe aquí la duración EXACTA de la intro en segundos (ej: 12.455)")]
    public double exactIntroDuration = 10.0;

    // Dos fuentes para mezcla perfecta
    private AudioSource introSource;
    private AudioSource loopSource;

    void Start()
    {
        // 1. Configurar AudioSources (Intro y Loop)
        introSource = GetComponent<AudioSource>();
        if (introSource == null) introSource = gameObject.AddComponent<AudioSource>();

        loopSource = gameObject.AddComponent<AudioSource>();
        
        // Copiar ajustes de volumen
        loopSource.outputAudioMixerGroup = introSource.outputAudioMixerGroup;
        loopSource.volume = introSource.volume;
        loopSource.spatialBlend = introSource.spatialBlend;

        if (introClip != null && loopClip != null)
        {
            PlayManualSequence();
        }
    }

    void PlayManualSequence()
    {
        // Asignar clips
        introSource.clip = introClip;
        loopSource.clip = loopClip;

        // Configurar Loop
        introSource.loop = false;
        loopSource.loop = true;

        // --- SISTEMA DE TIEMPO PRECISO ---
        
        // Momento actual del motor de audio + pequeño buffer (0.1s) para que le de tiempo a cargar
        double startTime = AudioSettings.dspTime + 0.1;

        // El loop empieza EXACTAMENTE cuando tú dijiste
        double loopStartTime = startTime + exactIntroDuration;

        // Agendar reproducción
        introSource.PlayScheduled(startTime);
        loopSource.PlayScheduled(loopStartTime);
        
        Debug.Log($"DAM SURVIVORS: Intro programada. El loop entrará a los {exactIntroDuration} segundos exactos.");
    }
}