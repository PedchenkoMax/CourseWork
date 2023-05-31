namespace Catalog.Infrastructure.Database.Schemas;

public static class CategorySchema
{
    public static string Table => "categories";

    public static class Columns
    {
        public static string Id => "id";
        public static string ParentCategoryId => "parent_category_id";
        public static string Name => "name";
        public static string Description => "description";
        public static string ImageFileName => "image_file_name";
    }
}