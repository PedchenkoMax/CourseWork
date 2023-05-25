namespace Catalog.Infrastructure.Database.Schemas;

public static class ProductImageSchema
{
    public static string Table => "product_images";

    public static class Columns
    {
        public static string Id => "id";
        public static string ProductId => "product_id";
        public static string ImageUrl => "image_url";
        public static string DisplayOrder => "display_order";
    }
}