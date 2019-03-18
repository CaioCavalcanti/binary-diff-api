using Microsoft.Extensions.DependencyInjection;

namespace BinaryDiff.Shared.WebApi.Extensions
{
    public static class MvcBuilderExtensions
    {
        public static void ConfigureJsonSerializerSettings(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder
                .AddJsonOptions(opt =>
                 {
                     opt.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                     opt.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                 });
        }
    }
}
