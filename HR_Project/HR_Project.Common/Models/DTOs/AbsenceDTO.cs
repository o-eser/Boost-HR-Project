﻿using HR_Project.Common.Models.VMs;
using HR_Project.Common.ValidationClass;
using HR_Project.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR_Project.Common.Models.DTOs
{
    public class AbsenceDTO
    {
        [Display(Name = "Personel Id")]
        public Guid PersonnelId { get; set; }
        [Display(Name = "Açıklama")]
        [Required(ErrorMessage = "Açıklama alanı boş bırakılamaz.")]
        public string Reason { get; set; }
        [Display(Name = "İzin Türü")]
        [RequiredEnum(ErrorMessage ="Lütfen İzin Türünü giriniz.")]
        public LeaveTypes LeaveTypes { get; set; }
        [Display(Name = "Başlangıç Tarihi")]
        [DateValidaditon(ErrorMessage = "Başlangıç tarihi 1 aydan önce olamaz.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Display(Name = "Bitiş Tarihi")]
        [DateCompare("StartDate", ErrorMessage = "Bitiş tarihi başlangıç tarihinden önce olamaz.")]
		[DataType(DataType.Date)]
		public DateTime EndDate { get; set; }

        [Display(Name = "İzin Süresi")]
        [AbsenceDurationValidation("StartDate", "EndDate")]
        public double AbsenceDuration { get; set; }

        [Display(Name = "İzin Durumu")]
        public ConditionType Condition { get; set; }
    }
}
