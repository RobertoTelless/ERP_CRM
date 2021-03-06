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
    
    public partial class CRM_COMERCIAL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CRM_COMERCIAL()
        {
            this.CRM_COMERCIAL_ACAO = new HashSet<CRM_COMERCIAL_ACAO>();
            this.CRM_COMERCIAL_ANEXO = new HashSet<CRM_COMERCIAL_ANEXO>();
            this.CRM_COMERCIAL_COMENTARIO_NOVA = new HashSet<CRM_COMERCIAL_COMENTARIO_NOVA>();
            this.CRM_COMERCIAL_CONTATO = new HashSet<CRM_COMERCIAL_CONTATO>();
            this.CRM_COMERCIAL_ITEM = new HashSet<CRM_COMERCIAL_ITEM>();
        }
    
        public int CRMC_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int CLIE_CD_ID { get; set; }
        public int TICR_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public int FILI_CD_ID { get; set; }
        public Nullable<int> CROR_CD_ID { get; set; }
        public Nullable<int> FOEN_CD_ID { get; set; }
        public Nullable<int> FOFR_CD_ID { get; set; }
        public int CRMC_IN_ATIVO { get; set; }
        public string CRMC_NR_NUMERO { get; set; }
        public string CRMC_NM_NOME { get; set; }
        public string CRMC_DS_DESCRICAO { get; set; }
        public string CRMC_DS_INFORMACOES_GERAIS { get; set; }
        public System.DateTime CRMC_DT_CRIACAO { get; set; }
        public System.DateTime CRMC_DT_VALIDADE { get; set; }
        public System.DateTime CRMC_DT_PREVISTA { get; set; }
        public int CRMC_IN_STATUS { get; set; }
        public int CRMC_IN_ESTRELA { get; set; }
        public Nullable<System.DateTime> CRMC_DT_VENCIMENTO { get; set; }
        public Nullable<System.DateTime> CRMC_DT_FATURAMENTO { get; set; }
        public System.DateTime CRMC_DT_TROCA_STATUS { get; set; }
        public Nullable<System.DateTime> CRMC_DT_APROVACAO { get; set; }
        public Nullable<System.DateTime> CRMC_DT_CANCELAMENTO { get; set; }
        public Nullable<int> MOCA_CD_ID { get; set; }
        public string CRMC_DS_JUSTIFICATIVA_CANCELAMENTO { get; set; }
        public Nullable<System.DateTime> CRMC_DT_ENCERRAMENTO { get; set; }
        public Nullable<int> MOEN_CD_ID { get; set; }
        public string CRMC_DS_JUSTIFICATIVA_ENCERRAMENTO { get; set; }
        public string CRMC_TX_OBSERVACAO { get; set; }
        public Nullable<int> CRMC_IN_DUMMY { get; set; }
        public string CRMC_AQ_IMAGEM { get; set; }
        public Nullable<System.DateTime> CRMC_DT_REPROVACAO { get; set; }
        public string CRMC_NR_NOTA_FISCAL { get; set; }
        public string CRMC_NM_CLIENTE { get; set; }
        public Nullable<System.DateTime> CRMC_DT_FINAL { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_COMERCIAL_ACAO> CRM_COMERCIAL_ACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_COMERCIAL_ANEXO> CRM_COMERCIAL_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_COMERCIAL_COMENTARIO_NOVA> CRM_COMERCIAL_COMENTARIO_NOVA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_COMERCIAL_CONTATO> CRM_COMERCIAL_CONTATO { get; set; }
        public virtual CRM_ORIGEM CRM_ORIGEM { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        public virtual FORMA_ENVIO FORMA_ENVIO { get; set; }
        public virtual FORMA_FRETE FORMA_FRETE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_COMERCIAL_ITEM> CRM_COMERCIAL_ITEM { get; set; }
        public virtual MOTIVO_CANCELAMENTO MOTIVO_CANCELAMENTO { get; set; }
        public virtual MOTIVO_ENCERRAMENTO MOTIVO_ENCERRAMENTO { get; set; }
        public virtual TIPO_CRM TIPO_CRM { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}
