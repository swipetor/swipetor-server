using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using AutoMapper;
using CloneExtensions;
using Google.Apis.Util;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SwipetorApp;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.DTOs;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Auth;
using SwipetorApp.Services.BackgroundTasks;
using SwipetorApp.Services.Config;
using SwipetorApp.Services.PhotoServices;
using SwipetorApp.Services.Security;
using SwipetorApp.System;
using SwipetorApp.System.Binders;
using SwipetorApp.System.MvcFilters;
using SwipetorApp.System.UserRoleAuth;
using WebAppShared.Config;
using WebAppShared.Contexts;
using WebAppShared.DI;
using WebAppShared.Exceptions;
using WebAppShared.Http;
using WebAppShared.Internal;
using WebAppShared.SharedLogic.Recaptcha;
using WebAppShared.Types;
using WebAppShared.Uploaders;
using WebAppShared.Uploaders.R2;
using WebAppShared.Videos;
using WebAppShared.WebPush;
using WebAppShared.WebSys;
using WebAppShared.WebSys.DI;
using WebAppShared.WebSys.MvcFilters;
using WebAppShared.WebSys.MvcRouting;

var builder = WebApplication.CreateBuilder(args);

#region Services

var s = builder.Services;
// Add services to the container.
var controllersWithViews = s.AddControllersWithViews(opts =>
{
    opts.Filters.Add<UpdateLastOnlineActionFilter>();
    opts.Filters.Add<WhitelistCountryAccessFilter>();

    opts.ModelBinderProviders.RemoveType<DateTimeModelBinderProvider>();
    opts.ModelBinderProviders.Insert(0, new CurrencyModelBinderProvider());
    var complexModelBinderProvider = opts.ModelBinderProviders.OfType<ComplexObjectModelBinderProvider>();

    opts.Filters.Add(new UserRoleAuthByAreaFilter(UserRole.HostMaster, AreaNames.HostMaster));
    opts.Filters.Add(new UserRoleAuthByAreaFilter(UserRole.Admin, AreaNames.Admin));

    opts.Filters.Add<GlobalExceptionFilter>();
}).AddNewtonsoftJson(opts =>
{
    opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
});

s.AddRazorPages();

if (builder.Environment.IsDevelopment()) controllersWithViews.AddRazorRuntimeCompilation();

s.AddLogging(options =>
{
    options.AddSimpleConsole(c =>
    {
        c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
        // c.UseUtcTimestamp = true; // something to consider
    });
    // options.AddFile("", LogLevel.Information, null, false, 1024 * 1024 * 1024, 5);
});

s.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

s.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

s.AddRouting(options =>
{
    options.LowercaseUrls = true;

    // Replace the type and the name used to refer to it with your own
    // IOutboundParameterTransformer implementation
    options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
    options.ConstraintMap["api"] = typeof(ApiParameterTransformer);
});

s.AddDbContext<DbCx>(opts => opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

#region AppConfig, reading local JSON config file

s.Configure<AppConfig>(builder.Configuration.GetSection("AppConfig"));
s.Configure<SiteConfig>(builder.Configuration.GetSection("AppConfig:Site"));
s.Configure<HostMasterConfig>(builder.Configuration.GetSection("AppConfig:HostMaster"));
s.Configure<SMTPConfig>(builder.Configuration.GetSection("AppConfig:SMTP"));
s.Configure<InfluxConfig>(builder.Configuration.GetSection("AppConfig:Influx"));
s.Configure<FirebaseConfig>(builder.Configuration.GetSection("AppConfig:Firebase"));
s.Configure<R2Config>(builder.Configuration.GetSection("AppConfig:R2"));
s.Configure<RecaptchaConfig>(builder.Configuration.GetSection("AppConfig:Recaptcha"));
s.Configure<StorageConfig>(builder.Configuration.GetSection("AppConfig:Storage"));

#endregion

s.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.Events.OnRedirectToAccessDenied =
            cx => throw new HttpStatusCodeException(HttpStatusCode.Forbidden);
        options.Events.OnRedirectToLogin =
            cx => throw new HttpStatusCodeException(HttpStatusCode.Forbidden);
    });

s.AddAuthorization(options =>
{
    foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
        options.AddPolicy($"MinimumRole{role}",
            policy => policy.Requirements.Add(new MinimumRoleRequirement(role)));
});

s.AddSingleton<IAuthorizationHandler, MinimumRoleHandler>();
if (builder.Environment.IsDevelopment()) s.AddSingleton<IHostLifetime, NoopConsoleLifetime>();

s.AddHttpContextAccessor();
s.AddDatabaseDeveloperPageExceptionFilter();

s.AddSingleton<IAuthorizationHandler, MinimumRoleHandler>();
s.AddSingleton<IMapper>(new Mapper(MapperConfig.Config));
s.AddSingleton(provider => new ServiceLocator(provider));
s.AddTransient(typeof(IFactory<>), typeof(Factory<>));
s.AddTransient<IFileUploader, R2FileUploader>();
s.AddTransient<IFileDeleter, R2FileDeleter>();
s.AddTransient<VideoWithPreviewUploader>();
s.AddTransient<VideoUploader>();
s.AddTransient<PhotoSaverSvc>();
s.AddTransient(sp => sp.GetService<IOptions<InfluxConfig>>().Value);
s.AddTransient<IHeaderAnalyzer, HeaderAnalyzer>();
s.AddTransient<IConnectionCx, ConnectionCx>();
s.AddTransient<VideoPreviewGenerator>();
s.AddTransient<IRecaptchaConfig>(sp => sp.GetService<RecaptchaCredsSvc>());
s.AddTransient<LocalVideoPathProvider>();
s.AddTransient(sp =>
    sp.GetService<IMapper>()!.Map<R2FileUploaderConfig>(sp.GetService<IOptions<R2Config>>().Value));
s.AddTransient<IHostnameCx>(sp => sp.GetService<HostnameCx>());

CloningInitializer.Init();

// Register services through ServiceAttribute
ServiceAttribute.RegisterServices(Assembly.GetExecutingAssembly(), s);
ServiceAttribute.RegisterServices(Assembly.Load("WebAppShared"), s);

//			services.TryAddEnumerable(ServiceDescriptor.Singleton<IFilterProvider, AdminAreaAuthFilterProvider>());
//			AutoMapperConfiguration.Configure();

var persistKeysDir = builder.Configuration["AppConfig:PersistKeysDir"];
persistKeysDir.ThrowIfNullOrEmpty("AppConfig:PersistKeysDir");

s.AddDataProtection()
    // This helps surviving a restart: a same app will find back its keys. Just ensure to create the folder.
    .PersistKeysToFileSystem(new DirectoryInfo(persistKeysDir!))
    // This helps surviving a site update: each app has its own store, building the site creates a new app
    .SetApplicationName("swipetor")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

s.AddHttpClient("chrome", ConfigureHttpClient.ChromeClient);

s.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

#endregion

s.AddHostedService<PostNotifCreatorBgSvc>();
// s.AddHostedService<PmEmailBgSvc>();
// s.AddHostedService<NotificationEmailBgSvc>();
s.AddHostedService<NotifWebPushBgSvc>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

ServiceLocator.SetLocatorProvider(app.Services);

var loggerFactory = app.Services.GetService<ILoggerFactory>();
WebAppSharedDefaults.SetLoggerFactory(loggerFactory);

loggerFactory.AddFile("/var/log/swipetor/app.log", fileSizeLimitBytes: 104857600, isJson: false,
    minimumLevel: LogLevel.Information, retainedFileCountLimit: 3);
AppDefaults.LoggerFactory.Value = loggerFactory;

app.UseForwardedHeaders();
// app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        if (builder.Environment.IsDevelopment())
        {
            context.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store");
            context.Context.Response.Headers.Append("Expires", "-1");
        }
    }
});

app.UseMiddleware<ContentSecurityPolicyMiddleware>();
app.UseMiddleware<RedirectExceptionMiddleware>();
app.UseMiddleware<CustomExceptionMiddleware>();
app.UseCookiePolicy();
app.UseStatusCodePages();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

#region Routes

app.MapControllerRoute(
    "areaRoute",
    "{area:exists}/{controller:slugify=Home}/{action:slugify=Index}/{id?}");

app.MapControllerRoute("default", "{controller:slugify=Home}/{action:slugify=Index}/{id?}");

app.MapRazorPages();

// endpoints.MapAreaControllerRoute(name: "site", AreaNames.Site,
//     "site/{controller=Home}/{action=Index}/{id?}");

app.MapFallbackToController("Index", "Fallback");

#endregion

// Make CamelCase the default contract resolved when outside of controllers' serialising context
JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    ContractResolver = new CamelCasePropertyNamesContractResolver()
};

FirebaseInit.Init(builder.Environment);

AppEnv.IsDevelopment.Value = builder.Environment.IsDevelopment();
AppEnv.IsProduction.Value = builder.Environment.IsProduction();

app.Run();