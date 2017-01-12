using System;

[Serializable]
public class Users
{
    public string firstname;
    public string name;
    public int age;
    public string photo;
    public string rights;
}

[Serializable]
public class Applications
{
    public string name;
    public string[] parameters;
    public string right;
}

[Serializable]
public class Timetable
{
    public string day;
    public string[] parameters;
}

[Serializable]
public class Event
{
    public string day;
    public string title;
    public string description;
}

[Serializable]
public class BuddyConf
{
    public int version;
    public Users[] users;
    public string houseMap;
    public string vocalSignature;
    public string faceRecognition;
    public Applications[] applications;
    public Timetable[] timetable;
    public Event[] events;
}

[Serializable]
public class KeyResponse
{
    public string token;
    public string uri;
}
