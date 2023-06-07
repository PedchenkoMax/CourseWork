namespace Catalog.Infrastructure.Database.Schemas;

public static class ProductSchema
{
    public static string Table => "products";

    public static class Columns
    {
        public static string Id => "id";
        public static string BrandId => "brand_id";
        public static string CategoryId => "category_id";
        public static string Slug => "slug";
        public static string Name => "name";
        public static string Description => "description";
        public static string Price => "price";
        public static string Discount => "discount";
        public static string SKU => "sku";
        public static string Stock => "stock";
        public static string Availability => "availability";
    }
}