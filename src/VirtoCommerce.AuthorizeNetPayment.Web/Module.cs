using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.AuthorizeNetPayment.Core;
using VirtoCommerce.AuthorizeNetPayment.Core.Models;
using VirtoCommerce.AuthorizeNetPayment.Core.Services;
using VirtoCommerce.AuthorizeNetPayment.Data.Providers;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.AuthorizeNetPayment.Web
{
    public class Module : IModule, IHasConfiguration
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public IConfiguration Configuration { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddOptions<AuthorizeNetPaymentMethodOptions>().Bind(Configuration.GetSection("Payments:AuthorizeNet")).ValidateDataAnnotations();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            // register settings
            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

            var authorizeNetOptions = appBuilder.ApplicationServices.GetRequiredService<IOptions<AuthorizeNetPaymentMethodOptions>>();
            var paymentMethodsRegistrar = appBuilder.ApplicationServices.GetRequiredService<IPaymentMethodsRegistrar>();
            paymentMethodsRegistrar.RegisterPaymentMethod(() => new AuthorizeNetPaymentMethod(
                appBuilder.ApplicationServices.GetService<IAuthorizeNetClient>(),
                appBuilder.ApplicationServices.GetService<IAuthorizeNetCheckoutService>(),
                authorizeNetOptions));

            settingsRegistrar.RegisterSettingsForType(ModuleConstants.Settings.General.AllSettings, nameof(AuthorizeNetPaymentMethodOptions));
        }

        public void Uninstall()
        {
            // do nothing in here
        }
    }
}
