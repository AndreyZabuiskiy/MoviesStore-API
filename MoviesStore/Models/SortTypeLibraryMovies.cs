public static class SortTypeLibraryMovies
{
    public const string TITLE = "title";
    public const string DURATION = "duration";
    public const string ADDED_AT = "added-at";
    public const string PRICE_DESC = "price-desc";
    public const string PRICE_ASC = "price-asc";
    public const string IMDB_RATIONG = "imdb-rating";
    public static HashSet<string> SortTypesList { get; }

    public static bool IsSortValid(string sortQuery)
    {
        return SortTypesList.Contains(sortQuery);
    }
    
    static SortTypeLibraryMovies()
    {
        SortTypesList = new HashSet<string>()
        {
            TITLE, DURATION, ADDED_AT, PRICE_DESC, PRICE_ASC, IMDB_RATIONG
        }; 
    }
}
