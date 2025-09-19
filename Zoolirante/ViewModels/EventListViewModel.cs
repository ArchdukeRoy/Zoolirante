using System;
using Zoolirante.Models; 

namespace Zoolirante.ViewModels
{
	public class EventListViewModel
	{
		public DefaultViewModel DefaultVM { get; set; } = new DefaultViewModel();

		public List<Event> Events { get; set; } = new List<Event>();

		public List<EventRollCall> EventRollCalls { get; set; } = new List<EventRollCall>(); 

	}
}

