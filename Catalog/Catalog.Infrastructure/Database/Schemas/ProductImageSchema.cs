namespace Catalog.Infrastructure.Database.Schemas;

public static class ProductImageSchema
{
    public static string Table => "product_images";

    public static class Columns
    {
        public static string Id => "id";
        public static string ProductId => "product_id";
        public static string ImageFileName => "image_file_name";
        public static string DisplayOrder => "display_order";
    }
}