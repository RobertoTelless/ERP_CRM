using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_CRM_Solution.ViewModels
{
    public class ItemProcessoCRMViewModel
    {
        [Key]
        public int CRCI_CD_ID { get; set; }
        public int CRMC_CD_ID { get; set; }
        public int PROD_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo QUANTIDADE obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> CRCI_QN_QUANTIDADE { get; set; }
        public int CRCI_IN_ATIVO { get; set; }
        public string CRCI_TX_OBSERVACAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRCI_VL_VALOR { get; set; }
        public string CRCI_DS_JUSTIFICATIVA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CRCI_DT_JUSTIFICATIVA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRCI_VL_PRODUTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> CRCI_QN_QUANTIDADE_REVISADA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CRCI_DT_APROVACAO { get; set; }
        public Nullable<int> CRCI_IN_TESTE { get; set; }

        public virtual CRM_COMERCIAL CRM_COMERCIAL { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }

    }
}