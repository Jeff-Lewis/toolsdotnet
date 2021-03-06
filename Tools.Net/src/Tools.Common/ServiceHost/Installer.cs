using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Configuration;
using System.Diagnostics;
using System.Xml;
using System.Reflection;
using Tools.Common.Exceptions;
using System.Xml.XPath;
using Tools.Common.Utils;
using System.IO;
using System.Security.Permissions;


namespace Tools.Common.ServiceHost
{
	
	#region Installer class
	
	[RunInstaller(true)]
	public class Installer : System.Configuration.Install.Installer
	{
		#region Declarations
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private ServiceInstaller				srvInstaller;
		private ServiceProcessInstaller			spInstaller;

        /// <summary>
        /// The full file path to the file with the installers
        /// </summary>
        private string installSource;

        public string InstallSource
        {
            get { return installSource; }
        }
		
        #endregion

		#region Constructors

		public Installer() : base()
		{
			try
			{
				//Debugger.Launch();
				InitializeComponent();
				srvInstaller = new ServiceInstaller();
				spInstaller = new ServiceProcessInstaller();

                EstablishServiceProperties(srvInstaller, spInstaller);

				Installers.Add(srvInstaller);
				Installers.Add(spInstaller);
			}
			catch (Exception ex)
			{
                //if (ExceptionPolicy.HandleException(ex, ExceptionPolicyName.Common))
                //    throw ex;
                Console.Write(ex.ToString());
                throw ex;
			}


		}
        /// <summary>
        /// Establishes the service properties.
        /// </summary>
        /// <param name="srvInstaller">The SRV installer.</param>
        /// <param name="spInstaller">The sp installer.</param>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        private void EstablishServiceProperties(
            ServiceInstaller srvInstaller, ServiceProcessInstaller spInstaller)
        {
            //ServiceHostConfigSection config = 
            //    ConfigurationManager.GetSection(this.GetType().FullName) as ServiceHostConfigSection;  
            installSource = Environment.GetCommandLineArgs()[Environment.GetCommandLineArgs().Length - 1];

            
            
            Configuration config = ConfigurationManager.OpenExeConfiguration(
                installSource);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            //TODO: (SD) Check if there is a null or exception for the non-existing config (as returns from machine anyway!)
            ServiceHostInstallConfigSection configSection = config.GetSection(
                typeof(Tools.Common.ServiceHost.Installer).FullName) as ServiceHostInstallConfigSection;
            // Check for the AllValues already covered by the configuration manager and the required constrains in the config section,
            // but just for any case.
            if (configSection == null ||
                !CompareUtility.AllValuesSetAsString(configSection.Name, configSection.DisplayName, configSection.Description))
            {
                throw new ConfigurationErrorsException(
                    String.Format("Configuration error. Expected section {0} not found or doesn't contain mandatory items:  {1} ",
                    this.GetType().FullName, "name, displayName, description"),
                    config.FilePath,
                    0);
            }

            srvInstaller.ServiceName = configSection.Name;
            srvInstaller.DisplayName = configSection.DisplayName;
            srvInstaller.Description = configSection.Description;


            //TODO: (SD) Provide configuration for those values and for the rest of them as well.
            spInstaller.Account = ServiceAccount.NetworkService;
            srvInstaller.StartType = ServiceStartMode.Manual;
        }
        //(SD) No test coverage for this one

        /// <summary>
        /// Handles the AssemblyResolve event of the AppDomain.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            ConsoleTraceListener consoleListener = new ConsoleTraceListener(false);
            if (!Trace.Listeners.Contains(consoleListener))
            {
                Trace.Listeners.Add(consoleListener);
            }
            // Can't use anything but basics here as it may be exactly the source of the 
            // failure. 
            // Changing it from the trace to console as well, to keep it simple.
            Trace.WriteLine(String.Format("Regular attempt to resolve the assembly {0} failed." +
                " Will try to load from {1}", args.Name, ResolveAssemblyPath(args.Name)));

            if (Trace.Listeners.Contains(consoleListener))
            {
                Trace.Listeners.Remove(consoleListener);
            }
            return Assembly.LoadFrom(ResolveAssemblyPath(args.Name));
        }

        private string ResolveAssemblyPath(string assemblyDllName)
        {
            return Path.GetDirectoryName(InstallSource) + @"\" + assemblyDllName;
        }




		#endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}

	#endregion

}
