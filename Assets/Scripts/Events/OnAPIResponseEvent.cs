using SimpleJSON;

public class OnAPIResponseEvent : GlobalEvent
{
    public JSONNode json;
    public Env.APIResponseType responseType;
}
