using System;
using System.IO.Packaging;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Windows.Management.Deployment;

namespace AppxInstaller
{
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty AppNameProperty = DependencyProperty.Register("AppName", typeof(string), typeof(MainWindow), new PropertyMetadata(""));
        public static readonly DependencyProperty PublisherProperty = DependencyProperty.Register("Publisher", typeof(string), typeof(MainWindow), new PropertyMetadata(""));
        public static readonly DependencyProperty CertificateProperty = DependencyProperty.Register("Certificate", typeof(string), typeof(MainWindow), new PropertyMetadata(""));
        public static readonly DependencyProperty InstallEnabledProperty = DependencyProperty.Register("InstallEnabled", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            var package = Package.Open(App.Target);
            var part = package.GetPart(new Uri("/AppxManifest.xml", UriKind.Relative));
            var manifest = XElement.Load(part.GetStream());
            package.Close();

            var properties = manifest.Element(manifest.Name.Namespace + "Properties");
            AppName = properties.Element(manifest.Name.Namespace + "DisplayName").Value;
            Publisher = properties.Element(manifest.Name.Namespace + "PublisherDisplayName").Value;

            Uri packageUri = new Uri(App.Target);
            var cert = new X509Certificate2(X509Certificate.CreateFromSignedFile(packageUri.AbsolutePath));

            Certificate = cert.Issuer;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            InstallEnabled = false;
            Uri packageUri = new Uri(App.Target);
            var cert = new X509Certificate2(X509Certificate.CreateFromSignedFile(packageUri.AbsolutePath));

            if (!cert.Verify())
            {
                var store = new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);
                store.Close();
            }

            PackageManager packageManager = new PackageManager();
            var tcs = new TaskCompletionSource<DeploymentResult>();
            var deploymentOperation = packageManager.AddPackageAsync(packageUri, null, DeploymentOptions.None);
            deploymentOperation.Completed = (dr, dp) => tcs.SetResult(dr.GetResults());
            var res = await tcs.Task;

            var status = string.IsNullOrEmpty(res.ErrorText) ? "Application installed" : res.ErrorText;

            MessageBox.Show(status);
            App.Current.Shutdown();
        }

        public string AppName
        {
            get { return (string)GetValue(AppNameProperty); }
            set { SetValue(AppNameProperty, value); }
        }

        public string Publisher
        {
            get { return (string)GetValue(PublisherProperty); }
            set { SetValue(PublisherProperty, value); }
        }

        public string Certificate
        {
            get { return (string)GetValue(CertificateProperty); }
            set { SetValue(CertificateProperty, value); }
        }

        public bool InstallEnabled
        {
            get { return (bool)GetValue(InstallEnabledProperty); }
            set { SetValue(InstallEnabledProperty, value); }
        }
    }
}
