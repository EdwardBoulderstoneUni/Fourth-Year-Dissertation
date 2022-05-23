using UnityEngine;
public class CharacterController2D : MonoBehaviour
{
    private int rejumpPreventionFrames;
    private int jumpFrame;
    private int frame;
    private LayerMask floor;
    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    private RaycastHit2D[] collidedObjects;
    private float distanceToGround;
    private float moveSpeed;
    private float jumpSpeed;
    private bool grounded;

    void Start(){
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        collidedObjects = new RaycastHit2D[1];
        var characterValues = gameObject.transform.parent.GetComponentInParent<CharacterValues>();
        floor = characterValues.floor;
        moveSpeed =  characterValues.moveSpeed;
        jumpSpeed = characterValues.jumpSpeed;
        rejumpPreventionFrames = characterValues.rejumpPreventionFrames;
        distanceToGround =  characterValues.distanceToGround + boxCollider.size.y/2;
        grounded = false;
        jumpFrame = -rejumpPreventionFrames;
        frame = 0;
    }
    void checkGrounded(){
        grounded = boxCollider.Raycast(Vector2.down, collidedObjects, distanceToGround, floor) == 1;
    }

    void jump(){
        jumpFrame = frame;
        rigidBody.velocity += new Vector2(0, jumpSpeed);
    }
    void move(int direction){
        rigidBody.velocity = new Vector2(moveSpeed * direction, rigidBody.velocity.y);
    }

    public void update(InputStruct input)
    {
        checkGrounded();
        if (grounded){
            if (input.jump && ((frame - jumpFrame) > rejumpPreventionFrames))
                jump();
            else
                move(input.horizontalMove);
        }
        frame += 1;
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
