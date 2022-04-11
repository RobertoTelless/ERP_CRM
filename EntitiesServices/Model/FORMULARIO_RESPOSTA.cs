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
    
    public partial class FORMULARIO_RESPOSTA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FORMULARIO_RESPOSTA()
        {
            this.FORMULARIO_RESPOSTA_ACAO = new HashSet<FORMULARIO_RESPOSTA_ACAO>();
            this.FORMULARIO_RESPOSTA_ANEXO = new HashSet<FORMULARIO_RESPOSTA_ANEXO>();
            this.FORMULARIO_RESPOSTA_COMENTARIO = new HashSet<FORMULARIO_RESPOSTA_COMENTARIO>();
        }
    
        public int FORE_CD_ID { get; set; }
        public Nullable<System.DateTime> FORE_DT_CADASTRO { get; set; }
        public string FORE_NM_NOME { get; set; }
        public string FORE_NM_EMAIL { get; set; }
        public string FORE_NR_CELULAR { get; set; }
        public Nullable<int> FORE_IN_MENSAGERIA { get; set; }
        public Nullable<int> FORE_IN_CRM { get; set; }
        public Nullable<int> FORE_IN_PATRIMONIO { get; set; }
        public Nullable<int> FORE_IN_ESTOQUE { get; set; }
        public Nullable<int> FORE_IN_COMPRA { get; set; }
        public Nullable<int> FORE_IN_VENDA { get; set; }
        public Nullable<int> FORE_IN_SERVICEDESK { get; set; }
        public Nullable<int> FORE_IN_FATURAMENTO { get; set; }
        public Nullable<int> FORE_IN_FINANCEIRO { get; set; }
        public Nullable<int> FORE_IN_EXPEDICAO { get; set; }
        public Nullable<int> FORE_IN_DOCUMENTO { get; set; }
        public string FORE_TX_MENSAGEM { get; set; }
        public Nullable<int> FORE_IN_ATIVO { get; set; }
        public Nullable<int> FORE_IN_STATUS { get; set; }
        public string FORE_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<System.DateTime> FORE_DT_CANCELAMENTO { get; set; }
        public string FORE_DS_JUSTIFICATIVA_CANCELAMENTO { get; set; }
        public Nullable<System.DateTime> FORE_DT_ENCERRAMENTO { get; set; }
        public string FORE_DS_MOTIVO_ENCERRAMENTO { get; set; }
        public Nullable<int> FORE_IN_ESTRELA { get; set; }
        public string FORE_NM_PROCESSO { get; set; }
        public Nullable<int> CROR_CD_ID { get; set; }
        public string FORE_DS_DESCRICAO { get; set; }
    
        public virtual UF UF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORMULARIO_RESPOSTA_ACAO> FORMULARIO_RESPOSTA_ACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORMULARIO_RESPOSTA_ANEXO> FORMULARIO_RESPOSTA_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORMULARIO_RESPOSTA_COMENTARIO> FORMULARIO_RESPOSTA_COMENTARIO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual CRM_ORIGEM CRM_ORIGEM { get; set; }
    }
}
