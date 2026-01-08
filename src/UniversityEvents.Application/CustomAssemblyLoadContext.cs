using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace UniversityEvents.Application;

public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    /// <summary>
    /// Load unmanaged library from absolute path
    /// </summary>
    /// <param name="absolutePath"></param>
    /// <returns>Pointer to loaded library</returns>
    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        return LoadUnmanagedDll(absolutePath);
    }

    /// <summary>
    /// Override to load unmanaged DLL
    /// </summary>
    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        return NativeLibrary.Load(unmanagedDllName);
    }

    /// <summary>
    /// We don't load any managed assembly here
    /// </summary>
    protected override Assembly Load(AssemblyName assemblyName)
    {
        return null;
    }
}

