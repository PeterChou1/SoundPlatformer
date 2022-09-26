using System;
using System.Collections.Generic;
using DSPLib;
using UnityEngine;
using Complex = System.Numerics.Complex;
using UnityEngine.Tilemaps;

public class LevelCreator : MonoBehaviour
{
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
    private int index;
    private List<SpectralFluxInfo> pointInfo;
    private Vector3 velocity = Vector3.zero;
    private Vector3 originalPos;
    private GameObject playerInScene;
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
		        int randomIndex = UnityEngine.Random.Range(0, values.Length);
		        Interacable spawnObjectType = (Interacable)values.GetValue(randomIndex);
		        GameObject spawnObject;
		        if (spawnObjectType == Interacable.Collectable)
		        {
			        var posThreshold = new Vector3Int(i, (int)(point.spectralFlux * 10), 0);
			        var worldPoint= map.CellToWorld(posThreshold);
			        spawnObject = Instantiate(Collectable, worldPoint, Quaternion.identity);
			        spawnObject.transform.parent = transform;
			        Debug.Log("Spawn Object pos: " + spawnObject.transform.position);
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
			        Debug.Log("Spawn Object pos: " + spawnObject.transform.position);
		        }
	        }
        }
        var start = new Vector3Int(0, (int)0, 0);
        var startPt= map.CellToWorld(start);
        var end = new Vector3Int(pointInfo.Count - 1, (int)0, 0);
        var endPt= map.CellToWorld(end);
        Debug.Log("Start point :" + startPt + " End pt : " + endPt);
        //playerInScene = Instantiate(Player, new Vector3(0, 1, 0), Quaternion.identity);
        //playerInScene.GetComponent<SpriteRenderer>().sortingOrder = 5;
        LevelCreated = true;
    }

    public void DestroyLevel()
    {
	    transform.position = originalPos;
	    map.ClearAllTiles();
	    audioSource.Stop();
	    Destroy(playerInScene);
	    index = 0;
	    LevelCreated = false;
    }


    public void Update()
    {
	    if (LevelCreated && index < pointInfo.Count - 1)
	    {
		    //var curSpectral = pointInfo[index];
		    //var nextSpectral = pointInfo[index + 1];
		    //var curDistance = Math.Abs(curSpectral.time - audioSource.time);
		    //var nextDistance = Math.Abs(nextSpectral.time - audioSource.time);
		    //if (nextDistance < curDistance)
		    //{
			//    index += 1;
		    //}
		    //Vector3 center = -map.GetCellCenterWorld(new Vector3Int(index, 0, 0));
		    //Debug.Log("index: " + index + " Center of cell: " + center);
		    //// current 
		    //var currentPos = Vector3.SmoothDamp(map.transform.position, center, ref velocity, 0.03f);
		    //map.transform.position = currentPos;
	    }
    }

    public float getTimeFromIndex(int index) 
    {
	    return 1f / sampleRate * index;
    }
    
    public void getFullSpectrum() {

	    try {
			// We only need to retain the samples for combined channels over the time domain
			float[] preProcessedSamples = new float[numTotalSamples];

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
}

