//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3031
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;
using WCell.Util;

// 
// This source code was auto-generated by xsd, Version=2.0.50727.1432.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[XmlType(AnonymousType = true)]
[XmlRoot("sniffitztlog", Namespace = "", IsNullable = false)]
public class Sniffitztlog : XmlConfig<Sniffitztlog>
{
	/// <remarks/>
	[XmlElement("header")]
	public SniffitztlogHeader Header
	{ get;
		set;
	}

	/// <remarks/>
	[XmlElement("packet")]
	public SniffitztlogPacket[] Packets
	{
		get; 
		set;
	}
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class SniffitztlogHeader
{

	private string accountNameField;

	private ushort clientBuildField;

	private string clientLangField;

	private string realmNameField;

	private string realmServerField;

	private string snifferVersionField;

	/// <remarks/>
	[System.Xml.Serialization.XmlAttributeAttribute()]
	public string accountName
	{
		get
		{
			return this.accountNameField;
		}
		set
		{
			this.accountNameField = value;
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlAttributeAttribute()]
	public ushort clientBuild
	{
		get
		{
			return this.clientBuildField;
		}
		set
		{
			this.clientBuildField = value;
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlAttributeAttribute()]
	public string clientLang
	{
		get
		{
			return this.clientLangField;
		}
		set
		{
			this.clientLangField = value;
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlAttributeAttribute()]
	public string realmName
	{
		get
		{
			return this.realmNameField;
		}
		set
		{
			this.realmNameField = value;
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlAttributeAttribute()]
	public string realmServer
	{
		get
		{
			return this.realmServerField;
		}
		set
		{
			this.realmServerField = value;
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlAttributeAttribute()]
	public string snifferVersion
	{
		get
		{
			return this.snifferVersionField;
		}
		set
		{
			this.snifferVersionField = value;
		}
	}
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class SniffitztlogPacket
{

	private uint dateField;

	private string directionField;

	private ushort opcodeField;

	private string valueField;

	/// <remarks/>
	[System.Xml.Serialization.XmlAttributeAttribute()]
	public uint date
	{
		get
		{
			return this.dateField;
		}
		set
		{
			this.dateField = value;
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlAttributeAttribute()]
	public string direction
	{
		get
		{
			return this.directionField;
		}
		set
		{
			this.directionField = value;
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlAttributeAttribute()]
	public ushort opcode
	{
		get
		{
			return this.opcodeField;
		}
		set
		{
			this.opcodeField = value;
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTextAttribute()]
	public string Value
	{
		get
		{
			return this.valueField;
		}
		set
		{
			this.valueField = value;
		}
	}
}