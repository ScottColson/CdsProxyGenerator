### Cds T4 Proxy Generator

This project provides tools for generating proxy classes for CDS entities and Sdk Messages using customizable T4 text templates. Project
artifacts include a CdsProxyGenerator library assembly that encapsulates the core functionality of the tool set, a command line 
executable that can be incorporated into DevOps processes, and a batch file that simplifies execution from within a development environment.

###### Credits

The code and concepts in the project are derived from the following open source projects:

- [Gayan Perera's](https://twitter.com/NZxRMGuy) [Dynamics CRM 2011 T4 Code Generator](https://archive.codeplex.com/?p=crm2011codegen)
- [Scott Durow's](https://twitter.com/ScottDurow) [spkl Dynamics 365 Deployment Task Runner](https://github.com/scottdurow/SparkleXrm/tree/master/spkl)
- [Daryl LaBar's](https://twitter.com/ddlabar) [Early Bound Generator](https://github.com/daryllabar/DLaB.Xrm.XrmToolBoxTools/tree/master/DLaB.EarlyBoundGenerator)
- [Damien Guard's](https://twitter.com/damienguard) [T4 Multiple Output Helper](https://github.com/damieng/DamienGKit/tree/master/T4/MultipleOutputHelper)

Early on in my development efforts, I ran into .NET Framework compatibility issues with Microsoft's current T4 engine which did not support .NET Framework 4.6 
required by Microsoft's Xrm Sdk implementation so I opted to rely on [Mono Project's](https://github.com/mono) [T4 Engine Implementation](https://github.com/mono/t4)
which supports both .NET Framework 4.6 and .NET Core.


###### Try it out

To try out the tool install the [CCLLC.CDS.ProxyBuilder](https://www.nuget.org/packages/CCLLC.CDS.ProxyBuilder/) Nuget package into your project. This will make the 
following changes:

- Download the executable and dependencies to your packages folder.
- Add a codgen folder and proxybuilder.bat file to your project source code.
- Add a proxies.json file to the root of your project. You can modify can modify this based on your needs.
- Add the default ProxyTemplate.t4 template file to the root of your project. You can modify this based on your needs.

After installing the package, execute the proxybuilder.bat file to kick off the process. If all goes well the tool will create a folder called Proxies with
sub-folders for entities, global enums, and messages specified in the proxies.json file.

If you are familiar with Scott Durow's spkl tool, this will feel very familiar. If you are working with a project where you have been using the 
spkl earlybound task you can delete the proxies.json file to allow the proxybuilder to use the existing earlybound definitions in your spkl.json file.






