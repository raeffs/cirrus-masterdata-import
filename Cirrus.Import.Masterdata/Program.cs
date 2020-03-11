using System;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Cirrus.Assortments;
using Cirrus.Import.Masterdata.Cirrus.Categories;
using Cirrus.Import.Masterdata.Cirrus.Groups;
using Cirrus.Import.Masterdata.Cirrus.Products;
using Cirrus.Import.Masterdata.Cirrus.Taxes;
using Cirrus.Import.Masterdata.Cirrus.Units;
using Cirrus.Import.Masterdata.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cirrus.Import.Masterdata
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var configuation = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false)
                    .AddJsonFile("appsettings.development.json", true)
                    .Build();

                var services = new ServiceCollection();

                services.AddSingleton(_ => configuation.GetSection("Api").Get<ApiOptions>());
                services.AddSingleton(_ => configuation.GetSection("Groups").Get<GroupOptions>());
                services.AddSingleton(_ => configuation.GetSection("Taxes").Get<TaxOptions>());
                services.AddSingleton(_ => configuation.GetSection("TheMealDb").Get<External.TheMealDb.TheMealDbOptions>());
                services.AddSingleton(_ => configuation.GetSection("TheCocktailDb").Get<External.TheCocktailDb.TheCocktailDbOptions>());
                services.AddSingleton(_ => configuation.GetSection("Scryfall").Get<External.Scryfall.ScryfallOptions>());
                services.AddSingleton(_ => configuation.GetSection("PokemonTcg").Get<External.PokemonTcg.PokemonTcgOptions>());
                services.AddSingleton(_ => configuation.GetSection("Brickset").Get<External.Brickset.BricksetOptions>());
                services.AddSingleton(_ => configuation.GetSection("PunkApi").Get<External.PunkApi.PunkApiOptions>());

                services.AddSingleton<Importer>();

                services.AddSingleton<UnitApi>();
                services.AddSingleton<TaxApi>();
                services.AddSingleton<GroupApi>();
                services.AddSingleton<AssortmentApi>();
                services.AddSingleton<CategoryApi>();
                services.AddSingleton<ProductApi>();

                services.AddSingleton<ExternalProvider, External.TheMealDb.TheMealDbProvider>();
                services.AddSingleton<ExternalProvider, External.TheCocktailDb.TheCocktailDbProvider>();
                services.AddSingleton<ExternalProvider, External.Scryfall.ScryfallProvider>();
                services.AddSingleton<ExternalProvider, External.PokemonTcg.PokemonTcgProvider>();
                services.AddSingleton<ExternalProvider, External.Brickset.BricksetProvider>();
                services.AddSingleton<ExternalProvider, External.PunkApi.PunkApiProvider>();

                var importer = services.BuildServiceProvider().GetService<Importer>();
                await importer.Import();

                Console.WriteLine("Import completed");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
