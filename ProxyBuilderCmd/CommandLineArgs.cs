using CmdLine;

[CommandLineArguments(Program = "proxybuilder", Title = "T4 Proxy Builder", Description = "Create CDS Proxies using T4 templates.")]
public class CommandLineArgs
{
    [CommandLineParameter(Command = "?", Default = false, Description = "Show Help", Name = "Help", IsHelp = true)]
    public bool Help { get; set; }

    [CommandLineParameter(Name = "settingspath", Command = "s", Required = false, Description = "Optional search path for the proxies.json file. When omitted search begins one level above the folder containing the proxybuilder executable.")]
    public string Path { get; set; }   

    [CommandLineParameter(Name = "connection", Command = "c", Required = false, Description = "Optional Connection String. When omitted executable will pause for credentials.")]
    public string Connection { get; set; }

    [CommandLineParameter(Name = "Wait for keypress", Command = "w", Required = false, Description = "Optional wait for a key press at the end of task run. When omitted the executable will exit after completion.")]
    public bool WaitForKey { get; set; } = false;

    [CommandLineParameter(Name = "Ignore Windows login", Command = "i", Required = false, Description = "Optional flag to ignore logged in windows credentials during discovery and always ask for username/password.")]
    public bool IgnoreLocalPrincipal { get; set; }

    [CommandLineParameter(Name = "Use Legacy Login", Command ="l", Required = false)]
    public bool LegacyLogin { get; set; }
}

