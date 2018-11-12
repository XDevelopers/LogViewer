using System;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Sitatex.Logging.Views
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            Loaded += AboutLoaded;
        }

        private void AboutLoaded(object sender, RoutedEventArgs e)
        {
            StringBuilder sbText = new StringBuilder();
            Assembly assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                object[] attributes = assembly.GetCustomAttributes(false);
                foreach (object attribute in attributes)
                {
                    Type type = attribute.GetType();
                    if (type == typeof(AssemblyTitleAttribute))
                    {
                        AssemblyTitleAttribute title = (AssemblyTitleAttribute)attribute;
                        labelAssemblyName.Content = title.Title;
                    }
                    if (type == typeof(AssemblyFileVersionAttribute))
                    {
                        AssemblyFileVersionAttribute version = (AssemblyFileVersionAttribute)attribute;
                        labelAssemblyVersion.Content = version.Version;
                    }
                    if (type == typeof(AssemblyCopyrightAttribute))
                    {
                        AssemblyCopyrightAttribute copyright = (AssemblyCopyrightAttribute)attribute;
                        sbText.AppendFormat("{0}\r", copyright.Copyright);
                    }
                    if (type == typeof(AssemblyCompanyAttribute))
                    {
                        AssemblyCompanyAttribute company = (AssemblyCompanyAttribute)attribute;
                        sbText.AppendFormat("{0}\r", company.Company);
                    }
                    if (type == typeof(AssemblyDescriptionAttribute))
                    {
                        AssemblyDescriptionAttribute description = (AssemblyDescriptionAttribute)attribute;
                        sbText.AppendFormat("{0}\r", description.Description);
                    }
                }
                labelAssembly.Content = sbText.ToString();
            }
            string text =
@"<appender name=""file"" type=""log4net.Appender.RollingFileAppender"">
      <file type=""log4net.Util.PatternString"">
        <conversionPattern value=""LogFiles\SitatexIP-%date{yyyy-MM}.log"" />
      </file>
      <appendToFile value=""true"" />
      <rollingStyle value=""Size"" />
      <maxSizeRollBackups value=""5"" />
      <maximumFileSize value=""2MB"" />
      <staticLogFileName value=""true"" />
    <layout type=""log4net.Layout.XmlLayoutSchemaLog4j"">
        <locationInfo value=""true"" />
      </layout>
    </appender>
    <root>
      <level value=""ALL"" />
      <appender-ref ref=""file"" />
    </root>
</log4net>";

            this.RichTextBox1.AppendText(text);
        }
    


        private void OkClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
