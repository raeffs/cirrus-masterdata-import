using System;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Cirrus.Assortments;
using Cirrus.Import.Masterdata.Cirrus.Categories;
using Cirrus.Import.Masterdata.Cirrus.Groups;
using Cirrus.Import.Masterdata.Cirrus.Products;
using Cirrus.Import.Masterdata.Cirrus.Taxes;
using Cirrus.Import.Masterdata.Cirrus.Units;
using Cirrus.Import.Masterdata.Common;
using Cirrus.Import.Masterdata.Infrastructure;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cirrus.Import.Masterdata
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var configuation = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false, true)
#if DEBUG
                    .AddJsonFile("appsettings.development.json", true, true)
#endif
                    .Build();

                var services = new ServiceCollection();

                services.AddSingleton(configuation);

                services.AddSingleton(_ => configuation.GetSection("Api").Get<ApiOptions>());
                services.AddSingleton(_ => configuation.GetSection("Groups").Get<GroupOptions>());
                services.AddSingleton(_ => configuation.GetSection("Taxes").Get<TaxOptions>());

                services.AddSingleton<Importer>();

                services.AddSingleton<UnitApi>();
                services.AddSingleton<TaxApi>();
                services.AddSingleton<GroupApi>();
                services.AddSingleton<AssortmentApi>();
                services.AddSingleton<CategoryApi>();
                services.AddSingleton<ProductApi>();

                services.AddExternalProvider<External.Swapi.SwapiProvider, External.Swapi.SwapiOptions>("Swapi");
                services.AddExternalProvider<External.PunkApi.PunkApiProvider, External.PunkApi.PunkApiOptions>("PunkApi");
                services.AddExternalProvider<External.TheMealDb.TheMealDbProvider, External.TheMealDb.TheMealDbOptions>("TheMealDb");
                services.AddExternalProvider<External.TheCocktailDb.TheCocktailDbProvider, External.TheCocktailDb.TheCocktailDbOptions>("TheCocktailDb");
                services.AddExternalProvider<External.Fono.FonoProvider, External.Fono.FonoOptions>("Fono");
                services.AddExternalProvider<External.Brickset.BricksetProvider, External.Brickset.BricksetOptions>("Brickset");
                services.AddExternalProvider<External.PokemonTcg.PokemonTcgProvider, External.PokemonTcg.PokemonTcgOptions>("PokemonTcg");
                services.AddExternalProvider<External.Scryfall.ScryfallProvider, External.Scryfall.ScryfallOptions>("Scryfall");
                services.AddExternalProvider<External.CarQuery.CarQueryProvider, External.CarQuery.CarQueryOptions>("CarQuery");
                services.AddExternalProvider<External.Giantbomb.GiantbombProvider, External.Giantbomb.GiantbombOptions>("Giantbomb");

                services.AddSingleton<PollyHttpClientFactory>();

                var serviceProvider = services.BuildServiceProvider();

                FlurlHttp.Configure(settings => settings.HttpClientFactory = serviceProvider.GetService<PollyHttpClientFactory>());

                var importer = serviceProvider.GetService<Importer>();
                await importer.Import();

                Console.WriteLine("Import completed");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static IServiceCollection AddExternalProvider<TProvider, TOptions>(this IServiceCollection services, string configurationSectionName)
            where TOptions : class
            where TProvider : class, ExternalProvider
        {
            return services
                .AddSingleton(s => s.GetService<IConfigurationRoot>().GetSection(configurationSectionName).Get<TOptions>())
                .AddSingleton<ExternalProvider, TProvider>();
        }
    }
}
