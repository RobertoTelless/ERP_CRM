using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERP_CRM_Solution.ViewModels
{
    public class ProdutoTabelaPrecoViewModel
    {
        public Int32 PROD_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FILIAL obrigatorio")]
        public Int32 FILI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CUSTO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Decimal? PROD_VL_CUSTO { get; set; }
        [Required(ErrorMessage = "Campo MARKUP obrigatorio")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Decimal? PROD_VL_MARKUP_PADRAO { get; set; }
        [Required(ErrorMessage = "Campo PRECO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Decimal? PRTP_VL_PRECO { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Decimal? PRTP_VL_PRECO_PROMOCAO { get; set; }
    }
}