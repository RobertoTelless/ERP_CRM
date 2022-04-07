using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_CRM_Solution.ViewModels
{
    public class FormularioRespostaComentarioViewModel
    {
        [Key]
        public int FRCO_CD_ID { get; set; }
        public int FORE_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÂO deve ser uma data válida")]
        public System.DateTime FRCO_DT_COMENTARIO { get; set; }
        [Required(ErrorMessage = "Campo TEXTO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string FRCO_DS_COMENTARIO { get; set; }
        public int FRCO_IN_ATIVO { get; set; }

        public virtual FORMULARIO_RESPOSTA FORMULARIO_RESPOSTA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}