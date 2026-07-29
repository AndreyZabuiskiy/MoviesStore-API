public class LibraryResponseDto
{
    public string SortType { get; set; }
    public string VisibleType { get; set; }
    public List<LibraryMovieDto> Movies { get; set; }
}