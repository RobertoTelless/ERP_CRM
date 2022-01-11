using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_CRM_Solution.ViewModels
{
    public class ProdutoBarcodeViewModel
    {
        public int PRBC_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PRODUTO obrigatorio")]
        public int PROD_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CÓDIGO DE BARRA obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O CÓDIGO DE BARRA deve ter no minimo 1 no máximo 50 caracteres.")]
        public string PRBC_NR_BARCODE { get; set; }
        public Nullable<int> PRBC_IN_ATIVO { get; set; }

        public virtual PRODUTO PRODUTO { get; set; }
    }
}