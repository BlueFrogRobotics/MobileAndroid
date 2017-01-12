using UnityEngine;
using UnityEngine.UI;
using BuddyAPI;

[RequireComponent(typeof(SpeechToText))]
public class STTManager : MonoBehaviour
{
    [SerializeField]
    private RegularFace face;

    [SerializeField]
    private InputField input;

    private SpeechToText mSTT;

    void Start()
    {
        mSTT = GetComponent<SpeechToText>();
        mSTT.OnBeginning.Add(OnBeginning);
        mSTT.OnBestRecognition.Add(OnSTTComplete);
        mSTT.OnEnd.Add(OnEnd);
    }
    
    void Update()
    {

    }

    public void RequestSTT()
    {
        mSTT.Request();
    }

    private void OnBeginning()
    {
        face.SetMood(MoodType.LISTENING);
    }

    private void OnSTTComplete(string iSpeech)
    {
        input.text = iSpeech;
    }

    private void OnEnd()
    {
        face.SetMood(MoodType.NEUTRAL);
    }
}
