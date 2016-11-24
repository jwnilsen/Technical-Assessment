using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HealthCatalyst.Startup))]
namespace HealthCatalyst
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            TelemetryConfiguration.Active.DisableTelemetry = true;
        }
    }
}
