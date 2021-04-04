namespace CTVPN_ClientService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CTVPN_serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.CTVPN_serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // CTVPN_serviceProcessInstaller
            // 
            this.CTVPN_serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.CTVPN_serviceProcessInstaller.Password = null;
            this.CTVPN_serviceProcessInstaller.Username = null;
            // 
            // CTVPN_serviceInstaller
            // 
            this.CTVPN_serviceInstaller.DisplayName = "CT VPN Service";
            this.CTVPN_serviceInstaller.ServiceName = "CT VPN Service";
            this.CTVPN_serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.CTVPN_serviceProcessInstaller,
            this.CTVPN_serviceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller CTVPN_serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller CTVPN_serviceInstaller;
    }
}