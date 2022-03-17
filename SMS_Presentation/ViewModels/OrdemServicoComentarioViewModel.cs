using EntitiesServices.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERP_CRM_Solution.ViewModels
{
    public class OrdemServicoComentarioViewModel
    {
        public int ORSC_CD_ID { get; set; }
        public int ORSE_CD_ID { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O COMENTARIO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string ORSC_TX_COMENTARIO { get; set; }
        public System.DateTime ORSC_DT_CRIACAO { get; set; }
        public int USUA_CD_ID { get; set; }

        public virtual ORDEM_SERVICO ORDEM_SERVICO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}