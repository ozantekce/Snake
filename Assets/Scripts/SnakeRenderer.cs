using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SnakeRenderer : MonoBehaviour
{

    public GameObject head;
    
    public float fixedZ = -2;


    public LineRenderer lineRenderer;
    private const int AddLineToEachMove = 15;

    private int _currentSize;
    private int _targetSize;

    private bool _moving = false;

    public bool Moving { get => _moving; set => _moving = value; }

    public void ArrangeSize(float size)
    {

        head.transform.localScale = new Vector3(1,1,20) * size;
        lineRenderer.endWidth = size * 20f;
        lineRenderer.startWidth = size * 15f;
    }

    public void ArrangeStartPosition(Vector3 position)
    {
        position.z = fixedZ;
        head.transform.position = position;
    }


    public void ArrangeTailSize(int tailSize, float gameSpeed)
    {
        _targetSize = (tailSize * AddLineToEachMove);
        //trailRenderer.time = (0.5f + tailSize - 1)/ gameSpeed;
        //lengthBasedTrail.maxLength = tailSize;
    }


    public void MoveHead(Vector3 pos, float duration)
    {

        StartCoroutine(MoveRoutine(pos, duration));

    }


    public IEnumerator MoveRoutine(Vector3 pos, float duration)
    {
        _moving = true;

        Vector3 startPos = head.transform.position;
        Vector3 endPos = pos;
        endPos.z = startPos.z;

        Vector3 delta = endPos - startPos;
        delta.Normalize();
        //Debug.Log(delta);
        float angle = head.transform.eulerAngles.z;
        float x = delta.x;
        float y = delta.y;
        if (y == -1) angle = -90;
        else if (y == +1) angle = +90;
        else if (x == +1) angle = 0;
        else if (x == -1) angle = 180;

        Quaternion startRot = head.transform.rotation;
        Quaternion endRot = Quaternion.Euler(0, 0, angle);

        float t = 0;
        float addLineT = 0;
        int addedLines = 0;
        while (t < duration)
        {
            if (addLineT <= t && addLineT<AddLineToEachMove)
            {
                addLineT += (duration / AddLineToEachMove);
                addedLines++;
                AddLine(head.transform.position);
                if (_currentSize > _targetSize)
                {
                    RemoveLine();
                }

            }

            t += Time.deltaTime;
            float factor = Mathf.SmoothStep(0, 1, t / duration);

            head.transform.position = Vector3.Lerp(startPos, endPos, factor);
            head.transform.rotation = Quaternion.Slerp(startRot, endRot, factor);

            yield return null;
        }
        
        if(addedLines < AddLineToEachMove) {
            while(addedLines < AddLineToEachMove)
            {
                addedLines++;
                AddLine(head.transform.position);
            }
        }
        
        while (_currentSize > _targetSize)
        {
            RemoveLine();
        }

        head.transform.position = endPos;
        head.transform.rotation = endRot;

        _moving = false;

    }


    
    private void AddLine(Vector3 pos)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
        _currentSize++;
    }


    private void RemoveLine()
    {
        RemoveFirstNPoints(1);
        _currentSize--;
    }

    private void RemoveFirstNPoints(int n)
    {
        var oldPositions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(oldPositions);

        if (n >= oldPositions.Length)
        {
            lineRenderer.positionCount = 0;
        }
        else
        {
            var newPositions = new Vector3[oldPositions.Length - n];
            System.Array.Copy(oldPositions, n, newPositions, 0, newPositions.Length);
            lineRenderer.positionCount = newPositions.Length;
            lineRenderer.SetPositions(newPositions);
        }
    }

}
