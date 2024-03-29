﻿using HR_Project.Domain.Entities.Abstract;
using HR_Project.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR_Project.Domain.Entities.Abstract;

namespace HR_Project.Domain.Entities.Concrete
{

	public class Advance : BaseEntity, IEntity<int>

	{
		public int Id { get; set; }
		public DateTime LastPaidDate { get; set; }
		public decimal Amount { get; set; }
		public string Reason { get; set; }
        public Currency Currency { get; set; }
        public ConditionType Condition { get; set; }

		public Guid PersonnelId { get; set; }
		public Personnel Personnel { get; set; }
    }
}
