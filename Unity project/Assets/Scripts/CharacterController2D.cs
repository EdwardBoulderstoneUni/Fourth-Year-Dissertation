using UnityEngine;
public class CharacterController2D : MonoBehaviour
{
    private int jumpFrame;
    public int frame;
    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    private RaycastHit2D[] collidedObjects;
    private GameState game;
    private bool grounded;

    void Start(){
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        game = gameObject.GetComponentInParent<GameState>();
        collidedObjects = new RaycastHit2D[1];
        grounded = false;
        jumpFrame = -game.rejumpPreventionFrames;
        frame = 0;
    }
    void checkGrounded(){
        grounded = boxCollider.Raycast(Vector2.down, collidedObjects, game.distanceToGround + boxCollider.size.y, game.floor) == 1;
    }

    void jump(){
        jumpFrame = frame;
        rigidBody.velocity += new Vector2(0, game.jumpSpeed);
    }
    void move(int direction){
        rigidBody.velocity = new Vector2(game.moveSpeed * direction, rigidBody.velocity.y);
    }

    public void update(InputStruct input)
    {
        checkGrounded();
        if (grounded){
            if (input.jump && ((frame - jumpFrame) > game.rejumpPreventionFrames))
                jump();
            else
                move(input.horizontalMove);
        }
        frame += 1;
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
