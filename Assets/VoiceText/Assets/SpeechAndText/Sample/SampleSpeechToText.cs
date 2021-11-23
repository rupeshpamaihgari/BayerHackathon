using UnityEngine;
using UnityEngine.UI;
using TextSpeech;

public class SampleSpeechToText : MonoBehaviour
{
    public GameObject loading;
    public Text inputLocale;
    public InputField QuestionText;
    public GameObject AnswerPanel;
    public Text inputText;
    public float pitch;
    public float rate;

    public Text txtLocale;
    public Text txtPitch;
    public Text txtRate;
    void Start()
    {
        Setting("en-US");
        loading.SetActive(false);
        SpeechToText.instance.onResultCallback = OnResultSpeech;

    }

    private void Update()
    {
        
    }

    public void PopulateAnswer()
    {
        if (QuestionText.text.Contains("tomato"))
        {
            AnswerPanel.gameObject.SetActive(true);
        }
    }


    public void StartRecording()
    {
        AnswerPanel.gameObject.SetActive(false);

#if UNITY_EDITOR
#else
        SpeechToText.instance.StartRecording("Speak any");
#endif
    }

    public void StopRecording()
    {
#if UNITY_EDITOR
        OnResultSpeech("Not support in editor.");
#else
        SpeechToText.instance.StopRecording();
#endif
#if UNITY_IOS
        loading.SetActive(true);
#endif
    }
    void OnResultSpeech(string _data)
    {
        QuestionText.text = _data;
#if UNITY_IOS
        loading.SetActive(false);
#endif
    }
    public void OnClickSpeak()
    {
        TextToSpeech.instance.StartSpeak(inputText.text);
    }
    public void  OnClickStopSpeak()
    {
        TextToSpeech.instance.StopSpeak();
    }
    public void Setting(string code)
    {
        TextToSpeech.instance.Setting(code, pitch, rate);
        SpeechToText.instance.Setting(code);
        //txtLocale.text = "Locale: " + code;
        //txtPitch.text = "Pitch: " + pitch;
        //txtRate.text = "Rate: " + rate;
    }
    public void OnClickApply()
    {
        //Setting(inputLocale.text);
    }
}
