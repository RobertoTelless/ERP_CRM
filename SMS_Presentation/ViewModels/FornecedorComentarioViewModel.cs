using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace SMS_Solution.ViewModels
{
    public class FornecedorComentarioViewModel
    {
        [Key]
        public int FOCM_CD_ID { get; set; }
        public int FORN_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> FOCM_DT_COMENTARIO { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O COMENTÁRIO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string FOCM_DS_COMENTARIO { get; set; }
        public Nullable<int> FOCM_IN_ATIVO { get; set; }

        public virtual FORNECEDOR FORNECEDOR { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}