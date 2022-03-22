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
    
    public partial class MENSAGENS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MENSAGENS()
        {
            this.CRM = new HashSet<CRM>();
            this.EMAIL_AGENDAMENTO = new HashSet<EMAIL_AGENDAMENTO>();
            this.MENSAGEM_ANEXO = new HashSet<MENSAGEM_ANEXO>();
            this.MENSAGENS_DESTINOS = new HashSet<MENSAGENS_DESTINOS>();
        }
    
        public int MENS_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<int> TEMP_CD_ID { get; set; }
        public Nullable<int> TSMS_CD_ID { get; set; }
        public Nullable<int> TEEM_CD_ID { get; set; }
        public Nullable<int> MENS_IN_ATIVO { get; set; }
        public Nullable<System.DateTime> MENS_DT_CRIACAO { get; set; }
        public string MENS_NM_CAMPANHA { get; set; }
        public string MENS_TX_TEXTO { get; set; }
        public Nullable<int> MENS_IN_TIPO { get; set; }
        public Nullable<System.DateTime> MENS_DT_AGENDAMENTO { get; set; }
        public Nullable<System.DateTime> MENS_DT_ENVIO { get; set; }
        public string MENS_TX_RETORNO { get; set; }
        public string MENS_NM_CABECALHO { get; set; }
        public string MENS_NM_RODAPE { get; set; }
        public string MENS_NM_LINK { get; set; }
        public string MENS_TX_SMS { get; set; }
        public Nullable<int> MENS_IN_CRM { get; set; }
        public string MENS_TX_TEXTO_LIMPO { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM> CRM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMAIL_AGENDAMENTO> EMAIL_AGENDAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGEM_ANEXO> MENSAGEM_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS_DESTINOS> MENSAGENS_DESTINOS { get; set; }
        public virtual TEMPLATE TEMPLATE { get; set; }
        public virtual TEMPLATE_EMAIL TEMPLATE_EMAIL { get; set; }
        public virtual TEMPLATE_SMS TEMPLATE_SMS { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}
