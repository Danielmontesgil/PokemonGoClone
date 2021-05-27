using UnityEngine.UI;

public class OnAPIRequestEvent : GlobalEvent
{
    public string url;
    public Env.APIResponseType type;
}

public class OnImageRequestEvent: GlobalEvent
{
    public string url;
    public RawImage image;
}
