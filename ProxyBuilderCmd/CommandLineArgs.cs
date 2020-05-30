using CmdLine;

[CommandLineArguments(Program = "proxybuilder", Title = "T4 Proxy Builder", Description = "Create CDS Proxies using T4 templates.")]
public class CommandLineArgs
{
    [CommandLineParameter(Command = "?", Default = false, Description = "Show Help", Name = "Help", IsHelp = true)]
    public bool Help { get; set; }

    [CommandLineParameter(Name = "settings", ParameterIndex = 1, Required = false, Description = "Optional path to the settings json file")]
    public string Path { get; set; }   

    [CommandLineParameter(Name = "connection", ParameterIndex = 2, Required = false, Description = "Optional Connection String")]
    public string Connection { get; set; }

    [CommandLineParameter(Name = "Wait for keypress", Command = "w", Required = false, Description = "Optional wait for a key press at the end of task run")]
    public bool WaitForKey { get; set; } = true;

    [CommandLineParameter(Name = "Ignore Windows login", Command = "i", Required = false, Description = "Optional flag to ignore logged in windows credentials during discovery and always ask for username/password.")]
    public bool IgnoreLocalPrincipal { get; set; }
}

