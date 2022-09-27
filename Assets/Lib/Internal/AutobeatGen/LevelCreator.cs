using System;
using System.Collections.Generic;
using DSPLib;
using UnityEngine;
using Complex = System.Numerics.Complex;
using UnityEngine.Tilemaps;

public class LevelCreator : MonoBehaviour
{
	public Animator WorldAnimator;                                                                      //JAKOB SCRIPT JAKOB SCRIPT JAKOB SCRIPT JAKOB SCRIPT

	public GameObject Player;
	public GameObject Collectable;
	public GameObject Obstacles;
	public GameObject Background;
	private bool LevelCreated;
    // Start is called before the first frame update
    private AudioSource audioSource;
    private Tilemap map;
    private int numChannels;
    private int numTotalSamples;
    private int sampleRate;
    private float clipLength;
    private float[] multiChannelSamples;
    private SpectralFluxAnalyzer preProcessedSpectralFluxAnalyzer;
    private List<SpectralFluxInfo> pointInfo;
    private Vector3 originalPos;
    private GameObject playerInScene;
    private float LevelLength;
    private bool finished = false;
    private List<GameObject> levelObj = new List<GameObject>();
    enum Interacable
    {
	    Obstacles, Collectable
    };
    public void Awake()
    {
	    originalPos = transform.position;
	    audioSource = GetComponent<AudioSource>();
	    map = GetComponentInChildren<Tilemap>();
    }

    public void CreateLevel()
    {
	    preProcessedSpectralFluxAnalyzer = new SpectralFluxAnalyzer (AudioSettings.outputSampleRate);
        multiChannelSamples = new float[audioSource.clip.samples * audioSource.clip.channels];
        numChannels = audioSource.clip.channels;
        numTotalSamples = audioSource.clip.samples;
        clipLength = audioSource.clip.length;
        sampleRate = audioSource.clip.frequency;
        audioSource.clip.GetData(multiChannelSamples, 0);
        getFullSpectrum();
        // get spectral flux list
        pointInfo = preProcessedSpectralFluxAnalyzer.spectralFluxSamples;
        // generate a level based on Spectral flux analysis
        for(int i = 0; i < pointInfo.Count; i++)
        {
	        var point = pointInfo[i];
	        // if we have a peak
	        if (point.isPeak)
	        {
		        // randomly decide if this is an obstacle or coin
		        
		        Type type = typeof(Interacable);
		        Array values = type.GetEnumValues();
		        Interacable spawnObjectType = (Interacable)values.GetValue(GetRandomValue());
		        GameObject spawnObject;
		        if (spawnObjectType == Interacable.Collectable)
		        {
			        var posThreshold = new Vector3Int(i, (int)(point.spectralFlux * 10), 0);
			        var worldPoint= map.CellToWorld(posThreshold);
			        spawnObject = Instantiate(Collectable, worldPoint, Quaternion.identity);
			        spawnObject.transform.parent = transform;
			        SetLayerMask(LayerMask.NameToLayer("UI"), spawnObject);
			        levelObj.Add(spawnObject);
		        }
		        else if (spawnObjectType == Interacable.Obstacles)
		        {
			        var posThreshold = new Vector3Int(i, 0, 0);
			        var worldPoint= map.CellToWorld(posThreshold);
			        spawnObject = Instantiate(Obstacles, worldPoint, Quaternion.identity);
			        var objCollider = spawnObject.GetComponent<BoxCollider2D>();
			        worldPoint.y += objCollider.size.y;
			        spawnObject.transform.position = worldPoint;
			        spawnObject.transform.parent = transform;
			        SetLayerMask(LayerMask.NameToLayer("UI"), spawnObject);
			        levelObj.Add(spawnObject);
		        }
	        }
        }

        Vector3 startPt = map.CellToWorld(new Vector3Int(0, 0, 0));
        Vector3 endPt = map.CellToWorld(new Vector3Int(pointInfo.Count - 1, 0, 0));
        LevelLength = (endPt - startPt).x;
        playerInScene = Instantiate(Player, map.CellToWorld(new Vector3Int(0, 1, 0)), Quaternion.identity);
        playerInScene.layer = LayerMask.NameToLayer("UI");
        playerInScene.GetComponent<SpriteRenderer>().sortingOrder = 5;
        // populate the background
        float width = Background.GetComponent<SpriteRenderer>().bounds.size.x;
        float height = Background.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        float backgroundFill = 0;

        int index = 0;


        while (backgroundFill < LevelLength  + width)
        {
	        Vector3 pos = startPt + index * transform.right * width + transform.up * height;
	        var backgroundObj = Instantiate(Background, pos, Quaternion.identity);
	        backgroundObj.transform.parent = transform;
	        SetLayerMask(LayerMask.NameToLayer("UI"), backgroundObj);
	        index++;
	        backgroundFill += width;
	        levelObj.Add(backgroundObj);
        }
        LevelCreated = true;
    }
    
    public void DestroyLevel()
    {
	    transform.position = originalPos;
	    map.ClearAllTiles();
	    audioSource.Stop();
	    Destroy(playerInScene);
	    foreach (GameObject obj in levelObj)
	    {
		    Destroy(obj);
	    }
	    LevelCreated = false;
	    finished = false;
    }


    public void Update()
    {
	    if (LevelCreated && !finished)
	    {
		    float progress = audioSource.time / clipLength;
		    Vector3 currentPos = transform.position;
		    currentPos.x = -Mathf.Lerp(0, LevelLength, progress);
		    transform.position = currentPos;
		    if (Mathf.Approximately(progress, 1))
		    {
			    finished = true;
			    var controller = playerInScene.GetComponent<PlayerPlatformerController>();
			    controller.finishedLevel = finished;
		    }
	    }

		//weird SCRIPT for level finishing                                                                                //JAKOB SCRIPT  JAKOB SCRIPT  JAKOB SCRIPT  JAKOB SCRIPT  JAKOB SCRIPT currently finishing level 4 finishes the game

		if (finished == true)
        {
			//GetComponent<Animation>()["BusRide_WeatherChange"].wrapMode = WrapMode.Once;
			WorldAnimator.Play("BusRide_WeatherChange");
        }

    }

    public float getTimeFromIndex(int index) 
    {
	    return 1f / sampleRate * index;
    }
    
    

    
    public void getFullSpectrum() {

	    try {
			// We only need to retain the samples for combined channels over the time domain
			float[] preProcessedSamples = new float[this.numTotalSamples];

			int numProcessed = 0;
			float combinedChannelAverage = 0f;
			for (int i = 0; i < multiChannelSamples.Length; i++) {
				combinedChannelAverage += multiChannelSamples [i];

				// Each time we have processed all channels samples for a point in time, we will store the average of the channels combined
				if ((i + 1) % numChannels == 0) {
					preProcessedSamples[numProcessed] = combinedChannelAverage / numChannels;
					numProcessed++;
					combinedChannelAverage = 0f;
				}
			}

			// Once we have our audio sample data prepared, we can execute an FFT to return the spectrum data over the time domain
			int spectrumSampleSize = 1024;
			int iterations = preProcessedSamples.Length / spectrumSampleSize;

			FFT fft = new FFT ();
			fft.Initialize ((UInt32)spectrumSampleSize);
			double[] sampleChunk = new double[spectrumSampleSize];
			for (int i = 0; i < iterations; i++) {
				// Grab the current 1024 chunk of audio sample data
				Array.Copy (preProcessedSamples, i * spectrumSampleSize, sampleChunk, 0, spectrumSampleSize);

				// Apply our chosen FFT Window
				double[] windowCoefs = DSP.Window.Coefficients (DSP.Window.Type.Hanning, (uint)spectrumSampleSize);
				double[] scaledSpectrumChunk = DSP.Math.Multiply (sampleChunk, windowCoefs);
				double scaleFactor = DSP.Window.ScaleFactor.Signal (windowCoefs);

				// Perform the FFT and convert output (complex numbers) to Magnitude
				Complex[] fftSpectrum = fft.Execute (scaledSpectrumChunk);
				double[] scaledFFTSpectrum = DSP.ConvertComplex.ToMagnitude (fftSpectrum);
				scaledFFTSpectrum = DSP.Math.Multiply (scaledFFTSpectrum, scaleFactor);

				// These 1024 magnitude values correspond (roughly) to a single point in the audio timeline
				float curSongTime = getTimeFromIndex(i) * spectrumSampleSize;
				// Send our magnitude data off to our Spectral Flux Analyzer to be analyzed for peaks
				preProcessedSpectralFluxAnalyzer.analyzeSpectrum (Array.ConvertAll (scaledFFTSpectrum, x => (float)x), curSongTime);
			}
			
				
		} catch (Exception e) {
			// Catch exceptions here since the background thread won't always surface the exception to the main thread
			Debug.Log (e.ToString ());
		}
	}

    /// 70% of returning 1 and 30% of returning 0
    private int GetRandomValue() {
	    float rand = UnityEngine.Random.value;
	    if (rand <= .7f)
		    return 1;
	    return 0;
    }

    
    private static void SetLayerMask(int mask, GameObject obj)
    {
	    obj.layer = mask;
	    var children = obj.GetComponentsInChildren<Transform>(includeInactive: true);
	    foreach (var child in children)
	    {
		    child.gameObject.layer = mask;
	    }
    }

}

