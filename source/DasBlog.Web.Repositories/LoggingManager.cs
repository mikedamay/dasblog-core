﻿using System.IO;
using AutoMapper;
using DasBlog.Core;
using DasBlog.Managers.Interfaces;
using newtelligence.DasBlog.Runtime;
using EventDataItem = DasBlog.Core.EventDataItem;

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
	}
}
