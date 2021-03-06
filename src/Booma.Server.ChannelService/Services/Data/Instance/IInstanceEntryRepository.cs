﻿using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;

namespace Booma
{
	/// <summary>
	/// <see cref="IGenericRepositoryCrudable{TKey,TModel}"/> for <see cref="InstanceEntry"/>.
	/// </summary>
	public interface IInstanceEntryRepository : IGenericRepositoryCrudable<int, InstanceEntry>, IEntireTableQueryable<InstanceEntry>
	{

	}
}
