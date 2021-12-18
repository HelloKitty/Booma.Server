using System;
using System.Collections.Generic;
using System.Text;

namespace Booma.Tools.PlayerStats.JSON
{
	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
	public class BaseStats
	{
		public int atp { get; set; }
		public int mst { get; set; }
		public int evp { get; set; }
		public int hp { get; set; }
		public int dfp { get; set; }
		public int ata { get; set; }
		public int lck { get; set; }
	}

	public class Level
	{
		public int atp { get; set; }
		public int mst { get; set; }
		public int evp { get; set; }
		public int hp { get; set; }
		public int dfp { get; set; }
		public int ata { get; set; }
		public int lck { get; set; }
		public int esp { get; set; }
		public int xp { get; set; }
	}

	public class ClassList
	{
		public object name { get; set; }
		public BaseStats baseStats { get; set; }
		public List<Level> levels { get; set; }
	}

	public class ItemPLT
	{
		public List<ClassList> classList { get; set; }
	}
}
