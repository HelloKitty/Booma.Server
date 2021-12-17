using System;
using System.Collections.Generic;
using System.Text;

namespace Booma.Tools.ItemPMT.JSON.Barriers
{
	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
	public class Sfx
	{
		public int sSFX { get; set; }
		public int sPitch { get; set; }
	}

	public class List
	{
		public string name { get; set; }
		public string desc { get; set; }
		public int stars { get; set; }
		public int announce { get; set; }
		public bool qreward { get; set; }
		public Sfx sfx { get; set; }
		public int id { get; set; }
		public int model { get; set; }
		public int texture { get; set; }
		public int teamPoints { get; set; }
		public int dfp { get; set; }
		public int evp { get; set; }
		public int blockParticle { get; set; }
		public int blockEffect { get; set; }
		public int _class { get; set; }
		public int level { get; set; }
		public int efr { get; set; }
		public int eth { get; set; }
		public int eic { get; set; }
		public int edk { get; set; }
		public int elt { get; set; }
		public int dfpRange { get; set; }
		public int evpRange { get; set; }
		public int statBoost { get; set; }
		public int techBoost { get; set; }
		public int unknown1 { get; set; }
		public int unknown2 { get; set; }
	}

	public class Barriers
	{
		public double sale { get; set; }
		public List<List> list { get; set; }
	}

	public class ItemPMTBarriers
	{
		public Barriers barriers { get; set; }
	}
}
