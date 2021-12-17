using System;
using System.Collections.Generic;
using System.Text;

namespace Booma.Tools.ItemPMT.JSON.Units
{
	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
	public class List
	{
		public string name { get; set; }
		public string desc { get; set; }
		public int stars { get; set; }
		public int announce { get; set; }
		public bool qreward { get; set; }
		public int id { get; set; }
		public int model { get; set; }
		public int texture { get; set; }
		public int teamPoints { get; set; }
		public int stat { get; set; }
		public int amount { get; set; }
		public int mod { get; set; }
		public int reserved { get; set; }
	}

	public class Units
	{
		public double sale { get; set; }
		public List<List> list { get; set; }
	}

	public class ItemPMTUnits
	{
		public Units units { get; set; }
	}
}
