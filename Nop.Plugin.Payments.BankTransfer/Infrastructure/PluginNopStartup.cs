﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.BankTransfer.Services;
using Nop.Plugin.Payments.BankTransfer.Services.Interfaces;

namespace Nop.Plugin.Payments.BankTransfer.Infrastructure
{
    public class PluginNopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ViewLocationExpander());
            });

            //register services and interfaces
            //services.AddScoped<CustomModelFactory, ICustomerModelFactory>();

            services.AddScoped<IBankTransferService, BankTransferService>();
            services.AddScoped<IRechargeKeyGenerator, RechargeKeyGenerator>();
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 11;
    }
}