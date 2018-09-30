using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MyDiary.Startup))]

namespace MyDiary
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}