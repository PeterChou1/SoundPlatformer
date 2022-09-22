using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DSPLib;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelCreator : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource audioSource;
    private Tilemap map;
    private int numChannels;
    private int numTotalSamples;
    private int sampleRate;
    private float clipLength;
    private float[] multiChannelSamples;
    SpectralFluxAnalyzer preProcessedSpectralFluxAnalyzer;
    
    void CreateLevel()
    {
	    audioSource = GetComponent<AudioSource>();
        map = GetComponentInChildren<Tilemap>();
        preProcessedSpectralFluxAnalyzer = new SpectralFluxAnalyzer (AudioSettings.outputSampleRate);
        multiChannelSamples = new float[audioSource.clip.samples * audioSource.clip.channels];
        numChannels = audioSource.clip.channels;
        numTotalSamples = audioSource.clip.samples;
        clipLength = audioSource.clip.length;
        sampleRate = audioSource.clip.frequency;
        audioSource.clip.GetData(multiChannelSamples, 0);
        getFullSpectrum();
        // get spectral flux list
        List<SpectralFluxInfo> pointInfo = preProcessedSpectralFluxAnalyzer.spectralFluxSamples;
        // generate a level based on Spectral flux analysis
        for(int i = 0; i < pointInfo.Count; i++)
        {
	        var point = pointInfo[i];
	        var pos = new Vector3Int(i, (int)(point.threshold * 10), 0);
	        map.SetTile(pos, TileLoader.GetTestTile());
	        Debug.Log(point.threshold);
        }
    }
    
    public float getTimeFromIndex(int index) {
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

			Debug.Log ("Combine Channels done");
			Debug.Log (preProcessedSamples.Length);

			// Once we have our audio sample data prepared, we can execute an FFT to return the spectrum data over the time domain
			int spectrumSampleSize = 1024;
			int iterations = preProcessedSamples.Length / spectrumSampleSize;

			FFT fft = new FFT ();
			fft.Initialize ((UInt32)spectrumSampleSize);

			Debug.Log (string.Format("Processing {0} time domain samples for FFT", iterations));
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

			Debug.Log ("Spectrum Analysis done");
			Debug.Log ("Background Thread Completed");
				
		} catch (Exception e) {
			// Catch exceptions here since the background thread won't always surface the exception to the main thread
			Debug.Log (e.ToString ());
		}
	}
}

