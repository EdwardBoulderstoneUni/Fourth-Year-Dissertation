using UnityEngine;
public class LocalInput : MonoBehaviour
{
    private const int inputs = 3;
    [SerializeField] private KeyCode jumpKey = KeyCode.W;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    private InputStruct input;
    private bool[] inputBuffer;
    void Start(){
        inputBuffer = new bool[inputs];
    }
    void Update()
    {
        inputBuffer[0] = Input.GetKey(jumpKey) || inputBuffer[0];
        inputBuffer[1] = Input.GetKey(leftKey) || inputBuffer[1];
        inputBuffer[2] = Input.GetKey(rightKey) || inputBuffer[2];
    }
        
    public InputStruct getInput() {
        input.jump = inputBuffer[0];
        input.horizontalMove = 0;
        input.horizontalMove -= inputBuffer[1] ? 1 : 0;
        input.horizontalMove += inputBuffer[2] ? 1 : 0;
        reset();
        return input;
    }

    private void reset(){
        for (int index = 0; index < inputs; index++)
            inputBuffer[index] = false;
    }
}
