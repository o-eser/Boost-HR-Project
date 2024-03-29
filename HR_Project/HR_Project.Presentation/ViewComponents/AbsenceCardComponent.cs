﻿
using System.Linq;
using HR_Project.Common.Models.DTOs;
using HR_Project.Common.Models.VMs;
using HR_Project.Domain.Enum;
using HR_Project.Presentation.APIService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace HR_Project.Presentation.ViewComponents
{
    public class AbsenceCardComponent : ViewComponent
    {
        private readonly IAPIService _apiService;

        public AbsenceCardComponent(IAPIService apiService)
        {
            _apiService = apiService;
        }

        //public async Task<IViewComponentResult> InvokeAsync()
        //{
        //    List<AbsenceVM> absences = await _apiService.GetAsync<List<AbsenceVM>>($"Absence/{ConditionType.Pending}", HttpContext.Request.Cookies["access-token"]);

        //    return await Task.FromResult(View(absences));
        //}

        public async Task<IViewComponentResult> InvokeAsync(bool ownList=true)
        {
            if (ownList)
            {
				List<AbsenceVM> pendingAbsences = await _apiService.GetAsync<List<AbsenceVM>>($"Absence/{ConditionType.Pending}", HttpContext.Request.Cookies["access-token"]);

				int pendingAbsenceCount = pendingAbsences.Count;

				return new ContentViewComponentResult(pendingAbsenceCount.ToString());
			}
			else
            {
				List<PersonnelsListDTO> pendingPersonnelAbsences = await _apiService.GetAsync<List<PersonnelsListDTO>>($"Absence/GetPendingAbsence", HttpContext.Request.Cookies["access-token"]);

                int pendingAbsenceCount = pendingPersonnelAbsences.Where(x=>x.Absences!=null).SelectMany(x=>x.Absences).Count();


                return new ContentViewComponentResult(pendingAbsenceCount.ToString());
			}
        }
    }
}
