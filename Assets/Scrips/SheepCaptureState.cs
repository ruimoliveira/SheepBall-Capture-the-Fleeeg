using UnityEngine;

public class SheepCaptureState : MonoBehaviour
{
    private bool captured = false;
    private GameObject captureBase;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isCaptured()
    {
        return this.captured;
    }

    public void capture(GameObject captureBase)
    {
        this.captured = true;
        this.captureBase = captureBase;

        Color baseColor = captureBase.GetComponent<SheepBase>().GetColor();
        GameObject sheepBody = transform.Find("Demo White Rabbit/Rabbit").gameObject;
        sheepBody.GetComponent<Renderer>().material.SetColor("_EmissionColor", baseColor);
    }
}
