using UnityEngine;
public struct InputStruct{
    public bool jump;
    public int horizontalMove;
    public override bool Equals(object obj) => this.Equals(obj is InputStruct other && this.Equals(other));

    public bool Equals(InputStruct other) => jump == other.jump && horizontalMove == other.horizontalMove;

    public override int GetHashCode() => (jump, horizontalMove).GetHashCode();
    public static bool operator==(InputStruct lhs, InputStruct rhs){
        return lhs.jump == rhs.jump && lhs.horizontalMove == rhs.horizontalMove;
    }
    public static bool operator!=(InputStruct lhs, InputStruct rhs){
        return !(lhs == rhs);
    }
    
    public override string ToString(){
        return "jump: " + jump + ", horizontalMove: " + horizontalMove;
    }
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
