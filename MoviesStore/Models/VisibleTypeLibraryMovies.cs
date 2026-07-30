public static class VisibleTypeLibraryMovies
{
    public const string VISIBLE = "visible";
    public const string HIDDEN = "hidden";
    public const string ALL = "all";
    public static HashSet<string> VisibleTypesList { get; }

    public static bool IsVisibleValid(string visibleQuery)
    {
        return VisibleTypesList.Contains(visibleQuery);
    }
    
    static VisibleTypeLibraryMovies()
    {
        VisibleTypesList = new HashSet<string>()
        {
            VISIBLE, HIDDEN, ALL
        }; 
    }
}
