namespace Aps.Configuration.Application.Dtos;

public class AccountsRequestDto
{
    public string Nabp { get; set; }
    public string Npi { get; set; }
    public string Group { get; set; }
    public string Bin { get; set; }
    public string BillCode { get; set; }
    public string Region { get; set; }
    public string SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}