using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_CRM_Solution.ViewModels
{
    public class ProdutoKitViewModel
    {
        [Key]
        public int PRKI_CD_ID { get; set; }
        public int PROD_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PRODUTO obrigatorio")]
        public Nullable<int> PRKI_CD_KIT { get; set; }
        [Required(ErrorMessage = "Campo QUANTIDADE obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PRKI_IN_QUANTIDADE { get; set; }
        public Nullable<int> PRKI_IN_ATIVO { get; set; }

        public virtual PRODUTO PRODUTO { get; set; }
        public virtual PRODUTO PRODUTO1 { get; set; }

    }
}