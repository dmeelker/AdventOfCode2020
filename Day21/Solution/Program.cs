using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Solution
{
    public class Program
    {
        static void Main(string[] args)
        {
            var input = Parser.ParseInput(File.ReadAllText("input.txt"));
            var (part1, part2) = Solve(input);

            Console.WriteLine($"Part 1: {part1} Part 2: {part2}");
        }

        public static (int,string) Solve(Food[] input)
        {
            var allAllergens = input.SelectMany(food => food.Allergens).Distinct().ToArray();
            var allergens = new Dictionary<string, HashSet<string>>();

            foreach(var food in input)
            {
                foreach (var allergen in food.Allergens)
                {
                    if (allergens.ContainsKey(allergen))
                    {
                        allergens[allergen] = allergens[allergen].Intersect(food.Ingredients.Distinct()).ToHashSet();
                    }
                    else
                    {
                        allergens[allergen] = food.Ingredients.ToHashSet();
                    }
                }
            }

            while (allergens.Values.Any(x => x.Count != 1))
            {
                var singles = allergens.Where(a => a.Value.Count == 1);
                foreach (var single in singles)
                {
                    allergens = allergens.ToDictionary(
                        a => a.Key,
                        a => a.Key == single.Key ? a.Value : a.Value.Except(new[] { single.Value.Single() }).ToHashSet()
                    );
                }
            }

            var badIngredients = allergens.SelectMany(a => a.Value).ToHashSet();

            var part1 = input.Sum(food => food.Ingredients.Count(i => !badIngredients.Contains(i)));

            var part2 = allergens.Select(a => (allergen: a.Key, ingredient: a.Value.Single()))
                .OrderBy(i => i.allergen)
                .Select(i => i.ingredient)
                .Aggregate((i1, i2) => i1 + "," + i2);

            return (part1, part2);
        }
    }
}

