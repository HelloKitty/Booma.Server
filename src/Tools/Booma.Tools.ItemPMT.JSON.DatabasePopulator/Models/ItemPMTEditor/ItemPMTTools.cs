using System;
using System.Collections.Generic;
using System.Text;

namespace Booma.Tools.ItemPMT.JSON.Tools
{
	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
	public class List
	{
		public string name { get; set; }
		public string desc { get; set; }
		public int announce { get; set; }
		public bool qreward { get; set; }
		public int id { get; set; }
		public int model { get; set; }
		public int texture { get; set; }
		public int teamPoints { get; set; }
		public int amount { get; set; }
		public int tech { get; set; }
		public int cost { get; set; }
		public int use { get; set; }
	}

	public class Group
	{
		public List<List> list { get; set; }
	}

	public class Tools
	{
		public List<Group> group { get; set; }
	}

	public class ItemPMTTools
	{
		public Tools tools { get; set; }
	}
}
