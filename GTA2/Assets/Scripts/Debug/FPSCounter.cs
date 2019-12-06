using UnityEngine;
using System.Collections;
using UnityEngine.UI;
 
/* **************************************************************************
 * CLASS: FPS COUNTER
 * *************************************************************************/ 
public class FPSCounter : MonoBehaviour {
	/* Public Variables */
	public float frequency = 0.5f;
 
	/* **********************************************************************
	 * PROPERTIES
	 * *********************************************************************/
	public int FramesPerSec { get; protected set; }

    Text outputText;


	/* **********************************************************************
	 * EVENT HANDLERS
	 * *********************************************************************/
	/*
	 * EVENT: Start
	 */
	private void Start() {
        outputText = GetComponent<Text>();
		StartCoroutine(FPS());
	}
 
	/*
	 * EVENT: FPS
	 */
	private IEnumerator FPS() {
		for(;;){
			// Capture frame-per-second
			int lastFrameCount = Time.frameCount;
			float lastTime = Time.realtimeSinceStartup;
			yield return new WaitForSeconds(frequency);
			float timeSpan = Time.realtimeSinceStartup - lastTime;
			int frameCount = Time.frameCount - lastFrameCount;
 
			// Display it
			FramesPerSec = Mathf.RoundToInt(frameCount / timeSpan);
            outputText.text = FramesPerSec.ToString() + " fps";
		}
	}
}