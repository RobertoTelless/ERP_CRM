using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_CRM_Solution.ViewModels
{
    public class CRMComercialDTOViewModel
    {
        public Int32 QUANTIDADE { get; set; }
        public String DESCRICAO { get; set; }
    }
}