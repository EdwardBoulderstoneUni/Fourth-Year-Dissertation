using UnityEngine;
public class InputManager : MonoBehaviour
{
    protected const int inputs = 3;
    [SerializeField] private KeyCode jumpKey = KeyCode.W;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    protected InputStruct input;
    protected bool[] inputBuffer;
    void Start(){
        inputBuffer = new bool[inputs];
    }
    public void ping()
    {
        inputBuffer[0] = Input.GetKey(jumpKey) || inputBuffer[0];
        inputBuffer[1] = Input.GetKey(leftKey) || inputBuffer[1];
        inputBuffer[2] = Input.GetKey(rightKey) || inputBuffer[2];
    }

    protected void readInputs(){
        input.jump = inputBuffer[0];
        input.horizontalMove = 0;
        input.horizontalMove -= inputBuffer[1] ? 1 : 0;
        input.horizontalMove += inputBuffer[2] ? 1 : 0;
    }
        
    public virtual InputStruct getInput() {
        readInputs();
        reset();
        return input;
    }

    protected void reset(){
        for (int index = 0; index < inputs; index++)
            inputBuffer[index] = false;
    }
}
