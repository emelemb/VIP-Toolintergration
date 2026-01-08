using UnityEngine;

public class ToolCursor : MonoBehaviour
{
    public Transform mousecursor;
    // Update is called once per frame
    void Update()
    {
       mousecursor.position = Input.mousePosition;
    }
}
