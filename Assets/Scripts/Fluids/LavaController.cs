using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaController : MonoBehaviour
{
    public GradientPath path;
    public bool startRolling = false;
    
    public int currentReplyIndex;
    private int current;

    private Rigidbody rb;
    
    private List<GridPoint> recordedPoints;

    // Update is called once per frame
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        recordedPoints = new List<GridPoint>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("structure"))
            Destroy(other.gameObject);
        if (other.gameObject.tag.Equals("GridPoint"))
        {
            other.gameObject.GetComponent<GridPoint>().IsoValue = 1;
            recordedPoints.Add(other.gameObject.GetComponent<GridPoint>());
            currentReplyIndex++;
        }
    }

    void FixedUpdate()
    {
        if (startRolling && current < path.GetPoints().Count)
        {
            if (transform.position != path.GetPoints()[current].Item1)
            {
                var pos = Vector3.MoveTowards(
                    transform.position,
                    path.GetPoints()[current].Item1,
                    path.GetPoints()[current].Item4 * Time.deltaTime
                );
                rb.MovePosition(pos);
            }
            else
                current++;
        }
    }

    public void Rewind(int changeRate)
    {
        if (!LavaManager.Instance.IsSimulationDone())
            return;
        currentReplyIndex = Mathf.Clamp(currentReplyIndex+changeRate, 0, recordedPoints.Count-1);
        recordedPoints[currentReplyIndex].IsoValue = changeRate < 0 ? 0 : 1;
    }
    
    public bool IsDone()
    {
        return current >= path.GetPoints().Count;
    }
}
