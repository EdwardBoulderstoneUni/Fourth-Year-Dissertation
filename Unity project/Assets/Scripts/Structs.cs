using UnityEngine;
using System;
[Serializable] public struct InputStruct{
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
public struct SerializedPlayer {
    public Vector2 location;
    public Vector2 velocity;
    public bool grounded;
    static bool Approximately(Vector2 lhs, Vector2 rhs){
        return Mathf.Abs(lhs.x - rhs.x) < 0.01 && Mathf.Abs(lhs.y - rhs.y) < 0.01;
    }
    public override bool Equals(object obj) => this.Equals(obj is SerializedPlayer other && this.Equals(other));

    public bool Equals(SerializedPlayer other) => Approximately(location, other.location) && Approximately(velocity, other.velocity) && grounded == other.grounded;

    public override int GetHashCode() => (location, velocity, grounded).GetHashCode();
    public static bool operator==(SerializedPlayer lhs, SerializedPlayer rhs){
        return Approximately(lhs.location, rhs.location) && Approximately(lhs.velocity, rhs.velocity) && lhs.grounded == rhs.grounded;
    }
    public static bool operator!=(SerializedPlayer lhs, SerializedPlayer rhs){
        return !(lhs == rhs);
    }
    public override string ToString(){
        return "location: " + location + ", velocity: " + velocity + ", grounded: " + grounded;
    }
}

public struct State{
    public SerializedPlayer player1;
    public SerializedPlayer player2;
    public override bool Equals(object obj) => this.Equals(obj is State other && this.Equals(other));
    public bool Equals(State other) => player1 == other.player1 && player2 == other.player2;
    public override int GetHashCode() => (player1, player2).GetHashCode();
    public static bool operator==(State lhs, State rhs){
        return lhs.player1 == rhs.player1 && lhs.player2 == rhs.player2;
    }
    public static bool operator!=(State lhs, State rhs){
        return !(lhs == rhs);
    }
    public override string ToString(){
        return "Player1: {" + player1.ToString() + "}, " + "Player2: {" + player2.ToString() + "}";
    }
}

[Serializable] public struct TimedData<T>{
    public int frame;
    public T data;
    public override string ToString(){
        return "Frame: " + frame + ", " + data.ToString();
    }
}