using System;
using UnityEngine;
public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private GameObject physicsObject;
    private int jumpFrame;
    private Rigidbody2D rigidBody;
    private GameState game;
    private bool grounded;
    private const float groundedY = 1.52f;
    private bool syncedRendering = false;

    void Start(){
        rigidBody = physicsObject.GetComponent<Rigidbody2D>();
        game = gameObject.GetComponentInParent<GameState>();
        grounded = false;
        jumpFrame = -GameState.rejumpPreventionFrames;
    }
    void checkGrounded(){
        grounded = transform.position.y <= groundedY;
    }

    void jump(){
        jumpFrame = game.getFrame();
        rigidBody.velocity += new Vector2(0, GameState.jumpSpeed);
    }
    void move(int direction){
        rigidBody.velocity = new Vector2(GameState.moveSpeed * direction, rigidBody.velocity.y);
    }

    public bool doInputsMatter(){
        checkGrounded();
        return grounded;
    } 

    public void update(InputStruct input)
    {
        if (input.jump && ((game.getFrame() - jumpFrame) > GameState.rejumpPreventionFrames))
            jump();
        else
            move(input.horizontalMove);
        noInputUpdate();
    }
    public void noInputUpdate(){
        if (syncedRendering){
            transform.position += physicsObject.transform.position;
            physicsObject.transform.position = new Vector3();
        }
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

    public void desyncPhysics(){
        syncedRendering = false;
    }

    public void resyncPhysics(){
        syncedRendering = true;
        noInputUpdate();

    }
}
