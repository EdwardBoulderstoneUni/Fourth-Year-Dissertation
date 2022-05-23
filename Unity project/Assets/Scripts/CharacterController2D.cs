using UnityEngine;
public struct SerializedPlayer {
    public Vector2 location;
    public Vector2 velocity;
    public bool grounded;
    public override string ToString(){
        return "location: " + location + ", velocity: " + velocity + ", grounded: " + grounded;
    }
}
public class CharacterController2D : MonoBehaviour
{
    private LayerMask floor;
    private GameState state;
    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    private RaycastHit2D[] collidedObjects;
    private float distanceToGround; 
    private bool grounded;

    void Start(){
        state = gameObject.GetComponentInParent<GameState>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        floor = gameObject.transform.parent.GetComponentInParent<CharacterValues>().floor;
        collidedObjects = new RaycastHit2D[1];
        distanceToGround =  GameState.DistanceToGround + boxCollider.size.y/2;
        grounded = false;
    }
    void checkGrounded(){
        grounded = boxCollider.Raycast(Vector2.down, collidedObjects, distanceToGround, floor) == 1;
    }

    void jump(){
        rigidBody.velocity += new Vector2(0, GameState.JumpSpeed);
    }
    void move(int direction){
        rigidBody.velocity = new Vector2(GameState.MoveSpeed * direction, rigidBody.velocity.y);
    }

    public void update(InputStruct input)
    {
        checkGrounded();
        if (grounded){
            if (input.jump)
                jump();
            else
                move(input.horizontalMove);
        }
    }

    public void pause(){
        rigidBody.isKinematic = true;
    }
    public void resume(){
        rigidBody.isKinematic = false;
    }

    public SerializedPlayer serialized(){
        SerializedPlayer serialized = new SerializedPlayer();
        serialized.location = transform.position;
        serialized.velocity = rigidBody.velocity;
        serialized.grounded = grounded;
        return serialized;
    }

    public void loadState(SerializedPlayer state){
        transform.position = state.location;
        rigidBody.velocity = state.velocity;
        grounded = state.grounded;
    }
}
