public static class SortTypeLibraryMovie
{
    public const string TITLE = "title";
    public const string DURATION = "duration";
    public const string ADDED_AT = "added-at";
    public const string PRICE_DESC = "price-desc";
    public const string PRICE_ASC = "price-asc";
    public const string IMDB_RATIONG = "imdb-rating";
    
    public static List<string> SortTypesList { get; private set; }
    
    static SortTypeLibraryMovie()
    {
        SortTypesList = new List<string>()
        {
            TITLE, DURATION, ADDED_AT, PRICE_DESC, PRICE_ASC, IMDB_RATIONG
        }; 
    }
}
