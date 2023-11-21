using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class MissionTexts : MonoBehaviour
{
    [SerializeField] private string upperText;
    [SerializeField] private string bottomtext;
    
    [SerializeField] private GameObject missionText;
    [SerializeField] private GameObject objectiveText;

    [SerializeField] private float speed = 13;
    [SerializeField] private float movingTime = 1;
    [SerializeField] private float waitingTime = 2.5f;
    private float _canvasWith;
    private bool _isMovingTextToMiddle;
    private bool _isMovingTextAway;

    private Vector3 _startPosition;
    private Vector3 _canvasMiddlePositionUp;
    private Vector3 _canvasMiddlePosition;
    private Vector3 _canvasRightPosition;
    private Vector3 _canvasLeftPosition;
    private void Start()
    {
        _startPosition = new Vector3(10000, 0, 0);
        var canvas = FindObjectOfType<Canvas>();
        _canvasWith = canvas.pixelRect.width;
        _canvasMiddlePosition = new Vector3(_canvasWith / 1.5f, 0, 0);
        
        _canvasRightPosition = new Vector3(_canvasWith  * 2, 0, 0);
        _canvasLeftPosition = new Vector3(0 -1 * (_canvasWith / 8), 0, 0);
        SetText();
    }

    private void Update()
    {
        var missionPosition = _isMovingTextToMiddle ? _canvasMiddlePosition : _canvasLeftPosition;
        var objectivePosition = _isMovingTextToMiddle ? _canvasMiddlePosition : _canvasRightPosition;
        Lerp(missionText, missionPosition);
        Lerp(objectiveText, objectivePosition);
    }

    private void Lerp(GameObject target, Vector3 endPosition)
    {
        target.transform.position = Vector3.Lerp(target.transform.position, endPosition, speed * Time.deltaTime);
    }

    private void SetText()
    {
        var canvas = FindObjectOfType<Canvas>(); 
        

        missionText = new GameObject("MissionText");
        objectiveText = new GameObject("ObjectiveText");
        objectiveText.transform.position = _startPosition;
        Update();
        Font arial;
        arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

        missionText.transform.parent = canvas.transform;
        objectiveText.transform.parent = canvas.transform;
        
        missionText.AddComponent<Text>();
        objectiveText.AddComponent<Text>();
        
        RectTransform upperTextRt = missionText.GetComponent<RectTransform>();
        RectTransform bottomTextRt = objectiveText.GetComponent<RectTransform>();
        
        var upperTextPosition = new Vector2(_canvasWith / 1.8f, _canvasWith / 1.8f);
        var bottomTextPosition = new Vector2(_canvasWith / 1.8f, _canvasWith / 2.3f);
        upperTextRt.sizeDelta = upperTextPosition;
        bottomTextRt.sizeDelta = bottomTextPosition;

        var text = missionText.GetComponent<Text>();
        var objective = objectiveText.GetComponent<Text>();
        
        text.font = arial;
        objective.font = arial;
        
        text.text = upperText;
        objective.text = bottomtext;
        
        var fontSize = Mathf.RoundToInt(_canvasWith / 38.2f);
        text.fontSize = fontSize;
        objective.fontSize = fontSize;

        
        StartCoroutine(MoveText());
    }

    private IEnumerator MoveText()
    {
        yield return new WaitForSeconds(movingTime);
        _isMovingTextToMiddle = true;
        yield return new WaitForSeconds(waitingTime);
        _isMovingTextToMiddle = false;
        _isMovingTextAway = true;
        yield return new WaitForSeconds(movingTime);
        _isMovingTextAway = false;
        Destroy(missionText); 
        Destroy(objectiveText);
        Destroy(gameObject);
    }
}
