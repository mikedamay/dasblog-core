using System;
using System.Collections.Generic;
using System.Text;

namespace CookComputing.XmlRpc
{
	public enum MappingAction {Ignore, Error}
    public class XmlRpcMissingMappingAttribute : Attribute
    {
		public string Description {get; set;}
		public XmlRpcMissingMappingAttribute(MappingAction action)
		{

		}
    }

	public class XmlRpcMethodAttribute : Attribute
	{
		public string Description {get; set;}
		public XmlRpcMethodAttribute(string something)
		{

		}
	}
	public class XmlRpcReturnValueAttribute : Attribute
	{
		public string Description {get; set;}
		public XmlRpcReturnValueAttribute()
		{
		}
	}
	public class XmlRpcMemberAttribute : Attribute
	{
		public string Description {get; set;}
		public XmlRpcMemberAttribute()
		{

		}
	}
	public class XmlRpcParameterAttribute : Attribute
	{
		public string Description {get; set;}
		public XmlRpcParameterAttribute()
		{

		}
	}
	public class XmlRpcServiceAttribute : Attribute
	{
		public string Description {get; set;}
		public string Name {get; set;}
		public XmlRpcServiceAttribute()
		{

		}
	}
}
