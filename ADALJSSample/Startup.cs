using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ADALJSSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //Uncomment these lines to enable authorization.
            //This sample authorization handler looks for the specified groupId
            //in the claims of the logged in user.
            //To enable this, configure the groupMembershipClaims key in 
            //your app registration's manifest. More information here:
            //https://docs.microsoft.com/en-us/azure/active-directory/develop/reference-app-manifest
            
            //services.AddSingleton<IAuthorizationHandler, MemberOfValidGroupHandler>();

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("MemberOfValidGroup", policy =>
            //        policy.Requirements.Add(new MemberOfValidGroupRequirement("TODO: SET GROUP ID HERE")));
            //});

            services.AddAuthentication(AzureADDefaults.BearerAuthenticationScheme)
                .AddAzureADBearer(options => 
                Configuration.Bind("AzureAd", options));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            

            services.Configure<AzureADOptions>(Configuration.GetSection("AzureAd"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication(); //<-- The order of this is very important!

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    public class MemberOfValidGroupRequirement : IAuthorizationRequirement
    {
        public string GroupId { get; private set; }

        public MemberOfValidGroupRequirement(string groupId)
        {
            GroupId = groupId;
        }
    }

    public class MemberOfValidGroupHandler : AuthorizationHandler<MemberOfValidGroupRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       MemberOfValidGroupRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "groups" &&
                                            c.Value == requirement.GroupId))
            {
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

}
