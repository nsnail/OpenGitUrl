using System.Diagnostics;
using System.Security.Cryptography;
using CommandLine;
using Microsoft.Win32;
using ShellProgressBar;
using System.Text.RegularExpressions;

namespace OpenGitUrl;

internal class Program
{
    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o =>
            {
                if (o.InstallRightClickMenu)
                {
                    Registry.SetValue(@$"HKEY_CLASSES_ROOT\Directory\shell\{nameof(OpenGitUrl)}\command", "",
                        @$"""{Process.GetCurrentProcess().MainModule!.FileName}"" -f ""%1""");
                    Console.WriteLine("Window right-click menu was added!");
                    return;
                }

                if (o.UninstallRightClickMenu)
                {
                    Registry.ClassesRoot.OpenSubKey(@"Directory\shell", true)
                        ?.DeleteSubKeyTree(nameof(OpenGitUrl), false);
                    Console.WriteLine("Window right-click menu was removed!");
                    return;
                }

                if (!string.IsNullOrWhiteSpace(o.File) )
                {
                    var file = @$"{o.File}\.git\config";
                    if(File.Exists(file))
                    {
                        var content = File.ReadAllText(file);
                        Console.WriteLine(content);
                        var url = Regex.Match(content,"url = (http.*)").Groups[1].Value;
                        Process.Start("explorer.exe", url);
                    }
                }
                else
                {
                    Console.WriteLine(
                        $"try '{nameof(OpenGitUrl)} --help' for more information");
                }
            });
    }
}

internal class Options
{
    [Option('f', "file", Required = false, HelpText = "Enter the file path")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string File { get; init; }

    [Option('i', "install-right-click-menu", Required = false,
        HelpText = "Add right-click context menu in Windows")]
    public bool InstallRightClickMenu { get; init; }

    [Option('u', "uninstall-right-click-menu", Required = false,
        HelpText = "Remove right-click context menu in Windows")]
    public bool UninstallRightClickMenu { get; init; }
}