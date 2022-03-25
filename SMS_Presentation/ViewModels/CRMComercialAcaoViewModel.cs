using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_CRM_Solution.ViewModels
{
    public class CRMComercialAcaoViewModel
    {
        [Key]
        public int CRCA_CD_ID { get; set; }
        public int CRMC_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo USUÁRIO obrigatorio")]
        public Nullable<int> USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ATRIBUIÇÂO obrigatorio")]
        public Nullable<int> USUA_CD_ID2 { get; set; }
        [Required(ErrorMessage = "Campo TIPO obrigatorio")]
        public int TIAC_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÂO deve ser uma data válida")]
        public System.DateTime CRCA_DT_ACAO { get; set; }
        [Required(ErrorMessage = "Campo TÍTULO obrigatorio")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "O TÍTULO deve conter no minimo 1 e no máximo 150 caracteres.")]
        public string CRCA_NM_TITULO { get; set; }
        [Required(ErrorMessage = "Campo DESCRIÇÂO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "A DESCRIÇÃO deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string CRCA_DS_DESCRICAO { get; set; }
        [Required(ErrorMessage = "Campo DATA PREVISTA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA PREVISTA deve ser uma data válida")]
        public System.DateTime CRCA_DT_PREVISTA { get; set; }
        public int CRCA_IN_STATUS { get; set; }
        public int CRCA_IN_ATIVO { get; set; }
        public Nullable<int> CRIA_AGENDA { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CRM_COMERCIAL CRM_COMERCIAL { get; set; }
        public virtual TIPO_ACAO TIPO_ACAO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }

    }
}