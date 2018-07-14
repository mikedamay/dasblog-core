using System;
using System.Collections.Generic;
using DasBlog.Core;

namespace DasBlog.Managers.Interfaces
{
	public interface ILoggingManager
	{
		void AddEvent(EventDataItem eventData);
		List<EventDataDisplayItem> GetEventsForDay(DateTime date);
	}
}
