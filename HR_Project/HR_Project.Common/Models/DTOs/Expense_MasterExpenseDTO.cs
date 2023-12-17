﻿using HR_Project.Common.ValidationClass;
using HR_Project.Domain.Entities.Concrete;
using HR_Project.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HR_Project.Common.Models.DTOs
{
    public class Expense_MasterExpenseDTO
    {
        public int MasterExpenseId { get; set; }

        public int ExpenseId { get; set; }

        public double AggregateAmount { get; set; }

        [Display(Name = "Masraf Türü")]
        [RequiredEnum(ErrorMessage = "Lütfen masraf türünü giriniz.")]
        public ExpenseType ExpenseType { get; set; }

        [Display(Name = "Masraf Tarihi")]
        public DateTime ExpenseDate { get; set; }


        [Display(Name = "Masraf Miktarı")]
        [Required(ErrorMessage = "Tutar alanı boş bırakılamaz.")]
        public double ExpenseAmount { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Para Birimi")]
        [RequiredEnum(ErrorMessage = "Lütfen para birimini giriniz.")]
        public Currency Currency { get; set; }

        [Display(Name = "Açıklama")]
        [Required(ErrorMessage = "Açıklama alanı boş bırakılamaz.")]
        public string Reason { get; set; }

        [Display(Name = "Masraf Durumu")]
        public ConditionType Condition { get; set; }

    }
}
