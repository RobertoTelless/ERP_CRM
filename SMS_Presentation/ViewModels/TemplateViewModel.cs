using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_CRM_Solution.ViewModels
{
    public class TemplateViewModel
    {
        [Key]
        public int TEMP_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo SIGLA obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "A SIGLA deve conter no minimo 1 caracteres e no máximo 10.")]
        public string TEMP_SG_SIGLA { get; set; }
        public Nullable<int> TEMP_IN_TIPO { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string TEMP_NM_NOME { get; set; }
        public string TEMP_TX_CONTEUDO { get; set; }
        public string TEMP_AQ_ARQUIVO { get; set; }
        public int TEMP_IN_ATIVO { get; set; }
        public string TEMP_TX_CONTEUDO_LIMPO { get; set; }
        public string TEMP_TX_CABECALHO { get; set; }
        public string TEMP_TX_CORPO { get; set; }
        public string TEMP_TX_DADOS { get; set; }
        public Nullable<System.DateTime> TEMP_DT_CRIACAO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS> MENSAGENS { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
    }
}