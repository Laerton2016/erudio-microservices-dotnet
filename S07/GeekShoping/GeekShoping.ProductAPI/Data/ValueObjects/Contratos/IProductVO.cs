namespace GeekShoping.ProductAPI.Data.ValueObjects
{
    public interface IProductVO
    {
        long Id { get; set; }
        string Name { get; set; }
        decimal Price { get; set; }
        string? Description { get; set; }
        string? CategoryName { get; set; }
        string? ImageURL { get; set; }
    }
}