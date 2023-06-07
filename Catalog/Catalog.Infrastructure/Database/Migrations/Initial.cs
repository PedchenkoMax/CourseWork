using System.Data;
using Catalog.Infrastructure.Database.Schemas;
using FluentMigrator;

namespace Catalog.Infrastructure.Database.Migrations;

[Migration(1)]
public class Initial : Migration
{
    public override void Up()
    {
        #region Tables

        Create.Table(ProductSchema.Table)
              .WithColumn(ProductSchema.Columns.Id).AsGuid().PrimaryKey()
              .WithColumn(ProductSchema.Columns.BrandId).AsGuid().Nullable()
              .WithColumn(ProductSchema.Columns.CategoryId).AsGuid().Nullable()
              .WithColumn(ProductSchema.Columns.Slug).AsString().NotNullable()
              .WithColumn(ProductSchema.Columns.Name).AsString().NotNullable()
              .WithColumn(ProductSchema.Columns.Description).AsString().NotNullable()
              .WithColumn(ProductSchema.Columns.Price).AsDecimal().NotNullable()
              .WithColumn(ProductSchema.Columns.Discount).AsDecimal().NotNullable()
              .WithColumn(ProductSchema.Columns.SKU).AsString().NotNullable()
              .WithColumn(ProductSchema.Columns.Stock).AsInt32().NotNullable()
              .WithColumn(ProductSchema.Columns.Availability).AsBoolean().NotNullable();

        Create.Table(ProductImageSchema.Table)
              .WithColumn(ProductImageSchema.Columns.Id).AsGuid().PrimaryKey()
              .WithColumn(ProductImageSchema.Columns.ProductId).AsGuid().NotNullable()
              .WithColumn(ProductImageSchema.Columns.ImageFileName).AsString().NotNullable()
              .WithColumn(ProductImageSchema.Columns.DisplayOrder).AsInt32().NotNullable();

        Create.Table(CategorySchema.Table)
              .WithColumn(CategorySchema.Columns.Id).AsGuid().PrimaryKey()
              .WithColumn(CategorySchema.Columns.ParentCategoryId).AsGuid().Nullable()
              .WithColumn(CategorySchema.Columns.Name).AsString().NotNullable()
              .WithColumn(CategorySchema.Columns.Description).AsString().NotNullable()
              .WithColumn(CategorySchema.Columns.ImageFileName).AsString().Nullable();

        Create.Table(BrandSchema.Table)
              .WithColumn(BrandSchema.Columns.Id).AsGuid().PrimaryKey()
              .WithColumn(BrandSchema.Columns.Name).AsString().NotNullable()
              .WithColumn(BrandSchema.Columns.Description).AsString().NotNullable()
              .WithColumn(BrandSchema.Columns.ImageFileName).AsString().Nullable();

        #endregion

        #region Foreign Keys

        Create.ForeignKey("fk_products_brands")
              .FromTable(ProductSchema.Table).ForeignColumn(ProductSchema.Columns.BrandId)
              .ToTable(BrandSchema.Table).PrimaryColumn(BrandSchema.Columns.Id)
              .OnDelete(Rule.SetNull);

        Create.ForeignKey("fk_products_categories")
              .FromTable(ProductSchema.Table).ForeignColumn(ProductSchema.Columns.CategoryId)
              .ToTable(CategorySchema.Table).PrimaryColumn(CategorySchema.Columns.Id)
              .OnDelete(Rule.SetNull);

        Create.ForeignKey("fk_product_images_products")
              .FromTable(ProductImageSchema.Table).ForeignColumn(ProductImageSchema.Columns.ProductId)
              .ToTable(ProductSchema.Table).PrimaryColumn(ProductSchema.Columns.Id)
              .OnDelete(Rule.Cascade);

        Create.ForeignKey("fk_categories_parent_categories")
              .FromTable(CategorySchema.Table).ForeignColumn(CategorySchema.Columns.ParentCategoryId)
              .ToTable(CategorySchema.Table).PrimaryColumn(CategorySchema.Columns.Id)
              .OnDelete(Rule.SetNull);

        #endregion

        #region Indexes

        Create.Index("idx_products_brand_id")
              .OnTable(ProductSchema.Table)
              .OnColumn(ProductSchema.Columns.BrandId)
              .Ascending();

        Create.Index("idx_products_category_id")
              .OnTable(ProductSchema.Table)
              .OnColumn(ProductSchema.Columns.CategoryId)
              .Ascending();

        Create.Index("idx_products_slug")
              .OnTable(ProductSchema.Table)
              .OnColumn(ProductSchema.Columns.Slug)
              .Ascending();

        Create.Index("idx_products_price")
              .OnTable(ProductSchema.Table)
              .OnColumn(ProductSchema.Columns.Price)
              .Ascending();

        Create.Index("idx_products_discount")
              .OnTable(ProductSchema.Table)
              .OnColumn(ProductSchema.Columns.Discount)
              .Ascending();

        Create.Index("idx_product_images_product_id")
              .OnTable(ProductImageSchema.Table)
              .OnColumn(ProductImageSchema.Columns.ProductId)
              .Ascending();

        Create.Index("idx_categories_parent_category_id")
              .OnTable(CategorySchema.Table)
              .OnColumn(CategorySchema.Columns.ParentCategoryId)
              .Ascending();

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