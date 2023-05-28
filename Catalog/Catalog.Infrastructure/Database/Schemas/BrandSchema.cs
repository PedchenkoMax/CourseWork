namespace Catalog.Infrastructure.Database.Schemas;

public static class BrandSchema
{
    public static string Table => "brands";

    public static class Columns
    {
        public static string Id => "id";
        public static string Name => "name";
        public static string Description => "description";
        public static string ImageFileName => "image_file_name";
        public static string DisplayOrder => "display_order";
    }
}