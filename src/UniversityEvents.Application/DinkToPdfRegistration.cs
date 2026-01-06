using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;

namespace UniversityEvents.Application;

internal static class DinkToPdfRegistration
{
    public static IServiceCollection AddDinkToPdf(this IServiceCollection services)
    {
        try
        {
            string platform = GetPlatform();
            string arch = GetArchitecture();

            string fileName = platform switch
            {
                "win" => "libwkhtmltox.dll",
                "linux" => "libwkhtmltox.so",
                "osx" => "libwkhtmltox.dylib",
                _ => throw new Exception("Unsupported OS platform.")
            };

            string nativePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "runtimes",
                $"{platform}-{arch}",
                "native",
                fileName
            );

            if (!File.Exists(nativePath))
                throw new FileNotFoundException($"Native PDF library not found: {nativePath}");

            var context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(nativePath);

            services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));
            return services;
        }
        catch (Exception ex)
        {

            throw;
        }


    }

    private static string GetPlatform()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return "win";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return "linux";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return "osx";
            throw new Exception("Unsupported OS");
        }
        catch (Exception)
        {

            throw;
        }

    }

    private static string GetArchitecture()
    {
        try
        {
            return RuntimeInformation.OSArchitecture switch
            {
                Architecture.X64 => "x64",
                Architecture.X86 => "x86",
                Architecture.Arm64 => "arm64",
                _ => throw new Exception("Unsupported architecture")
            };
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}
