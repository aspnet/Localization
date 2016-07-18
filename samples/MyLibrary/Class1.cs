using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace MyLibrary
{
    public class Class1
    {
        private IStringLocalizer<Class1> _localizer;

        public Class1()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IHostingEnvironment, HostingEnvironment>();
            services.AddMvc();
            services.AddOptions()
                .AddRouting()
                .AddLocalization(options => options.ResourcesPath = "Resources");

            var provider = services.BuildServiceProvider();

            _localizer = provider.GetRequiredService<IStringLocalizer<Class1>>();
        }

        public string GetData()
        {
            return _localizer["Hello"];
        }
    }
}
