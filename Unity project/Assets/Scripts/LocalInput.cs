using UnityEngine;
public struct InputStruct{
    public bool jump;
    public int horizontalMove;
    public int frameCount;
}
public class LocalInput : MonoBehaviour
{
    [SerializeField] private KeyCode jumpKey = KeyCode.W;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    private InputStruct input;
    void Update()
    {
        input.jump = Input.GetKey(jumpKey);
        input.horizontalMove = 0;
        input.horizontalMove -= Input.GetKey(leftKey) ? 1 : 0;
        input.horizontalMove += Input.GetKey(rightKey) ? 1 : 0;
    }
        
    public InputStruct getInput() {
        return input;
    }
}
