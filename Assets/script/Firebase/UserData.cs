// Assets/Scripts/Firebase/UserData.cs
using System;

[Serializable]
public class UserData
{
    public string name;
    public string email;
    public string uid;

    public UserData() { }

    public UserData(string name, string email, string uid = "")
    {
        this.name = name;
        this.email = email;
        this.uid = uid;
    }
}
