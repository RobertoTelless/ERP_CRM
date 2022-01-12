using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_CRM_Solution.ViewModels
{
    public class MovimentoEntradaViewModel
    {
        public PRODUTO produto { get; set; }
        public MOVIMENTO_ESTOQUE_PRODUTO mvmtProduto { get; set; }
        public List<MOVIMENTO_ESTOQUE_PRODUTO> listaMvmtProduto { get; set; }
    }
}