//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntitiesServices.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class TEMPLATE_EMAIL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TEMPLATE_EMAIL()
        {
            this.CRM_PROPOSTA = new HashSet<CRM_PROPOSTA>();
            this.MENSAGEM_AUTOMACAO = new HashSet<MENSAGEM_AUTOMACAO>();
            this.MENSAGENS = new HashSet<MENSAGENS>();
        }
    
        public int TEEM_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public string TEEM_NM_NOME { get; set; }
        public string TEEM_SG_SIGLA { get; set; }
        public string TEEM_LK_LINK { get; set; }
        public Nullable<int> TEEM_IN_ATIVO { get; set; }
        public string TEEM_TX_CABECALHO { get; set; }
        public string TEEM_TX_CORPO { get; set; }
        public string TEEM_TX_DADOS { get; set; }
        public string TEEM_AQ_ARQUIVO { get; set; }
        public string TEEM_TX_COMPLETO { get; set; }
        public Nullable<int> TEEM_IN_HTML { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PROPOSTA> CRM_PROPOSTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGEM_AUTOMACAO> MENSAGEM_AUTOMACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS> MENSAGENS { get; set; }
    }
}
