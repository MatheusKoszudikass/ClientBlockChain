namespace ClientBlockChain.Entities;

public static class GuidToken
{
    private static Guid _guidTokenGlobal;
    public static Guid GuidTokenGlobal
    {
        get { return _guidTokenGlobal;}
        private set {_guidTokenGlobal = value;}
    }

    static GuidToken()
    {
        GuidTokenGlobal = Guid.NewGuid();
    }
}
