using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toast : MonoBehaviour
{


    private const int FONT_SIZE = 40; //14;
    private const int LINE_HEIGHT = 60; //30
    private const int BOTTOM_MARGIN = 200;
    private const int BOTTOM_MARGIN_OCULUS = 600;


    public Canvas Canvas;


    private List<GameObject> _messages = new List<GameObject>();


    void Start()
    {
    }


    public void Present(string toastMessage)
    {
        Present(toastMessage, 0.5f, 1f, 0.5f);
    }


    public void Present(string toastMessage, float fadeInDuration, float displayDuration, float fadeOutDuration)
    {
        PushMessagesUp();

        GameObject gameObject = CreateTextObject(toastMessage);
        _messages.Add(gameObject);  

        StartCoroutine(showToastCOR(gameObject, fadeInDuration, displayDuration, fadeOutDuration));
    }


    private void PushMessagesUp()
    {
        lock(_messages)
        {
            //push them up
            foreach (GameObject gameObject in _messages)
            {
                Text text2 = gameObject.GetComponent<Text>();
                text2.rectTransform.localPosition = new Vector3(0, text2.rectTransform.localPosition.y + LINE_HEIGHT, 0);
            }
        }
    }


    private GameObject CreateTextObject(string toastMessage)
    {
        bool isOculus = UnityEngine.XR.XRSettings.isDeviceActive;

        RectTransform objectRectTransform = Canvas.gameObject.GetComponent<RectTransform>();
        float top = -((objectRectTransform.rect.height / 2) - (isOculus ? BOTTOM_MARGIN_OCULUS : BOTTOM_MARGIN));

        GameObject toast = new GameObject("GameObject");
        toast.name = "toastMessage";
        toast.transform.parent = Canvas.transform;

        Text text = toast.AddComponent<Text>();
        text.fontSize = FONT_SIZE;
        text.text = toastMessage;
        text.alignment = TextAnchor.MiddleCenter;
        text.rectTransform.pivot.Scale(new Vector2(1, 1));
        text.rectTransform.pivot.Set(0.5f, 0.5f);
        text.rectTransform.localRotation = new Quaternion(0, 0, 0, 0);
        text.rectTransform.localPosition =  new Vector3(0, top, -100);
        text.rectTransform.localScale = new Vector3(1, 1, 1);
        text.rectTransform.sizeDelta = new Vector2(100000, LINE_HEIGHT);


        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.font = ArialFont;
        text.material = ArialFont.material;  

        return toast; 
    }


    private IEnumerator showToastCOR(GameObject gameObject, float fadeInDuration, float displayDuration, float fadeOutDuration)
    {
        Text textObject = gameObject.GetComponent<Text>();

        Color orginalColor = new Color(255, 255, 255, 255);

        //Fade in
        yield return fadeInAndOut(textObject, true, fadeInDuration);

        //Wait for the duration
        float counter = 0;
        while (counter < displayDuration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return fadeInAndOut(textObject, false, fadeOutDuration);

        textObject.enabled = false;
        textObject.color = orginalColor;

        _messages.Remove(gameObject);
    }


    private IEnumerator fadeInAndOut(Text targetText, bool fadeIn, float duration)
    {
        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 1f;
        }
        else
        {
            a = 1f;
            b = 0f;
        }

        Color currentColor = new Color(255, 255, 255, 0);// Color.clear;
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
    }


}
