using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_CRM_Solution.ViewModels
{
    public class TemplateEMailViewModel
    {
        [Key]
        public int TEEM_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50 caracteres.")]
        public string TEEM_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo SIGLA obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "A SIGLA deve conter no minimo 1 caracteres e no máximo 10 caracteres.")]
        public string TEEM_SG_SIGLA { get; set; }
        [StringLength(50, ErrorMessage = "O LINK deve conter máximo 50 caracteres.")]
        public string TEEM_LK_LINK { get; set; }
        public Nullable<int> TEEM_IN_ATIVO { get; set; }
        public string TEEM_TX_CABECALHO { get; set; }
        public string TEEM_TX_CORPO { get; set; }
        public string TEEM_TX_DADOS { get; set; }
        [StringLength(250, ErrorMessage = "O NOME DO ARQUIVO deve conter no máximo 250 caracteres.")]
        public string TEEM_AQ_ARQUIVO { get; set; }
        public Nullable<int> TEEM_IN_HTML { get; set; }

        public string file { get; set; }
        public Int32 html { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS> MENSAGENS { get; set; }

    }
}