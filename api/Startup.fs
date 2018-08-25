namespace API

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open FSharp.Data.Dapper

type Startup private () =

    member val private enableCors = false with get, set

    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        // Add framework services.

        // Configure CORS so we can call the API from the client.
        let corsOrigins = this.Configuration.["server.cors:origins"];
        if not (String.IsNullOrEmpty(corsOrigins)) then
            services.AddCors(
                fun options ->
                    options.AddPolicy(
                        "FromAppSettings",
                        fun builder -> builder.WithOrigins(corsOrigins.Split(";"))
                                              .AllowAnyMethod()
                                              .AllowAnyHeader() |> ignore
                    ) |> ignore
            ) |> ignore
            this.enableCors <- true

        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1) |> ignore
        services.AddSingleton<IConfiguration>(this.Configuration) |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =
        if this.enableCors then
            app.UseCors("FromAppSettings") |> ignore

        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
        else
            app.UseHsts() |> ignore

        app.UseHttpsRedirection() |> ignore
        app.UseMvc() |> ignore
        // This registers Option<'T> types with Dapper.
        OptionHandler.RegisterTypes()
        // Enable columns like decision_id to map to decisionId
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores <- true
        this.Configuration.["ConnectionString"] <-
            match this.Configuration.["DATABASE"] with
            | empty when String.IsNullOrEmpty(empty) -> (__SOURCE_DIRECTORY__ + "/bin/exchange.db")
            | valid -> valid

    member val Configuration : IConfiguration = null with get, set