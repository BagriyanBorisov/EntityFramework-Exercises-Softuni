using System.Text;
using BookShop.Models.Enums;

namespace BookShop;
using Data;
using Initializer;
using System.Linq;

public class StartUp
{
    public static void Main() 
        { 
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            string res = GetBooksByPrice(db);
            Console.WriteLine(res);

        }
    //02
    public static string GetBooksByAgeRestriction(BookShopContext db, string command)
    {
        bool hasParsed = Enum.TryParse(typeof(AgeRestriction), command, true, out object? ageRestrObj);
        if (hasParsed)
        {
            AgeRestriction ageRestriction = (AgeRestriction)ageRestrObj;

            var books = db.Books.ToArray()
                .Where(b => b.AgeRestriction == ageRestriction)
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();
            return string.Join(Environment.NewLine, books);
        }
        return null;
    }

    //03
    public static string GetGoldenBooks(BookShopContext context)
    {
        var booksInfo = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title).ToArray();

        return string.Join(Environment.NewLine, booksInfo);
    }
    //04
    public static string GetBooksByPrice(BookShopContext context)
    {
        StringBuilder sb = new StringBuilder();

        var booksInfo = context.Books.Where(b => b.Price > 40)
            .OrderByDescending(b => b.Price)
            .Select(b => new
            {
                b.Title,
                Price = b.Price.ToString("f2")
            }).ToArray();

        foreach (var b in booksInfo)
        {
            sb.AppendLine($"{b.Title} - ${b.Price}");
        }

        return sb.ToString().TrimEnd();
    }


}



