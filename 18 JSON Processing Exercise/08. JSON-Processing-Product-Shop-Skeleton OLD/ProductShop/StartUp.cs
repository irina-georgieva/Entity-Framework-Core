using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.User;
using ProductShop.Models;
using AutoMapper;
using ProductShop.DTOs.Product;
using ProductShop.DTOs.Category;
using System.ComponentModel.DataAnnotations;
using AutoMapper.QueryableExtensions;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            //Mapper.Initialize(cfg => cfg.AddProfile(typeof(ProductShopProfile)));

            ProductShopContext dbContext = new ProductShopContext();

            //string inputJson = File.ReadAllText("../../../Datasets/users.json");
            //string inputJson = File.ReadAllText("../../../Datasets/products.json");
            //string inputJson = File.ReadAllText("../../../Datasets/categories.json");
            //string inputJson = File.ReadAllText("../../../Datasets/categories-products.json");

            //string outputJson = Path.Combine(Directory.GetCurrentDirectory(), "../../../Results/", "products -in -range.json");
            //string outputJson = Path.Combine(Directory.GetCurrentDirectory(), "../../../Results/", "users-sold-product.json");
            //string outputJson = Path.Combine(Directory.GetCurrentDirectory(), "../../../Results/", "categories-by-products.json");
            string outputJson = Path.Combine(Directory.GetCurrentDirectory(), "../../../Results/", "users-and-products.json");


            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();

            //Console.WriteLine($"Database was created");

            //string output = ImportUsers(dbContext, inputJson);
            //string output = ImportProducts(dbContext, inputJson);
            //string output = ImportCategories(dbContext, inputJson);
            //string output = ImportCategoryProducts(dbContext, inputJson);

            //string json = GetProductsInRange(dbContext);
            //string json = GetSoldProducts(dbContext);
            //string json = GetCategoriesByProductsCount(dbContext);
            string json = GetUsersWithProducts(dbContext);

            File.WriteAllText(outputJson, json);

            //Console.WriteLine(output);
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            ImportUserDto[] userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson);

            ICollection<User> users = new List<User>();
            foreach (ImportUserDto userDto in userDtos)
            {
                User user = Mapper.Map<User>(userDto);
                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            ImportProductDto[] productDtos = JsonConvert
                .DeserializeObject<ImportProductDto[]>(inputJson);
        
            ICollection<Product> validProducts = new List<Product>();

            //Mapping
            foreach (ImportProductDto productDto in productDtos)
            {
                Product product = Mapper.Map<Product>(productDto);
                validProducts.Add(product);
            }

            context.Products.AddRange(validProducts);
            context.SaveChanges();

            return $"Successfully imported {validProducts.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            ImportCategoryDto[] categoriesDto = JsonConvert
                .DeserializeObject<ImportCategoryDto[]>(inputJson);

            ICollection<Category> validCategories = new List<Category>();

            foreach (ImportCategoryDto categoryDto in categoriesDto)
            {
                if (!IsValid(categoryDto))
                {
                    continue;
                }

                Category category = Mapper.Map<Category>(categoryDto);
                validCategories.Add(category);
            }

            context.Categories.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            ImportCategoryProductsDto[] categoryProductsDtos = JsonConvert
                .DeserializeObject<ImportCategoryProductsDto[]>(inputJson);

            ICollection <CategoryProduct> validCp = new List<CategoryProduct>();
            foreach (ImportCategoryProductsDto cpDto in categoryProductsDtos)
            {
                CategoryProduct categoryProduct = Mapper.Map<CategoryProduct>(cpDto);
                validCp.Add(categoryProduct);
            }

            context.CategoryProducts.AddRange(validCp);
            context.SaveChanges();

            return $"Successfully imported {validCp.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            ExportInRangeProductDto[] products = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .ProjectTo<ExportInRangeProductDto>()
                .ToArray();

            string json = JsonConvert.SerializeObject(products, Formatting.Indented);
            return json;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            ExportUserWithSoldProductsDto[] users = context
                .Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ProjectTo<ExportUserWithSoldProductsDto>()
                .ToArray();

            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            return json;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context
                .Categories
                .OrderByDescending(c => c.CategoryProducts.Count())
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoryProducts.Count(),
                    averagePrice = c.CategoryProducts.Average(p => p.Product.Price).ToString("f2"),
                    totalRevenue = c.CategoryProducts.Sum(p => p.Product.Price).ToString("f2")
                })
                .ToArray();


            string json = JsonConvert.SerializeObject(categories, Formatting.Indented);
            return json;
        }

        //public static string AutoMapper_GetUsersWithProducts(ProductShopContext context)
        //{
            //ExportUsersWithFullProductInfoDto[] users = context
            //    .Users
            //    .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
            //    .OrderByDescending(u => u.ProductsSold.Count(p => p.BuyerId.HasValue))
            //    .ProjectTo<ExportUsersWithFullProductInfoDto>()
            //    .ToArray();

            //ExportUsersInfoDto serDto = new ExportUsersInfoDto()
            //{
            //    UsersCount = users.Length,
            //    Users = users
            //};

            //JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
            //{
            //    NullValueHandling = NullValueHandling.Ignore
            //};

            //string json = JsonConvert.SerializeObject(serDto, Formatting.Indented, serializerSettings);
            //return json;
        //}

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            ExportUsersInfoDto serDto = new ExportUsersInfoDto()
            {
                Users = context
                    .Users
                    .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
                    .OrderByDescending(u => u.ProductsSold.Count(p => p.BuyerId.HasValue))
                    .Select(u => new ExportUsersWithFullProductInfoDto()
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Age = u.Age,
                        SoldProductsInfo = new ExportSoldProductsFullInfoDto()
                        {
                            SoldProducts = u.ProductsSold
                            .Where(p => p.BuyerId.HasValue)
                            .Select(p => new ExportSoldProductShortInfoDto()
                            {
                                Name = p.Name,
                                Price = p.Price
                            })
                            .ToArray()
                        }
                    })
                    .ToArray()
            };

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(serDto, Formatting.Indented, serializerSettings);
            return json;
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult);
            return isValid;
        }

    }
}