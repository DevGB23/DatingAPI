namespace Dating_WebAPI.Helpers;
public class PaginationParams
{
    private const int _maxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > _maxPageSize) ? _maxPageSize : value; 
    }

}
