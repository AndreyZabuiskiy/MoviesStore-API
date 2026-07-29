public static class VisibleTypeLibraryMovie
{
    public const string VISIBLE = "visible";
    public const string HIDDEN = "hidden";
    public const string ALL = "all";

    public static List<string> VisibleTypesList { get; private set; }
    
    static VisibleTypeLibraryMovie()
    {
        VisibleTypesList = new List<string>()
        {
            VISIBLE, HIDDEN, ALL
        }; 
    }
}
