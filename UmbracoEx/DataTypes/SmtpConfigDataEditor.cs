using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using umbraco.cms.businesslogic.datatype;
using EBA.Ex;

namespace UmbracoEx.DataTypes
{
    /// <summary>
    /// <system.net>
    ///   <mailSettings>
    ///     <smtp from="test@foo.com">
    ///       <network host="smtpserver1" port="25" userName="username" password="secret" defaultCredentials="true" />
    ///     </smtp>
    ///   </mailSettings>
    /// </system.net>
    /// </summary>
    public class SmtpConfigDataEditor : AbstractDataEditor
    {
        public string From { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool DefaultCredentials { get; set; }
        public bool EnableSSL { get; set; }

        public override string DataTypeName
        {
            get { return "UmbracoEx: SmtpConfig"; }
        }

        Guid _Id = new Guid("DF906429-0145-422F-AB63-8975ABD6C825");

        public override Guid Id
        {
            get { return this._Id; }
        }


        /// <summary>
        /// 
        /// </summary>
        private PlaceHolder Control = new PlaceHolder() { ID = "UmbracoEx_SmtpConfig" };


        private TextBox txtFrom;
        private TextBox txtHost;
        private TextBox txtPort;
        private TextBox txtUserName;
        private TextBox txtPassword;
        private CheckBox ckbDefaultCredentials;
        private CheckBox ckbEnableSsl;

        private CustomValidator Validator;

        public SmtpConfigDataEditor()
        {
            this.RenderControl = this.Control;

            this.Control.Init += new EventHandler(this.Control_Init);

            this.DataEditorControl.OnSave += new AbstractDataEditorControl.SaveEventHandler(this.DataEditorControl_OnSave);
        }

  
        private void Control_Init(object sender, EventArgs e)
        {

            Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
            MailSettingsSectionGroup mailSettings = (MailSettingsSectionGroup)configurationFile.GetSectionGroup("system.net/mailSettings");
        

            this.txtFrom = new TextBox() { ID = this.Control.ID + "_txtForm" };
            this.txtHost = new TextBox() { ID = this.Control.ID + "_txtHost" };
            this.txtPassword = new TextBox() { ID = this.Control.ID + "_txtPassword" };
            this.txtUserName = new TextBox() { ID = this.Control.ID + "_txtUserName" };
            this.txtPort = new TextBox() { ID = this.Control.ID + "_txtPort" };

            this.Validator = new CustomValidator() { ControlToValidate = this.txtPort.ID, Display = ValidatorDisplay.None };
            this.Validator.ServerValidate += new ServerValidateEventHandler(Validator_ServerValidate);

            this.ckbDefaultCredentials = new CheckBox() { ID = this.Control.ID + "_ckbDefaultCredentials" };
            this.ckbEnableSsl = new CheckBox { ID = this.Control.ID + "_enableSsl" };


            if (mailSettings != null)
            {
                this.txtFrom.Text = mailSettings.Smtp.From;
                this.txtHost.Text = mailSettings.Smtp.Network.Host;
                this.txtPort.Text = mailSettings.Smtp.Network.Port.ToString();
                this.txtUserName.Text = mailSettings.Smtp.Network.UserName;
                this.txtPassword.Text = mailSettings.Smtp.Network.Password;
                this.ckbDefaultCredentials.Checked = mailSettings.Smtp.Network.DefaultCredentials;
                this.ckbEnableSsl.Checked = mailSettings.Smtp.Network.EnableSsl;
            }

            this.Control.Controls.Add(new LiteralControl("<label style=\"display:block\">From:</label>"));
            this.Control.Controls.Add(this.txtFrom);
            this.Control.Controls.Add(new LiteralControl("</br><label style=\"display:block\">Host:</label>"));
            this.Control.Controls.Add(this.txtHost);
            this.Control.Controls.Add(new LiteralControl("</br><label style=\"display:block\">Port:</label>"));
            this.Control.Controls.Add(this.txtPort);
            this.Control.Controls.Add(new LiteralControl("</br><label style=\"display:block\">UserName:</label>"));
            this.Control.Controls.Add(this.txtUserName);
            this.Control.Controls.Add(new LiteralControl("</br><label style=\"display:block\">Password:</label>"));
            this.Control.Controls.Add(this.txtPassword);
            this.Control.Controls.Add(new LiteralControl("</br><label>DefaultCredentials:</label>"));
            this.Control.Controls.Add(this.ckbDefaultCredentials);
            this.Control.Controls.Add(new LiteralControl("</br><label>EnableSSL:</label>"));
            this.Control.Controls.Add(this.ckbEnableSsl);

        }

        /// <summary>
        /// Handles the ServerValidate event of the Validator control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        private void Validator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            int port;

            if (this.txtPort.Text.HasValue() && int.TryParse(this.txtPort.Text, out port) == false)
            {
                args.IsValid = false;
                this.Validator.ErrorMessage = "The port is invalid.";
            }
            else
            {
                args.IsValid = true;
            }

        }


        private void DataEditorControl_OnSave(EventArgs e)
        {
            string fullWebConfigFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            XDocument xDocument = XDocument.Load(fullWebConfigFileName, LoadOptions.PreserveWhitespace);
            XElement systemNet = xDocument.Root.Descendants("system.net").FirstOrDefault();

            if (systemNet == null)
            {
                systemNet = new XElement("system.net");
                xDocument.Root.Add(systemNet);
            }

            var mailSettings = systemNet.Descendants("mailSettings").FirstOrDefault();

            if (mailSettings == null)
            {
                mailSettings = new XElement("mailSettings");
                systemNet.Add(mailSettings);
            }

            var smtp = mailSettings.Descendants("smtp").FirstOrDefault();

            if (smtp == null)
            {
                smtp = new XElement("smtp");
                mailSettings.Add(smtp);
            }

            smtp.SetAttributeValue("from", this.txtFrom.Text);

            var network = smtp.Descendants("network").FirstOrDefault();
            if (network == null)
            {
                network = new XElement("network");
                smtp.Add(network);
            }

            network.SetAttributeValue("host", this.txtHost.Text);
            network.SetAttributeValue("port", this.txtPort.Text);
            network.SetAttributeValue("userName", this.txtUserName.Text);
            network.SetAttributeValue("password", this.txtPassword.Text);
            network.SetAttributeValue("defaultCredentials", this.ckbDefaultCredentials.Checked);
            network.SetAttributeValue("enableSsl", this.ckbEnableSsl.Checked);
            


            xDocument.Save(fullWebConfigFileName, SaveOptions.None);

            ConfigurationManager.RefreshSection("system.net/mailSettings");
        }
    }


}
