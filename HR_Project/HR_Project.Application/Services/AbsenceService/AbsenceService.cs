﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HR_Project.Common.Models.DTOs;
using HR_Project.Common.Models.VMs;
using HR_Project.Domain.Entities.Concrete;
using HR_Project.Domain.Enum;
using HR_Project.Domain.Repositories;

namespace HR_Project.Application.Services.AbsenceService
{
	public class AbsenceService : IAbsenceService
    {
		private readonly IAbsenceRepository _absenceRepository;
		private readonly IMapper _mapper;

		public AbsenceService(IAbsenceRepository absenceRepository, IMapper mapper)
		{
            _absenceRepository = absenceRepository;
			_mapper = mapper;
		}

		public async Task Create(AbsenceDTO model)
		{
			Absence absence = _mapper.Map<Absence>(model);
            absence.Status = Status.Inserted;
            absence.CreatedDate = DateTime.Now;

			await _absenceRepository.Create(absence);
		}

		public async Task Delete(int id)
		{
			Absence absence = await _absenceRepository.GetDefault(x => x.Id == id);
			if (id == 0)
			{
				throw new ArgumentException("Id 0 Olamaz!");

			}
			else if (absence == null)
			{
				throw new ArgumentException("Böyle bir yazar mevcut değil!");
			}

            absence.DeletedDate = DateTime.Now;
            absence.Status = Status.Deleted;
			await _absenceRepository.Delete(absence);
		}

		public async Task<List<AbsenceVM>> GetAbsences()
		{
			return await _absenceRepository.GetFilteredList(x => new AbsenceVM
			{
                Reason = x.Reason,
                LeaveTypes = x.LeaveTypes,
				Condition = x.Condition,
                AbsenceDuration = x.AbsenceDuration,
			}, x => x.Status != Status.Deleted);
		}

		public async Task<List<AbsenceVM>> GetByCondition(ConditionType condition)
		{
			return await _absenceRepository.GetFilteredList(x => new AbsenceVM
            {
                Reason = x.Reason,
                LeaveTypes = x.LeaveTypes,
                Condition = x.Condition,
                AbsenceDuration = x.AbsenceDuration,
            }, x => x.Status != Status.Deleted && x.Condition == condition);
		}

		public async Task Update(UpdateAbsenceDTO model)
		{
            bool isExist = await _absenceRepository.Any(x => x.Id == model.Id);

            if (isExist)
            {
                var absence = _mapper.Map<Absence>(model);
				await _absenceRepository.Update(absence);
            }
        }
	}
}