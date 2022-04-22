using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_CRM_Solution.ViewModels
{
    public class CRMPropostaComentarioViewModel
    {
        [Key]
        public int PRAC_CD_ID { get; set; }
        public int CRPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÂO deve ser uma data válida")]
        public System.DateTime PRAC_DT_ACOMPANHAMENTO { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TEXTO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string PRAC_TX_ACOMPANHAMENTO { get; set; }
        public int PRAC_IN_ATIVO { get; set; }

        public virtual CRM_PROPOSTA CRM_PROPOSTA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}