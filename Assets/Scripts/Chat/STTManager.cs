using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Class to control the face along with the STT's flow and generate the chat messages at end of speech
/// </summary>
//[RequireComponent(typeof(SpeechToText))]
public class STTManager : MonoBehaviour
{
    //[SerializeField]
    //private RegularFace face;

    //[SerializeField]
    //private InputField input;

    //[SerializeField]
    //private ChatManager chatManager;

    //private SpeechToText mSTT;

    //void Start()
    //{
    //    mSTT = GetComponent<SpeechToText>();
    //    mSTT.OnBeginning.Add(OnBeginning);
    //    mSTT.OnBestRecognition.Add(OnSTTComplete);
    //    mSTT.OnEnd.Add(OnEnd);
    //}
    
    //void Update()
    //{

    //}

    //public void RequestSTT()
    //{
    //    mSTT.Request();
    //}

    //private void OnBeginning()
    //{
    //    face.SetMood(MoodType.LISTENING);
    //}

    ////Get the said sentence and generate the message in chat
    //private void OnSTTComplete(string iSpeech)
    //{
    //    //input.text = iSpeech;
    //    chatManager.NewChatMessage(iSpeech);
    //}

    //private void OnEnd()
    //{
    //    face.SetMood(MoodType.NEUTRAL);
    //}
}
