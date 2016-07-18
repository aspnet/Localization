using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace OtherNamespace
{
    public class OutsideNamespaceClass
    {
        private IStringLocalizer<OutsideNamespaceClass> _localizer;

        public OutsideNamespaceClass()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IHostingEnvironment, HostingEnvironment>();
            services.AddMvc();
            services.AddOptions()
                .AddRouting()
                .AddLocalization(options => options.ResourcesPath = "Resources");

            var provider = services.BuildServiceProvider();

            _localizer = provider.GetRequiredService<IStringLocalizer<OutsideNamespaceClass>>();
        }

        public string GetData()
        {
            return _localizer["Hello"];
        }
    }
}
