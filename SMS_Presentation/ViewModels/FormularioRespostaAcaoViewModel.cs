using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_CRM_Solution.ViewModels
{
    public class FormularioRespostaAcaoViewModel
    {
        [Key]
        public int FRAC_CD_ID { get; set; }
        public int FORE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo USUÁRIO obrigatorio")]
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ATRIBUIÇÂO obrigatorio")]
        public Nullable<int> USUA_CD_ID2 { get; set; }
        [Required(ErrorMessage = "Campo TÍTULO obrigatorio")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "O TÍTULO deve conter no minimo 1 e no máximo 150 caracteres.")]
        public string FRAC_NM_TITULO { get; set; }
        [Required(ErrorMessage = "Campo DESCRIÇÂO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "A DESCRIÇÃO deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string FRAC_DS_DESCRICAO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÂO deve ser uma data válida")]
        public System.DateTime FRAC_DT_CRIACAO { get; set; }
        [Required(ErrorMessage = "Campo DATA PREVISTA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA PREVISTA deve ser uma data válida")]
        public Nullable<System.DateTime> FRAC_DT_PREVISTA { get; set; }
        public int FRAC_IN_STATUS { get; set; }
        public int FRAC_IN_ATIVO { get; set; }

        public virtual FORMULARIO_RESPOSTA FORMULARIO_RESPOSTA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }

    }
}