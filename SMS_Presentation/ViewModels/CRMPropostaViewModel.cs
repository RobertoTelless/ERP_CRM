using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_CRM_Solution.ViewModels
{
    public class CRMPropostaViewModel
    {
        [Key]
        public int CRPR_CD_ID { get; set; }
        public int CRM1_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo USUÁRIO obrigatorio")]
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÂO deve ser uma data válida")]
        public System.DateTime CRPR_DT_PROPOSTA { get; set; }
        [Required(ErrorMessage = "Campo DATA DE VALIDADE obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE VALIDADE deve ser uma data válida")]
        public System.DateTime CRPR_DT_VALIDADE { get; set; }
        [StringLength(5000, ErrorMessage = "AS INFORMAÇÕES GERAIS deve conter no máximo 5000 caracteres.")]
        public string CRPR_DS_INFORMACOES { get; set; }
        public int CRPR_IN_STATUS { get; set; }
        public string CRPR_TX_TEXTO { get; set; }
        [StringLength(250, ErrorMessage = "O NOME DO ARQUIVO deve conter no máximo 250 caracteres.")]
        public string CRPR_AQ_ARQUIVO { get; set; }
        public Nullable<int> TEPR_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CANCELAMENTO deve ser uma data válida")]
        public Nullable<System.DateTime> CRPR_DT_CANCELAMENTO { get; set; }
        [StringLength(5000, ErrorMessage = "A JUSTIFICATIVA DE CANCELAMENTO deve conter no máximo 5000 caracteres.")]
        public string CRPR_DS_CANCELAMENTO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE REPROVAÇÂO deve ser uma data válida")]
        public Nullable<System.DateTime> CRPR_DT_REPROVACAO { get; set; }
        [StringLength(5000, ErrorMessage = "A DESCRIÇÃO DA REPROVAÇÃO deve conter no máximo 5000 caracteres.")]
        public string CRPR_DS_REPROVACAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE APROVAÇÃO deve ser uma data válida")]
        public Nullable<System.DateTime> CRPR_DT_APROVACAO { get; set; }
        [StringLength(5000, ErrorMessage = "A DESCRIÇÃO DA APROVAÇÂO deve conter no máximo 5000 caracteres.")]
        public string CRPR_DS_APROVACAO { get; set; }
        public int CRPR_IN_ATIVO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE ENVIO deve ser uma data válida")]
        public Nullable<System.DateTime> CRPR_DT_ENVIO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPR_VL_VALOR { get; set; }

        public virtual CRM CRM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PROPOSTA_ACOMPANHAMENTO> CRM_PROPOSTA_ACOMPANHAMENTO { get; set; }
        public virtual TEMPLATE_PROPOSTA TEMPLATE_PROPOSTA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }

    }
}