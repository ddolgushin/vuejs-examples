using System;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RunTogether.ApplicationServices.Mapping.Dto;
using RunTogether.Dal.Model;

namespace RunTogether.ApplicationServices
{
	public class MarathonService
	{
		private string _connectionString;
		private IMapper _mapper;

		public MarathonService(string connectionString, IMapper mapper)
		{
			_connectionString = connectionString;
			_mapper = mapper;
		}

		public MarathonDto[] Get()
		{
			using (var context = new DatabaseContext(_connectionString))
			{
				var res = context.Marathons.OrderBy(o => o.Date).ToArray();
				
				return _mapper.Map<Mapping.Dto.MarathonDto[]>(res);
			}
		}

		public MarathonDto Get(int id)
		{
			using (var context = new DatabaseContext(_connectionString))
			{
				var res = context
					.Marathons
					.Include(o => o.WaypointInfos)
					.FirstOrDefault(o => o.Id == id);

				return res != null
					? _mapper.Map<Mapping.Dto.MarathonDto>(res)
					: null;
			}
		}

		public MarathonDto Create(int userId)
		{
			using (var context = new DatabaseContext(_connectionString))
			{
				var marathon = new Dal.Model.Marathon();

				context.Add(marathon);
				context.SaveChanges();

				marathon.MarathonLinks.Add(new Dal.Model.MarathonLink {
					MarathonId = marathon.Id,
					UserId = userId
				});

				context.Marathons.Update(marathon);
				context.SaveChanges();

				return _mapper.Map<Mapping.Dto.MarathonDto>(marathon);
			}
		}

		public void Update(MarathonDto marathon)
		{
			using (var context = new DatabaseContext(_connectionString))
			{
				var domainMarathon = _mapper.Map<Dal.Model.Marathon>(marathon);
				var res = context.Marathons.Update(domainMarathon);

				context.SaveChanges();
			}
		}

		public ParticipantModelDto[] GetParticipants(int marathonId)
		{
			using (var context = new DatabaseContext(_connectionString))
			{
				var res = context
					.Marathons
					.Include(o => o.MarathonLinks)
					.ThenInclude(o => o.User)
					.FirstOrDefault(o => o.Id == marathonId);

				return res.MarathonLinks
					.Select(o => _mapper.Map<Mapping.Dto.ParticipantModelDto>(o.User))
					.OrderBy(o => o.Name)
					.ToArray();
			}
		}

		public void AssignParticipant(int marathonId, int userId)
		{
			using (var context = new DatabaseContext(_connectionString))
			{
				var marathon = context
					.Marathons
					.Include(o => o.MarathonLinks)
					.FirstOrDefault(o => o.Id == marathonId);

				if (marathon.MarathonLinks.Any(o => o.UserId == userId))
					throw new Exception($"Пользователь {userId} уже является участником марафона {marathonId}");

				context.MarathonLinks.Add(new Dal.Model.MarathonLink
				{
					MarathonId = marathonId,
					UserId = userId,
					Role = Dal.Model.MarathonRole.Participant
				});
				context.SaveChanges();
			}
		}

		public void DeclineParticipant(int marathonId, int userId)
		{
			using (var context = new DatabaseContext(_connectionString))
			{
				var marathon = context
					.Marathons
					.Include(o => o.MarathonLinks)
					.FirstOrDefault(o => o.Id == marathonId);
				var link = marathon.MarathonLinks.FirstOrDefault(o => o.UserId == userId);

				if (link == null)
					throw new Exception($"Пользователь {userId} не является участником марафона {marathonId}");
				if (link.Role == Dal.Model.MarathonRole.Creator)
					throw new Exception($"Пользователь {userId} является создателем марафона {marathonId}, связь с ним не может быть удалена");

				context.MarathonLinks.Remove(link);
				context.SaveChanges();
			}
		}

		public bool OwnedBy(int marathonId, int userId)
		{
			using (var context = new DatabaseContext(_connectionString))
			{
				var res = context
					.Users
					.Include(o => o.MarathonLinks)
					.FirstOrDefault(o =>
						o.MarathonLinks.Any(p => p.UserId == userId
							&& p.MarathonId == marathonId
							&& p.Role == Dal.Model.MarathonRole.Creator));

				return res != null;
			}
		}
	}
}
