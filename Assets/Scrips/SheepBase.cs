using UnityEngine;

public class SheepBase : MonoBehaviour
{
    public enum BaseColor
    {
        Red,
        Blue,
        Yellow
    };
    public BaseColor color;

    // Start is called before the first frame update
    void Awake()
    {
        // Set base color
        GetComponent<Renderer>().material.SetColor("_Color", this.GetColor());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Color GetColor()
    {
        switch (this.color)
        {
            case BaseColor.Red:
                return Constants.RED;
            case BaseColor.Blue:
                return Constants.BLUE;
            default:
                return Constants.YELLOW;
        }
    }

    private void OnTriggerEnter(Collider body)
    {
        Debug.Log(body.tag + " ENTROU NA BASEEEEEEEEEEEEEEEEEEEEEEE");
        // If sheep has entered base
        if (body.tag == Constants.SHEEP_TAG)
        {
            Debug.Log("DENTRO DO IF");
            body.gameObject.GetComponent<SheepCaptureState>().capture(this.gameObject);
            // sheep.GetComponent<SheepAI>().updateSheeps(); maybe change state to not escape base
        }
    }
}
