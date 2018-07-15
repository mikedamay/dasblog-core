using System;
using System.Collections.Generic;
using System.IO;
using DasBlog.Core;
using DasBlog.Managers.Interfaces;
using newtelligence.DasBlog.Runtime;
using EventDataItem = DasBlog.Core.EventDataItem;
using EventCodes = DasBlog.Core.EventCodes;

namespace DasBlog.Managers
{
	public class LoggingManager : ILoggingManager
	{
		private ILoggingDataService _service;
		public LoggingManager(IDasBlogSettings dasBlogSettings)
		{
			string logDir = dasBlogSettings.SiteConfiguration.LogDir;
			string absoluteLogPath = Path.Combine(dasBlogSettings.WebRootDirectory, logDir.TrimStart('\\'));
			_service = LoggingDataServiceFactory.GetService(absoluteLogPath);
		}
		public void AddEvent(EventDataItem eventDataItem)
		{
			_service.AddEvent( new newtelligence.DasBlog.Runtime.EventDataItem(
			  (newtelligence.DasBlog.Runtime.EventCodes)(int)eventDataItem.EventCode
			  ,eventDataItem.UserMessage
			  ,eventDataItem.LoalUrl));
		}

		public List<EventDataDisplayItem> GetEventsForDay(DateTime date)
		{
			var eventDataDisplayItems = new List<EventDataDisplayItem>();
			var events = _service.GetEventsForDay(date);
			foreach (var edi in events)
			{
				eventDataDisplayItems.Add(new EventDataDisplayItem(
				  (EventCodes)edi.EventCode
				  ,edi.HtmlMessage, edi.EventTimeUtc));
			}

			return eventDataDisplayItems;
		}
	}
}
