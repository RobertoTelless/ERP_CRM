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
    
    public partial class ATENDIMENTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ATENDIMENTO()
        {
            this.ATENDIMENTO_ACOMPANHAMENTO = new HashSet<ATENDIMENTO_ACOMPANHAMENTO>();
            this.ATENDIMENTO_AGENDA = new HashSet<ATENDIMENTO_AGENDA>();
            this.ATENDIMENTO_ANEXO = new HashSet<ATENDIMENTO_ANEXO>();
            this.ORDEM_SERVICO = new HashSet<ORDEM_SERVICO>();
            this.NOTIFICACAO = new HashSet<NOTIFICACAO>();
        }
    
        public int ATEN_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public int CAAT_CD_ID { get; set; }
        public Nullable<int> CLIE_CD_ID { get; set; }
        public Nullable<int> PROD_CD_ID { get; set; }
        public Nullable<int> PEVE_CD_ID { get; set; }
        public Nullable<int> DEPT_CD_ID { get; set; }
        public Nullable<int> SERV_CD_ID { get; set; }
        public Nullable<System.DateTime> ATEN_DT_INICIO { get; set; }
        public Nullable<System.DateTime> ATEN_DT_ENCERRAMENTO { get; set; }
        public string ATEN_DS_DESCRICAO { get; set; }
        public int ATEN_IN_STATUS { get; set; }
        public int ATEN_IN_ATIVO { get; set; }
        public string ATEN_DS_ENCERRAMENTO { get; set; }
        public Nullable<System.DateTime> ATEN_DT_CANCELAMENTO { get; set; }
        public string ATEN_DS_CANCELAMENTO { get; set; }
        public string ATEN_TX_OBSERVACOES { get; set; }
        public Nullable<System.TimeSpan> ATEN_HR_INICIO { get; set; }
        public Nullable<System.TimeSpan> ATEN_HR_CANCELAMENTO { get; set; }
        public Nullable<System.TimeSpan> ATEN_HR_ENCERRAMENTO { get; set; }
        public string ATEN_NM_ASSUNTO { get; set; }
        public Nullable<int> ATEN_IN_PRIORIDADE { get; set; }
        public Nullable<int> ATEN_IN_TIPO { get; set; }
        public Nullable<int> ATEN_IN_DESTINO { get; set; }
        public Nullable<System.DateTime> ATEN_DT_PREVISTA { get; set; }
        public Nullable<int> ATEN_IN_SLA { get; set; }
        public string ATEN_NR_NUMERO { get; set; }
        public Nullable<int> ATEN_IN_CRM { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATENDIMENTO_ACOMPANHAMENTO> ATENDIMENTO_ACOMPANHAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATENDIMENTO_AGENDA> ATENDIMENTO_AGENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATENDIMENTO_ANEXO> ATENDIMENTO_ANEXO { get; set; }
        public virtual CATEGORIA_ATENDIMENTO CATEGORIA_ATENDIMENTO { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        public virtual DEPARTAMENTO DEPARTAMENTO { get; set; }
        public virtual PEDIDO_VENDA PEDIDO_VENDA { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }
        public virtual SERVICO SERVICO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEM_SERVICO> ORDEM_SERVICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICACAO> NOTIFICACAO { get; set; }
    }
}
