﻿//
// Copyright (c) Ping Castle. All rights reserved.
// https://www.pingcastle.com
//
// Licensed under the Non-Profit OSL. See LICENSE file in the project root for full license information.
//
using PingCastle.Healthcheck;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Globalization;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Xml;

namespace PingCastle.ADWS
{
	[DebuggerDisplay("{DistinguishedName}")]
	public class ADItem
	{
		public class ReplPropertyMetaDataItem
		{
			public int AttrType { get; set; }
			public string AttrName { get; set; }
			public int Version { get; set; }
			public DateTime LastOriginatingChange { get; set; }
			public Guid LastOriginatingDsaInvocationID { get; set; }
			public long UsnOriginatingChange { get; set; }
			public long UsnLocalChange { get; set; }
		}

		[AttributeUsage(AttributeTargets.Property)]
		private class ADAttributeAttribute : Attribute
		{
			public ADAttributeAttribute(string aDAttribute, ADAttributeValueKind valueKind)
			{
				ADAttribute = aDAttribute;
				ValueKind = valueKind;
			}
			public string ADAttribute { get; set; }
			public ADAttributeValueKind ValueKind { get; set; }
		}

		private enum ADAttributeValueKind
		{
			BoolValue,
			IntValue,
			LongValue,
			DateValue,
			DateValue2,
			GUIDValue,
			ByteArrayValue,
			StringValue,
			StringValueLowerInvariant,
			StringArrayValue,
			SecurityDescriptorValue,
			ReplMetadataValue,
			CertificateStore,
			ForestInfoValue,
			SIDValue,
			SIDArrayValue,
			ReplMetadataValue2,
		}

		private class ADAttributeTranslation
		{
			public ADAttributeAttribute aDAttributeAttribute { get; private set; }
			public System.Reflection.PropertyInfo prop { get; private set; }

			public ADAttributeTranslation(ADAttributeAttribute aDAttributeAttribute, System.Reflection.PropertyInfo prop)
			{
				this.aDAttributeAttribute = aDAttributeAttribute;
				this.prop = prop;
			}
			
		}

		static Dictionary<string, ADAttributeTranslation> AttributeTranslation;
		static ADItem()
		{
			AttributeTranslation = new Dictionary<string, ADAttributeTranslation>();
			foreach (var prop in typeof(ADItem).GetProperties())
			{
				var attributes = prop.GetCustomAttributes(typeof(ADAttributeAttribute), false);
				if (attributes != null && attributes.Length != 0)
				{
					AttributeTranslation.Add(((ADAttributeAttribute)attributes[0]).ADAttribute.ToLowerInvariant(), new ADAttributeTranslation((ADAttributeAttribute)attributes[0], prop));
				}
			}
		}

		public string Class { get; set; }
		
		[ADAttributeAttribute("adminCount", ADAttributeValueKind.IntValue)]
		public int AdminCount { get; set; }
		[ADAttributeAttribute("attributeID", ADAttributeValueKind.StringValue)]
		public string AttributeID { get; set; }
		[ADAttributeAttribute("cACertificate", ADAttributeValueKind.CertificateStore)]
		public X509Certificate2Collection CACertificate { get; set; }
		[ADAttributeAttribute("description", ADAttributeValueKind.StringValue)]
		public string Description { get; set; }
		[ADAttributeAttribute("displayName", ADAttributeValueKind.StringValue)]
		public string DisplayName { get; set; }
		[ADAttributeAttribute("distinguishedName", ADAttributeValueKind.StringValue)]
		public string DistinguishedName { get; set; }
		[ADAttributeAttribute("dNSHostName", ADAttributeValueKind.StringValue)]
		public string DNSHostName { get; set; }
		[ADAttributeAttribute("dnsRoot", ADAttributeValueKind.StringValueLowerInvariant)]
		public string DnsRoot { get; set; }
		[ADAttributeAttribute("dSHeuristics", ADAttributeValueKind.StringValue)]
		public string DSHeuristics { get; set; }
		[ADAttributeAttribute("ms-DS-MachineAccountQuota", ADAttributeValueKind.IntValue)]
		public int DSMachineAccountQuota { get; set; }
		[ADAttributeAttribute("flags", ADAttributeValueKind.IntValue)]
		public int Flags { get; set; }
		[ADAttributeAttribute("fSMORoleOwner", ADAttributeValueKind.StringValue)]
		public string fSMORoleOwner { get; set; }
		[ADAttributeAttribute("gPCFileSysPath", ADAttributeValueKind.StringValue)]
		public string GPCFileSysPath { get; set; }
		[ADAttributeAttribute("gPLink", ADAttributeValueKind.StringValue)]
		public string GPLink { get; set; }
		[ADAttributeAttribute("lastLogonTimestamp", ADAttributeValueKind.DateValue)]
		public DateTime LastLogonTimestamp { get; set; }
		[ADAttributeAttribute("lDAPDisplayName", ADAttributeValueKind.StringValue)]
		public string lDAPDisplayName { get; set; }
		[ADAttributeAttribute("location", ADAttributeValueKind.StringValue)]
		public string Location { get; set; }
		[ADAttributeAttribute("member", ADAttributeValueKind.StringArrayValue)]
		public string[] Member { get; set; }
		[ADAttributeAttribute("memberOf", ADAttributeValueKind.StringArrayValue)]
		public string[] MemberOf { get; set; }
		[ADAttributeAttribute("msDS-AllowedToActOnBehalfOfOtherIdentity", ADAttributeValueKind.SecurityDescriptorValue)]
		public ActiveDirectorySecurity msDSAllowedToActOnBehalfOfOtherIdentity { get; set; }
		[ADAttributeAttribute("msDS-AllowedToDelegateTo", ADAttributeValueKind.StringArrayValue)]
		public string[] msDSAllowedToDelegateTo { get; set; }
		[ADAttributeAttribute("msDS-EnabledFeature", ADAttributeValueKind.StringArrayValue)]
		public string[] msDSEnabledFeature { get; set; }
		[ADAttributeAttribute("msDS-SupportedEncryptionTypes", ADAttributeValueKind.IntValue)]
		public int msDSSupportedEncryptionTypes { get; set; }
		[ADAttributeAttribute("msDS-MinimumPasswordAge", ADAttributeValueKind.LongValue)]
		public long msDSMinimumPasswordAge { get; set; }
		[ADAttributeAttribute("msDS-MaximumPasswordAge", ADAttributeValueKind.LongValue)]
		public long msDSMaximumPasswordAge { get; set; }
		[ADAttributeAttribute("msDS-MinimumPasswordLength", ADAttributeValueKind.IntValue)]
		public int msDSMinimumPasswordLength { get; set; }
		[ADAttributeAttribute("msDS-PasswordComplexityEnabled", ADAttributeValueKind.BoolValue)]
		public bool msDSPasswordComplexityEnabled { get; set; }
		[ADAttributeAttribute("msDS-PasswordHistoryLength", ADAttributeValueKind.IntValue)]
		public int msDSPasswordHistoryLength { get; set; }
		[ADAttributeAttribute("msDS-LockoutThreshold", ADAttributeValueKind.IntValue)]
		public int msDSLockoutThreshold { get; set; }
		[ADAttributeAttribute("msDS-LockoutObservationWindow", ADAttributeValueKind.LongValue)]
		public long msDSLockoutObservationWindow { get; set; }
		[ADAttributeAttribute("msDS-LockoutDuration", ADAttributeValueKind.LongValue)]
		public long msDSLockoutDuration { get; set; }
		[ADAttributeAttribute("msDS-PasswordReversibleEncryptionEnabled", ADAttributeValueKind.BoolValue)]
		public bool msDSPasswordReversibleEncryptionEnabled { get; set; }
		[ADAttributeAttribute("msDS-ReplAttributeMetaData", ADAttributeValueKind.ReplMetadataValue2)]
		public Dictionary<string, ReplPropertyMetaDataItem> msDSReplAttributeMetaData { get; set; }
		[ADAttributeAttribute("msDS-TrustForestTrustInfo", ADAttributeValueKind.ForestInfoValue)]
		public List<HealthCheckTrustDomainInfoData> msDSTrustForestTrustInfo { get; set; }
		[ADAttributeAttribute("msiFileList", ADAttributeValueKind.StringArrayValue)]
		public string[] msiFileList { get; set; }
		[ADAttributeAttribute("name", ADAttributeValueKind.StringValue)]
		public string Name { get; set; }
		[ADAttributeAttribute("nETBIOSName", ADAttributeValueKind.StringValue)]
		public string NetBIOSName { get; set; }
		[ADAttributeAttribute("nTSecurityDescriptor", ADAttributeValueKind.SecurityDescriptorValue)]
		public ActiveDirectorySecurity NTSecurityDescriptor { get; set; }
		[ADAttributeAttribute("objectSid", ADAttributeValueKind.SIDValue)]
		public SecurityIdentifier ObjectSid { get; set; }
		[ADAttributeAttribute("objectVersion", ADAttributeValueKind.IntValue)]
		public int ObjectVersion { get; set; }
		[ADAttributeAttribute("operatingSystem", ADAttributeValueKind.StringValue)]
		public string OperatingSystem { get; set; }
		[ADAttributeAttribute("primaryGroupID", ADAttributeValueKind.IntValue)]
		public int PrimaryGroupID { get; set; }
		[ADAttributeAttribute("pwdLastSet", ADAttributeValueKind.DateValue)]
		public DateTime PwdLastSet { get; set; }
		[ADAttributeAttribute("sAMAccountName", ADAttributeValueKind.StringValue)]
		public string SAMAccountName { get; set; }
		[ADAttributeAttribute("schemaIDGUID", ADAttributeValueKind.GUIDValue)]
		public Guid SchemaIDGUID { get; set; }
		[ADAttributeAttribute("schemaInfo", ADAttributeValueKind.ByteArrayValue)]
		public byte[] SchemaInfo { get; set; }
		[ADAttributeAttribute("scriptPath", ADAttributeValueKind.StringValue)]
		public string ScriptPath { get; set; }
		[ADAttributeAttribute("securityIdentifier", ADAttributeValueKind.SIDValue)]
		public SecurityIdentifier SecurityIdentifier { get; set; }
		[ADAttributeAttribute("servicePrincipalName", ADAttributeValueKind.StringArrayValue)]
		public string[] ServicePrincipalName { get; set; }
		[ADAttributeAttribute("sIDHistory", ADAttributeValueKind.SIDArrayValue)]
		public SecurityIdentifier[] SIDHistory { get; set; }
		[ADAttributeAttribute("siteObjectBL", ADAttributeValueKind.StringArrayValue)]
		public string[] SiteObjectBL { get; set; }
		[ADAttributeAttribute("trustAttributes", ADAttributeValueKind.IntValue)]
		public int TrustAttributes { get; set; }
		[ADAttributeAttribute("trustDirection", ADAttributeValueKind.IntValue)]
		public int TrustDirection { get; set; }
		[ADAttributeAttribute("trustPartner", ADAttributeValueKind.StringValueLowerInvariant)]
		public string TrustPartner { get; set; }
		[ADAttributeAttribute("trustType", ADAttributeValueKind.IntValue)]
		public int TrustType { get; set; }
		[ADAttributeAttribute("userAccountControl", ADAttributeValueKind.IntValue)]
		public int UserAccountControl { get; set; }
		[ADAttributeAttribute("whenCreated", ADAttributeValueKind.DateValue2)]
		public DateTime WhenCreated { get; set; }
		[ADAttributeAttribute("whenChanged", ADAttributeValueKind.DateValue2)]
		public DateTime WhenChanged { get; set; }
		[ADAttributeAttribute("replPropertyMetaData", ADAttributeValueKind.ReplMetadataValue)]
		public Dictionary<int, ReplPropertyMetaDataItem> ReplPropertyMetaData { get; set; }

		public List<string> GetApplicableGPO()
		{
			var output = new List<string>();
			if (string.IsNullOrEmpty(GPLink))
				return output;
			string[] gplinks = GPLink.Split(']');
			foreach (string gplink in gplinks)
			{
				if (string.IsNullOrEmpty(gplink.TrimEnd()))
					continue;
				string[] gpodata = gplink.Split(';');
				if (gpodata.Length != 2)
				{
					Trace.WriteLine("invalid gpolink1:" + gplink);
					continue;
				}
				int flag = int.Parse(gpodata[1]);
				if (flag == 1)
					continue;
				if (!gpodata[0].StartsWith("[LDAP://", StringComparison.InvariantCultureIgnoreCase))
				{
					Trace.WriteLine("invalid gpolink2:" + gplink);
					continue;
				}
				string dn = gpodata[0].Substring(8);
				output.Add(dn);
			}
			return output;
		}

		private static string StripNamespace(string input)
		{
			int index = input.IndexOf(':');
			if (index >= 0)
			{
				return input.Substring(index + 1);
			}
			return input;
		}

		private static string ExtractStringValue(XmlNode item)
		{
			XmlNode child = item.FirstChild;
			if (child != null && item.FirstChild != null)
			{
				return child.InnerText;
			}
			return String.Empty;
		}

		private static int ExtractIntValue(XmlNode item)
		{
			XmlNode child = item.FirstChild;
			if (child != null && item.FirstChild != null)
			{
				return int.Parse(child.InnerText);
			}
			return 0;
		}

		private static long ExtractLongValue(XmlNode item)
		{
			XmlNode child = item.FirstChild;
			if (child != null && item.FirstChild != null)
			{
				return long.Parse(child.InnerText);
			}
			return 0;
		}

		private static bool ExtractBoolValue(XmlNode item)
		{
			XmlNode child = item.FirstChild;
			if (child != null && item.FirstChild != null)
			{
				return bool.Parse(child.InnerText);
			}
			return false;
		}

		private static DateTime ExtractDateValue(XmlNode item)
		{
			XmlNode child = item.FirstChild;
			if (child != null && item.FirstChild != null)
			{
				return SafeExtractDateTimeFromLong(long.Parse(child.InnerText));
			}
			return DateTime.MinValue;
		}

		private static ActiveDirectorySecurity ExtractSDValue(XmlNode child)
		{
			string value = ExtractStringValue(child);
			byte[] data = Convert.FromBase64String(value);
			ActiveDirectorySecurity sd = new ActiveDirectorySecurity();
			sd.SetSecurityDescriptorBinaryForm(data);
			return sd;
		}

		private static SecurityIdentifier ExtractSIDValue(XmlNode child)
		{
			string value = ExtractStringValue(child);
			byte[] data = Convert.FromBase64String(value);
			return new SecurityIdentifier(data, 0);
		}

		// see https://msdn.microsoft.com/en-us/library/cc223786.aspx
		private static List<HealthCheckTrustDomainInfoData> ConvertByteToTrustInfo(byte[] data)
		{
			List<HealthCheckTrustDomainInfoData> output = new List<HealthCheckTrustDomainInfoData>();
			Trace.WriteLine("Beginning to analyze a forestinfo data " + Convert.ToBase64String(data));
			int version = BitConverter.ToInt32(data, 0);
			if (version != 1)
			{
				Trace.WriteLine("trust info version incompatible : " + version);
				return output;
			}
			int recordcount = BitConverter.ToInt32(data, 4);
			Trace.WriteLine("Number of records to analyze: " + recordcount);
			int pointer = 8;
			for (int i = 0; i < recordcount; i++)
			{
				int recordSize = 17;
				int recordLen = BitConverter.ToInt32(data, pointer);
				byte recordType = data[pointer + 16];
				DateTime dt = SafeExtractDateTimeFromLong((((long)BitConverter.ToInt32(data, pointer + 8)) << 32) + BitConverter.ToInt32(data, pointer + 12));
				if (recordType == 0 || recordType == 1)
				{
					int nameLen = BitConverter.ToInt32(data, pointer + recordSize);
					string name = UnicodeEncoding.UTF8.GetString(data, pointer + recordSize + 4, nameLen);
					Trace.WriteLine("RecordType 0 or 1: name=" + name);
				}
				else if (recordType == 2)
				{
					Trace.WriteLine("RecordType 2");
					int tempPointer = pointer + recordSize;
					int sidLen = BitConverter.ToInt32(data, tempPointer);
					tempPointer += 4;
					SecurityIdentifier sid = new SecurityIdentifier(data, tempPointer);
					tempPointer += sidLen;
					int DnsNameLen = BitConverter.ToInt32(data, tempPointer);
					tempPointer += 4;
					string DnsName = UnicodeEncoding.UTF8.GetString(data, tempPointer, DnsNameLen);
					tempPointer += DnsNameLen;
					int NetbiosNameLen = BitConverter.ToInt32(data, tempPointer);
					tempPointer += 4;
					string NetbiosName = UnicodeEncoding.UTF8.GetString(data, tempPointer, NetbiosNameLen);
					tempPointer += NetbiosNameLen;

					HealthCheckTrustDomainInfoData domaininfoc = new HealthCheckTrustDomainInfoData();
					domaininfoc.CreationDate = dt;
					domaininfoc.DnsName = DnsName.ToLowerInvariant();
					domaininfoc.NetbiosName = NetbiosName;
					domaininfoc.Sid = sid.Value;
					output.Add(domaininfoc);
				}
				pointer += 4 + recordLen;
			}
			return output;
		}

		private static Dictionary<int, ReplPropertyMetaDataItem> ConvertByteToMetaDataInfo(byte[] data)
		{
			var output = new Dictionary<int, ReplPropertyMetaDataItem>();
			//Trace.WriteLine("Beginning to analyze a replpropertymetadata data " + Convert.ToBase64String(data));
			int version = BitConverter.ToInt32(data, 0);
			if (version != 1)
			{
				Trace.WriteLine("trust info version incompatible : " + version);
				return output;
			}
			int recordcount = BitConverter.ToInt32(data, 8);
			//Trace.WriteLine("Number of records to analyze: " + recordcount);
			int pointer = 16;
			for (int i = 0; i < recordcount; i++)
			{
				var item = new ReplPropertyMetaDataItem();
				item.AttrType = BitConverter.ToInt32(data, pointer);
				item.Version = BitConverter.ToInt32(data, pointer + 4);
				long filetime = BitConverter.ToInt64(data, pointer + 8) * 10000000;
				item.LastOriginatingChange = DateTime.FromFileTime(filetime);
				byte[] guid = new byte[16];
				Array.Copy(data, pointer + 16, guid, 0, 16);
				item.LastOriginatingDsaInvocationID = new Guid(guid);
				item.UsnOriginatingChange = BitConverter.ToInt64(data, pointer + 32);
				item.UsnLocalChange = BitConverter.ToInt64(data, pointer + 40);
				pointer += 48;
				output[item.AttrType] = item;
			}
			return output;
		}

		private static Dictionary<string, ReplPropertyMetaDataItem> ConvertStringArrayToMetaDataInfo(IEnumerable<string> data)
		{
			var output = new Dictionary<string, ReplPropertyMetaDataItem>();
			foreach (var xml in data)
			{
				var metaData = new ReplPropertyMetaDataItem();
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xml.Replace('\0',' '));
				foreach (XmlNode child in doc.DocumentElement.ChildNodes)
				{
					switch (child.Name)
					{
						case "pszAttributeName":
							metaData.AttrName = child.InnerText;
							break;
						case "dwVersion":
							metaData.Version = int.Parse(child.InnerText);
							break;
						case "ftimeLastOriginatingChange":
							metaData.LastOriginatingChange = DateTime.Parse(child.InnerText);
							break;
						case "uuidLastOriginatingDsaInvocationID":
							metaData.LastOriginatingDsaInvocationID = new Guid(child.InnerText);
							break;
						case "usnOriginatingChange":
							metaData.UsnOriginatingChange = long.Parse(child.InnerText);
							break;
						case "usnLocalChange":
							metaData.UsnLocalChange = long.Parse(child.InnerText);
							break;
						case "pszLastOriginatingDsaDN":
							//metaData.LastOriginatingDsaInvocationID = child.InnerText;
							break;
					}
				}
				if (!String.IsNullOrEmpty(metaData.AttrName))
				{
					output[metaData.AttrName] = metaData;
				}
			}
			return output;
		}

		private static List<HealthCheckTrustDomainInfoData> ExtractTrustForestInfo(XmlNode child)
		{
			string value = ExtractStringValue(child);
			return ConvertByteToTrustInfo(Convert.FromBase64String(value));
		}

		private static Dictionary<int, ReplPropertyMetaDataItem> ExtractReplPropertyMetadata(XmlNode child)
		{
			string value = ExtractStringValue(child);
			return ConvertByteToMetaDataInfo(Convert.FromBase64String(value));
		}

		private static string[] ExtractStringArrayValue(XmlNode item)
		{
			XmlNode child = item.FirstChild;
			List<string> list = new List<string>();
			while (child != null)
			{
				list.Add(child.InnerText);
				child = child.NextSibling;
			}
			return list.ToArray();
		}


		private static X509Certificate2Collection ExtractCertificateStore(XmlNode item)
		{
			XmlNode child = item.FirstChild;
			X509Certificate2Collection store = new X509Certificate2Collection();
			while (child != null)
			{
				store.Add(new X509Certificate2(Convert.FromBase64String(child.InnerText)));
				child = child.NextSibling;
			}
			return store;
		}

		private static SecurityIdentifier[] ExtractSIDArrayValue(XmlNode item)
		{
			XmlNode child = item.FirstChild;
			List<SecurityIdentifier> list = new List<SecurityIdentifier>();
			while (child != null)
			{
				byte[] data = Convert.FromBase64String(child.InnerText);
				list.Add(new SecurityIdentifier(data, 0));
				child = child.NextSibling;
			}
			return list.ToArray();
		}

		public static ADItem Create(XmlElement item)
		{
			ADItem aditem = new ADItem();
			aditem.Class = StripNamespace(item.Name).ToLowerInvariant();
			XmlNode child = item.FirstChild;

			while (child != null && child is XmlElement)
			{
				string name = StripNamespace(child.Name).ToLowerInvariant();
				if (AttributeTranslation.ContainsKey(name))
				{
					try
					{
						var translation = AttributeTranslation[name];
						switch (translation.aDAttributeAttribute.ValueKind)
						{
							case ADAttributeValueKind.BoolValue:
								translation.prop.SetValue(aditem, ExtractBoolValue(child), null);
								break;
							case ADAttributeValueKind.IntValue:
								translation.prop.SetValue(aditem, ExtractIntValue(child), null);
								break;
							case ADAttributeValueKind.LongValue:
								translation.prop.SetValue(aditem, ExtractLongValue(child), null);
								break;
							case ADAttributeValueKind.DateValue:
								translation.prop.SetValue(aditem, ExtractDateValue(child), null);
								break;
							case ADAttributeValueKind.DateValue2:
								translation.prop.SetValue(aditem, DateTime.ParseExact(ExtractStringValue(child), "yyyyMMddHHmmss.f'Z'", CultureInfo.InvariantCulture), null);
								break;
							case ADAttributeValueKind.GUIDValue:
								translation.prop.SetValue(aditem, new Guid(Convert.FromBase64String(ExtractStringValue(child))), null);
								break;
							case ADAttributeValueKind.ByteArrayValue:
								translation.prop.SetValue(aditem, Convert.FromBase64String(ExtractStringValue(child)), null);
								break;
							case ADAttributeValueKind.StringValue:
								translation.prop.SetValue(aditem, ExtractStringValue(child), null);
								break;
							case ADAttributeValueKind.StringValueLowerInvariant:
								translation.prop.SetValue(aditem, ExtractStringValue(child).ToLowerInvariant(), null);
								break;
							case ADAttributeValueKind.StringArrayValue:
								translation.prop.SetValue(aditem, ExtractStringArrayValue(child), null);
								break;
							case ADAttributeValueKind.SecurityDescriptorValue:
								translation.prop.SetValue(aditem, ExtractSDValue(child), null);
								break;
							case ADAttributeValueKind.ReplMetadataValue:
								translation.prop.SetValue(aditem, ExtractReplPropertyMetadata(child), null);
								break;
							case ADAttributeValueKind.ReplMetadataValue2:
								translation.prop.SetValue(aditem, ConvertStringArrayToMetaDataInfo(ExtractStringArrayValue(child)), null);
								break;
							case ADAttributeValueKind.CertificateStore:
								translation.prop.SetValue(aditem, ExtractCertificateStore(child), null);
								break;
							case ADAttributeValueKind.ForestInfoValue:
								translation.prop.SetValue(aditem, ExtractTrustForestInfo(child), null);
								break;
							case ADAttributeValueKind.SIDValue:
								translation.prop.SetValue(aditem, ExtractSIDValue(child), null);
								break;
							case ADAttributeValueKind.SIDArrayValue:
								translation.prop.SetValue(aditem, ExtractSIDArrayValue(child), null);
								break;
						}
					}
					catch (Exception)
					{
						Trace.WriteLine("Error when translation attribute " + name);
						throw;
					}
				}
				else
				{
					switch (name)
					{
						case "objectreferenceproperty":
							break;
						case "objectclass":
							break;
						default:
							Trace.WriteLine("Unknown attribute: " + name);
							break;
					}
				}
				child = child.NextSibling;
			}
			return aditem;
		}

		// the AD is supposed to store Filetime as long.
		// Samba can return an out of range value
		private static DateTime SafeExtractDateTimeFromLong(long value)
		{
			try
			{
				return DateTime.FromFileTime(value);
			}
			catch
			{
				return DateTime.MinValue;
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static ADItem Create(SearchResult sr, bool nTSecurityDescriptor)
		{
			ADItem aditem = new ADItem();
			// note: nTSecurityDescriptor is not present in the property except when run under admin (because allowed to read it)
			// this workaround is here when running under lower permission
			var directoryEntry = sr.GetDirectoryEntry();
			if (nTSecurityDescriptor)
			{
				aditem.NTSecurityDescriptor = directoryEntry.ObjectSecurity;
			}
			aditem.Class = directoryEntry.SchemaClassName;

			foreach (string name in sr.Properties.PropertyNames)
			{
				if (name == "ntsecuritydescriptor")
					continue;
				if (AttributeTranslation.ContainsKey(name))
				{
					try
					{
						var translation = AttributeTranslation[name];
						switch (translation.aDAttributeAttribute.ValueKind)
						{
							case ADAttributeValueKind.BoolValue:
								translation.prop.SetValue(aditem, (bool)sr.Properties[name][0], null);
								break;
							case ADAttributeValueKind.IntValue:
								translation.prop.SetValue(aditem, (int)sr.Properties[name][0], null);
								break;
							case ADAttributeValueKind.LongValue:
								translation.prop.SetValue(aditem, (long)sr.Properties[name][0], null);
								break;
							case ADAttributeValueKind.DateValue:
								translation.prop.SetValue(aditem, SafeExtractDateTimeFromLong((long)sr.Properties[name][0]), null);
								break;
							case ADAttributeValueKind.DateValue2:
								translation.prop.SetValue(aditem, (DateTime)sr.Properties[name][0], null);
								break;
							case ADAttributeValueKind.GUIDValue:
								translation.prop.SetValue(aditem, new Guid((byte[])sr.Properties[name][0]), null);
								break;
							case ADAttributeValueKind.ByteArrayValue:
								translation.prop.SetValue(aditem, (byte[])sr.Properties[name][0], null);
								break;
							case ADAttributeValueKind.StringValue:
								translation.prop.SetValue(aditem, sr.Properties[name][0] as string, null);
								break;
							case ADAttributeValueKind.StringValueLowerInvariant:
								translation.prop.SetValue(aditem, (sr.Properties[name][0] as string).ToLowerInvariant(), null);
								break;
							case ADAttributeValueKind.StringArrayValue:
								{
									List<string> list = new List<string>();
									foreach (string data in sr.Properties[name])
									{
										list.Add(data);
									}
									translation.prop.SetValue(aditem, list.ToArray(), null);
								}
								break;
							case ADAttributeValueKind.SecurityDescriptorValue:
								{
									var sd = new ActiveDirectorySecurity();
									sd.SetSecurityDescriptorBinaryForm((byte[])sr.Properties[name][0], AccessControlSections.Access);
									translation.prop.SetValue(aditem, sd, null);
								}
								break;
							case ADAttributeValueKind.ReplMetadataValue:
								translation.prop.SetValue(aditem, ConvertByteToMetaDataInfo((byte[])sr.Properties[name][0]), null);
								break;
							case ADAttributeValueKind.ReplMetadataValue2:
								{
									List<string> list = new List<string>();
									foreach (string data in sr.Properties[name])
									{
										list.Add(data);
									}
									translation.prop.SetValue(aditem, ConvertStringArrayToMetaDataInfo(list), null);
								}
								break;
							case ADAttributeValueKind.CertificateStore:
								{
									X509Certificate2Collection store = new X509Certificate2Collection();
									foreach (byte[] data in sr.Properties[name])
									{
										store.Add(new X509Certificate2(data));
									}
									translation.prop.SetValue(aditem, store, null);
								}
								break;
							case ADAttributeValueKind.ForestInfoValue:
								translation.prop.SetValue(aditem, ConvertByteToTrustInfo((byte[])sr.Properties[name][0]), null);
								break;
							case ADAttributeValueKind.SIDValue:
								translation.prop.SetValue(aditem, new SecurityIdentifier((byte[])sr.Properties[name][0], 0), null);
								break;
							case ADAttributeValueKind.SIDArrayValue:
								{
									List<SecurityIdentifier> list = new List<SecurityIdentifier>();
									foreach (byte[] data in sr.Properties[name])
									{
										list.Add(new SecurityIdentifier(data, 0));
									}
									translation.prop.SetValue(aditem, list.ToArray(), null);
								}
								break;
						}
					}
					catch (Exception)
					{
						Trace.WriteLine("Error when translation attribute " + name);
						throw;
					}
				}
				else
				{
					switch (name)
					{
						case "adspath":
							break;
						case "ntsecuritydescriptor":
							break;
						case "objectclass":
							break;
						default:
							Trace.WriteLine("Unknown attribute: " + name);
							break;
					}
				}
			}
			if (string.IsNullOrEmpty(aditem.DistinguishedName))
			{
				string path = (string) sr.Properties["adspath"][0];
				int i = path.IndexOf('/', 7);
				if (i > 0)
				{
					aditem.DistinguishedName = path.Substring(i + 1);
				}
			}
			return aditem;
		}
	}
}
