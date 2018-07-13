using DasBlog.Core;

namespace DasBlog.Managers.Interfaces
{
	public interface ILoggingManager
	{
		void AddEvent(EventDataItem eventData);
	}
}
