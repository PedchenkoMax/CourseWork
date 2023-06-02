using System.Data;
using FluentMigrator;

namespace Catalog.Infrastructure.Database.Migrations;

[Migration(1)]
public class Initial : Migration
{
    public override void Up()
    {
        #region Product

        Create.Table("products")
              .WithColumn("id").AsGuid().PrimaryKey()
              .WithColumn("brand_id").AsGuid().Nullable()
              .WithColumn("category_id").AsGuid().Nullable()
              .WithColumn("slug").AsString().NotNullable()
              .WithColumn("name").AsString().NotNullable()
              .WithColumn("description").AsString().NotNullable()
              .WithColumn("price").AsDecimal().NotNullable()
              .WithColumn("discount").AsDecimal().NotNullable()
              .WithColumn("sku").AsString().NotNullable()
              .WithColumn("stock").AsInt32().NotNullable()
              .WithColumn("availability").AsBoolean().NotNullable();


        Create.ForeignKey("fk_products_brands")
              .FromTable("products").ForeignColumn("brand_id")
              .ToTable("brands").PrimaryColumn("id")
              .OnDelete(Rule.SetNull);

        Create.ForeignKey("fk_products_categories")
              .FromTable("products").ForeignColumn("category_id")
              .ToTable("categories").PrimaryColumn("id")
              .OnDelete(Rule.SetNull);


        Create.Index("idx_products_brand_id")
              .OnTable("products")
              .OnColumn("brand_id")
              .Ascending();

        Create.Index("idx_products_category_id")
              .OnTable("products")
              .OnColumn("category_id")
              .Ascending();

        Create.Index("idx_products_slug")
              .OnTable("products")
              .OnColumn("slug")
              .Ascending();

        Create.Index("idx_products_price")
              .OnTable("products")
              .OnColumn("price")
              .Ascending();

        Create.Index("idx_products_discount")
              .OnTable("products")
              .OnColumn("discount")
              .Ascending();

        #endregion

        #region Product Image

        Create.Table("product_images")
              .WithColumn("id").AsGuid().PrimaryKey()
              .WithColumn("product_id").AsGuid().NotNullable()
              .WithColumn("image_file_name").AsString().NotNullable()
              .WithColumn("display_order").AsInt32().NotNullable();


        Create.ForeignKey("fk_product_images_products")
              .FromTable("product_images").ForeignColumn("product_id")
              .ToTable("products").PrimaryColumn("id")
              .OnDelete(Rule.Cascade);


        Create.Index("idx_product_images_product_id")
              .OnTable("product_images")
              .OnColumn("product_id")
              .Ascending();

        #endregion

        #region Category

        Create.Table("categories")
              .WithColumn("id").AsGuid().PrimaryKey()
              .WithColumn("parent_category_id").AsGuid().Nullable()
              .WithColumn("name").AsString().NotNullable()
              .WithColumn("description").AsString().NotNullable()
              .WithColumn("image_file_name").AsString().NotNullable();


        Create.ForeignKey("fk_categories_parent_categories")
              .FromTable("categories").ForeignColumn("parent_category_id")
              .ToTable("categories").PrimaryColumn("id")
              .OnDelete(Rule.SetNull);


        Create.Index("idx_categories_parent_category_id")
              .OnTable("categories")
              .OnColumn("parent_category_id")
              .Ascending();

        #endregion

        #region Brand

        Create.Table("brands")
              .WithColumn("id").AsGuid().PrimaryKey()
              .WithColumn("name").AsString().NotNullable()
              .WithColumn("description").AsString().NotNullable()
              .WithColumn("image_file_name").AsString().NotNullable();

        #endregion
    }

    public override void Down()
    {
        Delete.Table("product_images");
        Delete.Table("products");
        Delete.Table("categories");
        Delete.Table("brands");
    }
}