using CultureWeb.Data;
using Microsoft.EntityFrameworkCore;

namespace CultureWeb.Models
{
    public class MainCategorySeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Check if data already exists
                if (context.MainCategories.Any())
                {
                    return; // Data has already been seeded
                }

                // Seed MainCategory data
                var mainCategories = new MainCategory[]
                {
                    new MainCategory
                    {
                        Name = "Product",
                        Description = "This categrory used for all Product",
                        Name_kh = "ផលិតផល",
                        Description_kh = "ប្រភេទនេះប្រើសម្រាប់ផលិតផលទាំងអស់។"
                    },
                    new MainCategory
                    {
                        Name = "Blog",
                        Description = "This categrory used for all Blog",
                        Name_kh = "អត្ថបទ",
                        Description_kh = "ប្រភេទនេះប្រើសម្រាប់អត្ថបទទាំងអស់។"
                    }
                    // Add more MainCategory objects as needed
                };

                context.MainCategories.AddRange(mainCategories);
                context.SaveChanges();
            }
        }
    }
}
