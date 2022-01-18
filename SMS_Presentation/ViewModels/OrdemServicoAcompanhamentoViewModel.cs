using EntitiesServices.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SystemBRPresentation.ViewModels
{
    public class OrdemServicoAcompanhamentoViewModel
    {
        public int ORSA_CD_ID { get; set; }
        public int ORSE_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ORSA_DT_ACOMPANHAMENTO { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O ACOMPANHAMENTO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string ORSA_DS_DESCRICAO { get; set; }
        public int ORSA_IN_ATIVO { get; set; }

        public virtual ORDEM_SERVICO ORDEM_SERVICO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}