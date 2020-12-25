using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Account
{
    public int ID;
    public string Username;
    public int Avatar;
    public bool isVerificated;
    public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

    [System.Serializable]
    public struct Character
    {
        public int ID;
        public string Name;
        public string Gender;
        public Vector3 Position;
        public Quaternion Rotation;

        public Character(int id, string char_name, string char_gender, Vector3 char_pos, Quaternion char_rot)
        {
            ID = id;
            Name = char_name;
            Gender = char_gender;
            Position = char_pos;
            Rotation = char_rot;
        }
    }

    public Account(int _id, string _username, int _avatar, bool _isVerificated, Dictionary<int, Character> _characters)
    {
        ID = _id;
        Username = _username;
        Avatar = _avatar;
        isVerificated = _isVerificated;
        Characters = _characters;
    }
}
