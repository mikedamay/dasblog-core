using System;
using System.Xml.Serialization;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Core
{
	public class EventDataItem
	{
		public EventCodes EventCode { get; }
		public string UserMessage { get; }
		public string LoalUrl { get; }

		public EventDataItem( EventCodes eventCode, string userMessage, string localUrl )
		{
			EventCode = eventCode;
			UserMessage = userMessage;
			LoalUrl = localUrl;
		}

	}
}
