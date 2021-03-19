using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glader.ASP.NameQuery;
using Glader.ASP.RPG;
using Glader.Essentials;
using Microsoft.Extensions.Logging;

namespace Booma
{
	/// <summary>
	/// Group implementation of <see cref="BaseNameQueryController{TObjectGuidType}"/>.
	/// </summary>
	[NameQueryRoute(nameof(EntityType.Group))]
	public class GroupEntityNameQueryController : BaseNameQueryController<NetworkEntityGuid>
	{
		private IRPGGroupRepository GroupRepository { get; }

		public GroupEntityNameQueryController(IClaimsPrincipalReader claimsReader, ILogger<AuthorizationReadyController> logger, IRPGGroupRepository groupRepository) 
			: base(claimsReader, logger)
		{
			GroupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
		}

		/// <inheritdoc />
		protected override async Task<ResponseModel<string, NameQueryResponseCode>> QueryEntityNameAsync(NetworkEntityGuid guid, CancellationToken token = default)
		{
			if (guid == null) throw new ArgumentNullException(nameof(guid));

			if (!await GroupRepository.ContainsAsync(guid.Identifier, token))
				return new EntityNameQueryResponse(NameQueryResponseCode.UnknownEntity);

			var group = await GroupRepository.RetrieveAsync(guid.Identifier, token);
			return new EntityNameQueryResponse(group.Name);
		}
	}
}
