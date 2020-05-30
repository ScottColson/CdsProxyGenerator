using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text.RegularExpressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Crm.Sdk.Samples;
using CmdLine;
using CCLLC.CDS.ProxyGenerator;
using CCLLC.CDS.ProxyBuilderCmd;
using CCLLC.CDS.ProxyBuilderCmd.Extensions;
using System.CodeDom;

class Program
{
    static void Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("CDS Proxy Builder v" + Assembly.GetEntryAssembly().GetName().Version);

        Console.ForegroundColor = ConsoleColor.Gray;
        bool error = false;
        CommandLineArgs arguments = null;

        try
        {
            arguments = CommandLine.Parse<CommandLineArgs>();

            Run(arguments);
        }
        catch (CommandLineException exception)
        {
            Console.WriteLine(exception.ArgumentHelp.Message);
            Console.WriteLine(exception.ArgumentHelp.GetHelpText(Console.BufferWidth));
        }
        catch (FaultException<OrganizationServiceFault> ex)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("The application terminated with an error.");
            Console.WriteLine("Timestamp: {0}", ex.Detail.Timestamp);
            Console.WriteLine("Code: {0}", ex.Detail.ErrorCode);
            Console.WriteLine("Message: {0}", ex.Detail.Message);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(ex.StackTrace);

            if (!string.IsNullOrEmpty(ex.Detail.TraceText))
            {
                Console.WriteLine("Plugin Trace: {0}", ex.Detail.TraceText);
            }
            if (ex.Detail.InnerFault != null)
            {
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
            }
            error = true;
            Console.ForegroundColor = ConsoleColor.White;
        }
        catch (TimeoutException ex)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("The application terminated with an error.");
            Console.WriteLine("Message: {0}", ex.Message);
            Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
            if (ex.InnerException != null)
            {
                Console.WriteLine("Inner Fault: {0}", ex.InnerException.Message ?? "No Inner Fault");
            }
            error = true;
            Console.ForegroundColor = ConsoleColor.White;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("The application terminated with an error.");
            Console.WriteLine(ex.Message);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(ex.StackTrace);

            // Display the details of the inner exception.
            if (ex.InnerException != null)
            {
                Console.WriteLine(ex.InnerException.Message);

                if (ex.InnerException is FaultException<OrganizationServiceFault> fe)
                {
                    Console.WriteLine("Timestamp: {0}", fe.Detail.Timestamp);
                    Console.WriteLine("Code: {0}", fe.Detail.ErrorCode);
                    Console.WriteLine("Message: {0}", fe.Detail.Message);
                    if (!string.IsNullOrEmpty(fe.Detail.TraceText))
                    {
                        Console.WriteLine("Plugin Trace: {0}", fe.Detail.TraceText);
                    }
                    if (fe.Detail.InnerFault != null)
                    {
                        Console.WriteLine("Inner Fault: {0}",
                            null == fe.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
                    }
                }
            }
            error = true;
            Console.ForegroundColor = ConsoleColor.White;
        }
        finally
        {
            if (error)
            {
                Environment.ExitCode = 1;
            }
        }

        if (arguments != null && arguments.WaitForKey == true)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        Console.ForegroundColor = ConsoleColor.Gray;
    }

    private static void Run(CommandLineArgs arguments)
    {
        try
        {

            //var trace = new TraceLogger();
            var executingDirectory = Environment.CurrentDirectory;
            var searchPath = Path.Combine(executingDirectory, "..\\..");

            if (arguments.Connection == null)
            {
                // No Connection is supplied to ask for connection on command line 
                ServerConnection serverConnect = new ServerConnection();
                ServerConnection.Configuration config = serverConnect.GetServerConfiguration(arguments.IgnoreLocalPrincipal);

                arguments.Connection = BuildConnectionString(config);

                using (var serviceProxy = new OrganizationServiceProxy(config.OrganizationUri, config.HomeRealmUri, config.Credentials, config.DeviceCredentials))
                {
                    // This statement is required to enable early-bound type support.
                    serviceProxy.EnableProxyTypes();
                    serviceProxy.Timeout = new TimeSpan(1, 0, 0);
                    BuildProxy(serviceProxy, searchPath);
                }
            }

            else
            {
                // Does the connection contain a password prompt?
                var passwordMatch = Regex.Match(arguments.Connection, "Password=[*]+", RegexOptions.IgnoreCase);
                if (passwordMatch.Success)
                {
                    // Prompt for password
                    Console.WriteLine("Password required for connection {0}", arguments.Connection);
                    Console.Write("Password:");
                    var password = ReadPassword('*');
                    arguments.Connection = arguments.Connection.Replace(passwordMatch.Value, "Password=" + password);
                }

                using (var serviceProxy = new CrmServiceClient(arguments.Connection))
                {
                    if (serviceProxy.OrganizationServiceProxy == null)
                    {
                        throw new Exception(String.Format("Error connecting to the Organization Service Proxy: {0}", serviceProxy.LastCrmError));
                    }

                    serviceProxy.OrganizationServiceProxy.Timeout = new TimeSpan(1, 0, 0);
                    if (!serviceProxy.IsReady)
                    {
                        // trace.WriteLine("Not Ready {0} {1}", serviceProxy.LastCrmError, serviceProxy.LastCrmException);
                    }

                    BuildProxy(serviceProxy, searchPath);
                }
            }
        }
        catch (CommandLineException exception)
        {
            Console.WriteLine(exception.ArgumentHelp.Message);
            Console.WriteLine(exception.ArgumentHelp.GetHelpText(Console.BufferWidth));
        }

    }

    private static string BuildConnectionString(ServerConnection.Configuration config)
    {
        //string onlineRegion, organisationName;
        //bool isOnPrem;
        //Utilities.GetOrgnameAndOnlineRegionFromServiceUri(config.OrganizationUri, out onlineRegion, out organisationName, out isOnPrem);
        string connectionString;

        // On prem connection
        // AuthType = AD; Url = http://contoso:8080/Test;

        // AuthType=AD;Url=http://contoso:8080/Test; Domain=CONTOSO; Username=jsmith; Password=passcode

        // Office 365 
        // AuthType = Office365; Username = jsmith@contoso.onmicrosoft.com; Password = passcode; Url = https://contoso.crm.dynamics.com

        // IFD
        // AuthType=IFD;Url=http://contoso:8080/Test; HomeRealmUri=https://server-1.server.com/adfs/services/trust/mex/;Domain=CONTOSO; Username=jsmith; Password=passcode

        switch (config.EndpointType)
        {
            case AuthenticationProviderType.ActiveDirectory:
                connectionString = String.Format("AuthType=AD;Url={0}", config.OrganizationUri);

                break;
            case AuthenticationProviderType.Federation:
                connectionString = String.Format("AuthType=IFD;Url={0}", config.OrganizationUri);
                break;

            case AuthenticationProviderType.OnlineFederation:
                connectionString = String.Format("AuthType=Office365;Url={0}", config.OrganizationUri);
                break;

            default:
                throw new Exception(String.Format("Unsupported Endpoint Type {0}", config.EndpointType.ToString()));
        }

        if (config.Credentials != null && config.Credentials.Windows != null)
        {
            if (!String.IsNullOrEmpty(config.Credentials.Windows.ClientCredential.Domain))
            {
                connectionString += ";DOMAIN=" + config.Credentials.Windows.ClientCredential.Domain;
            }

            if (!String.IsNullOrEmpty(config.Credentials.Windows.ClientCredential.UserName))
            {
                connectionString += ";Username=" + config.Credentials.Windows.ClientCredential.UserName;
            }

            if (!String.IsNullOrEmpty(config.Credentials.Windows.ClientCredential.Password))
            {
                connectionString += ";Password=" + config.Credentials.Windows.ClientCredential.Password;
            }
            else if (config.Credentials.Windows.ClientCredential.SecurePassword != null && config.Credentials.Windows.ClientCredential.SecurePassword.Length > 0)
            {
                var password = new System.Net.NetworkCredential(string.Empty, config.Credentials.Windows.ClientCredential.SecurePassword).Password;
                connectionString += ";Password=" + password;
            }
        }

        if (config.Credentials != null)
        {
            if (!String.IsNullOrEmpty(config.Credentials.UserName.UserName))
            {
                connectionString += ";Username=" + config.Credentials.UserName.UserName;
            }
            if (!String.IsNullOrEmpty(config.Credentials.UserName.Password))
            {
                connectionString += ";Password=" + config.Credentials.UserName.Password;
            }
        }

        if (config.HomeRealmUri != null)
        {
            connectionString += ";HomeRealmUri=" + config.HomeRealmUri.ToString();
        }

        return connectionString;
    }


    public static string ReadPassword(char mask)
    {
        const int ENTER = 13, BACKSP = 8, CTRLBACKSP = 127;
        int[] FILTERED = { 0, 27, 9, 10 /*, 32 space, if you care */ }; // const

        var pass = new Stack<char>();
        char chr = (char)0;

        while ((chr = Console.ReadKey(true).KeyChar) != ENTER)
        {
            if (chr == BACKSP)
            {
                if (pass.Count > 0)
                {
                    Console.Write("\b \b");
                    pass.Pop();
                }
            }
            else if (chr == CTRLBACKSP)
            {
                while (pass.Count > 0)
                {
                    Console.Write("\b \b");
                    pass.Pop();
                }
            }
            else if (FILTERED.Count(x => chr == x) > 0) { }
            else
            {
                pass.Push((char)chr);
                Console.Write(mask);
            }
        }

        Console.WriteLine();

        return new string(pass.Reverse().ToArray());
    }


    private static void BuildProxy(IOrganizationService organizationService, string searchPath)
    {        
       

        var settings = GetSettingsFromProxyConfigFile(searchPath) 
            ??  GetSettingsFromSpklConfigFile(searchPath) 
            ?? throw new Exception("Unable to load settings.");

        foreach (var setting in settings)
        {
            
            var metadataService = ServiceContainer.Resolve<ICDSMetadataServiceFactory>().Create(setting);
            metadataService.Message += MessageHandler;
            var entityMetadata = metadataService.GetEntityMetadata(organizationService);
            var sdkMessageMetadata = metadataService.GetMessageMetadata(organizationService);
            metadataService.Message -= MessageHandler;

            var typeConverterFactory = ServiceContainer.Resolve<ITypeConverterFactory>();
            var typeConverter = typeConverterFactory.Create(setting.TemplateLanguage);

            var modelService = new ProxyModelService(typeConverter);
            modelService.Message += MessageHandler;
            var model = modelService.BuildModel(entityMetadata, sdkMessageMetadata);
            modelService.Message -= MessageHandler;

            var generator = new ProxyGeneratorService(setting);
            generator.Message += MessageHandler;       
            generator.BuildProxies(model);
            generator.Message -= MessageHandler;
        }
    }

    private static IEnumerable<ISettings> GetSettingsFromProxyConfigFile(string searchPath)
    {
        var settings = ServiceContainer.Resolve<IProxySettingsService>().LoadSettings(searchPath);

        return settings?.Select(s => s
           .SetTemplateRelativeTo(s.ConfigurationPath)
           .SetOutputRelativeTo(s.ConfigurationPath));
    }

    private static IEnumerable<ISettings> GetSettingsFromSpklConfigFile(string searchPath)
    {        
        var settings = ServiceContainer.Resolve<ISpklSettingsService>().LoadSettings(searchPath);
               
        return settings?.Select(s => s
            .SetTemplateRelativeTo(s.ConfigurationPath)
            .SetOutputRelativeTo(s.ConfigurationPath));       
    }

    private static void MessageHandler(object sender, MessageEventArgs e)
    {
        Console.WriteLine(e.Message);
    }


}

