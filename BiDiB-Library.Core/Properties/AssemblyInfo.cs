using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("BiDiBLib")]
[assembly: AssemblyDescription("Library for handling of BiDiB-Messages via Serialport and Network")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("BiDiBLib.dll")]
[assembly: AssemblyCopyright("Copyright © 2013-2021 by Andreas Tillner & Robert Kölz")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("d1227ba5-df76-404a-a7fc-415a20e931a1")]

[assembly: AssemblyVersion("10.6.0.1333")]
[assembly: AssemblyFileVersion("10.6.0.1333")]

[assembly: InternalsVisibleTo("org.bidib.netbidibc.core.Test")]
[assembly: InternalsVisibleTo("org.bidib.Monitor.DecoderManagement.Test")]
[assembly: InternalsVisibleTo("org.bidib.Monitor.Infrastructure.Test")]
[assembly: InternalsVisibleTo("org.bidib.Monitor.Test")]
[assembly: InternalsVisibleTo("org.bidib.nbidibc.netbidib.test")]
[assembly: InternalsVisibleTo("org.bidib.Monitor.Applications")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // Rhino Mocks
