using FluentMigrator;

namespace Catalog.Infrastructure.Database.Migrations;

[Migration(1)]
public class Initial : Migration
{
    public override void Up()
    {
        Create.Table("Brands")
              .WithColumn("Id").AsGuid().PrimaryKey()
              .WithColumn("Name").AsString().NotNullable()
              .WithColumn("Description").AsString().NotNullable()
              .WithColumn("ImageUrl").AsString().NotNullable()
              .WithColumn("DisplayOrder").AsInt32().NotNullable();

        Create.Table("Categories")
              .WithColumn("Id").AsGuid().PrimaryKey()
              .WithColumn("ParentCategoryId").AsGuid().Nullable()
              .WithColumn("Name").AsString().NotNullable()
              .WithColumn("Description").AsString().NotNullable()
              .WithColumn("ImageUrl").AsString().NotNullable()
              .WithColumn("DisplayOrder").AsInt32().NotNullable();

        Create.Table("Products")
              .WithColumn("Id").AsGuid().PrimaryKey()
              .WithColumn("BrandId").AsGuid().NotNullable()
              .WithColumn("CategoryId").AsGuid().NotNullable()
              .WithColumn("Name").AsString().NotNullable()
              .WithColumn("Description").AsString().NotNullable()
              .WithColumn("Price").AsDecimal().NotNullable()
              .WithColumn("Discount").AsDecimal().NotNullable()
              .WithColumn("SKU").AsString().NotNullable()
              .WithColumn("Stock").AsInt32().NotNullable()
              .WithColumn("Availability").AsBoolean().NotNullable();

        Create.Table("ProductImages")
              .WithColumn("Id").AsGuid().PrimaryKey()
              .WithColumn("ProductId").AsGuid().NotNullable()
              .WithColumn("ImageUrl").AsString().NotNullable()
              .WithColumn("DisplayOrder").AsInt32().NotNullable();

        Create.ForeignKey("FK_Products_Brands")
              .FromTable("Products").ForeignColumn("BrandId")
              .ToTable("Brands").PrimaryColumn("Id");

        Create.ForeignKey("FK_Products_Categories")
              .FromTable("Products").ForeignColumn("CategoryId")
              .ToTable("Categories").PrimaryColumn("Id");

        Create.ForeignKey("FK_ProductImages_Products")
              .FromTable("ProductImages").ForeignColumn("ProductId")
              .ToTable("Products").PrimaryColumn("Id");
    }

    public override void Down()
    {
        Delete.Table("ProductImages");
        Delete.Table("Products");
        Delete.Table("Categories");
        Delete.Table("Brands");
    }
}