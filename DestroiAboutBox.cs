// DeSTRoi.NonMVVMWindows.DestroiAboutBox
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Xml;

[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
public class DestroiAboutBox : Window, IComponentConnector
{
	private const string propertyNameTitle = "Title";

	private const string propertyNameDescription = "Description";

	private const string propertyNameProduct = "Product";

	private const string propertyNameCopyright = "Copyright";

	private const string propertyNameCompany = "Company";

	private const string xPathRoot = "ApplicationInfo/";

	private const string xPathTitle = "ApplicationInfo/Title";

	private const string xPathVersion = "ApplicationInfo/Version";

	private const string xPathDescription = "ApplicationInfo/Description";

	private const string xPathProduct = "ApplicationInfo/Product";

	private const string xPathCopyright = "ApplicationInfo/Copyright";

	private const string xPathCompany = "ApplicationInfo/Company";

	private const string xPathLink = "ApplicationInfo/Link";

	private const string xPathLinkUri = "ApplicationInfo/Link/@Uri";

	private XmlDocument xmlDoc;

	internal Label productName;

	internal Label versionLabel;

	internal Label version;

	internal Label copyright;

	internal Label thanks;

	internal Label info;

	internal Image imageLic;

	internal Button okButton;

	private bool _contentLoaded;

	public string ProductTitle
	{
		get
		{
			string text = CalculatePropertyValue<AssemblyTitleAttribute>("Title", "ApplicationInfo/Title");
			if (string.IsNullOrEmpty(text))
			{
				text = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
			return text;
		}
	}

	public string Version
	{
		get
		{
			string empty = string.Empty;
			Version version = Assembly.GetExecutingAssembly().GetName().Version;
			if (version != null)
			{
				return version.ToString();
			}
			return GetLogicalResourceString("ApplicationInfo/Version");
		}
	}

	public string Description => CalculatePropertyValue<AssemblyDescriptionAttribute>("Description", "ApplicationInfo/Description");

	public string Product => CalculatePropertyValue<AssemblyProductAttribute>("Product", "ApplicationInfo/Product");

	public string Copyright => CalculatePropertyValue<AssemblyCopyrightAttribute>("Copyright", "ApplicationInfo/Copyright");

	public string Company => CalculatePropertyValue<AssemblyCompanyAttribute>("Company", "ApplicationInfo/Company");

	public string LinkText => GetLogicalResourceString("ApplicationInfo/Link");

	public string LinkUri => GetLogicalResourceString("ApplicationInfo/Link/@Uri");

	protected virtual XmlDocument ResourceXmlDocument
	{
		get
		{
			if (xmlDoc == null)
			{
				XmlDataProvider xmlDataProvider = TryFindResource("aboutProvider") as XmlDataProvider;
				if (xmlDataProvider != null)
				{
					xmlDoc = xmlDataProvider.Document;
				}
			}
			return xmlDoc;
		}
	}

	protected DestroiAboutBox()
	{
		InitializeComponent();
	}

	public DestroiAboutBox(Window parent)
		: this()
	{
		base.Owner = parent;
	}

	private void hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		if (e.Uri != null && !string.IsNullOrEmpty(e.Uri.OriginalString))
		{
			string absoluteUri = e.Uri.AbsoluteUri;
			Process.Start(new ProcessStartInfo(absoluteUri));
			e.Handled = true;
		}
	}

	private string CalculatePropertyValue<T>(string propertyName, string xpathQuery)
	{
		string text = string.Empty;
		object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), inherit: false);
		if (customAttributes.Length > 0)
		{
			PropertyInfo property = ((T)customAttributes[0]).GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
			if (property != null)
			{
				text = (property.GetValue(customAttributes[0], null) as string);
			}
		}
		if (text == string.Empty)
		{
			text = GetLogicalResourceString(xpathQuery);
		}
		return text;
	}

	protected virtual string GetLogicalResourceString(string xpathQuery)
	{
		string result = string.Empty;
		XmlDocument resourceXmlDocument = ResourceXmlDocument;
		if (resourceXmlDocument != null)
		{
			XmlNode xmlNode = resourceXmlDocument.SelectSingleNode(xpathQuery);
			if (xmlNode != null)
			{
				result = ((!(xmlNode is XmlAttribute)) ? xmlNode.InnerText : xmlNode.Value);
			}
		}
		return result;
	}

	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/DeSTRoi;component/nonmvvmwindows/destroiaboutbox.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			productName = (Label)target;
			break;
		case 2:
			versionLabel = (Label)target;
			break;
		case 3:
			version = (Label)target;
			break;
		case 4:
			copyright = (Label)target;
			break;
		case 5:
			thanks = (Label)target;
			break;
		case 6:
			info = (Label)target;
			break;
		case 7:
			imageLic = (Image)target;
			break;
		case 8:
			okButton = (Button)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
