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
    
    public partial class CONTA_RECEBER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CONTA_RECEBER()
        {
            this.CONTA_RECEBER_ANEXO = new HashSet<CONTA_RECEBER_ANEXO>();
            this.CONTA_RECEBER_PARCELA = new HashSet<CONTA_RECEBER_PARCELA>();
            this.CONTA_RECEBER_RATEIO = new HashSet<CONTA_RECEBER_RATEIO>();
        }
    
        public int CARE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> CLIE_CD_ID { get; set; }
        public Nullable<int> PEVE_CD_ID { get; set; }
        public Nullable<int> COBA_CD_ID { get; set; }
        public Nullable<System.DateTime> CARE_DT_LANCAMENTO { get; set; }
        public decimal CARE_VL_VALOR { get; set; }
        public string CARE_DS_DESCRICAO { get; set; }
        public int CARE_IN_TIPO_LANCAMENTO { get; set; }
        public string CARE_NM_FAVORECIDO { get; set; }
        public string CARE_NM_FORMA_PAGAMENTO { get; set; }
        public int CARE_IN_LIQUIDADA { get; set; }
        public int CARE_IN_ATIVO { get; set; }
        public Nullable<System.DateTime> CARE_DT_DATA_LIQUIDACAO { get; set; }
        public Nullable<decimal> CARE_VL_VALOR_LIQUIDADO { get; set; }
        public Nullable<System.DateTime> CARE_DT_VENCIMENTO { get; set; }
        public Nullable<int> CARE_NR_ATRASO { get; set; }
        public Nullable<int> FOPA_CD_ID { get; set; }
        public string CARE_TX_OBSERVACOES { get; set; }
        public Nullable<int> CARE_IN_PARCELADA { get; set; }
        public Nullable<int> CARE_IN_PARCELAS { get; set; }
        public Nullable<System.DateTime> CARE_DT_INICIO_PARCELA { get; set; }
        public Nullable<int> PERI_CD_ID { get; set; }
        public Nullable<decimal> CARE_VL_PARCELADO { get; set; }
        public string CARE_NR_DOCUMENTO { get; set; }
        public Nullable<System.DateTime> CARE_DT_COMPETENCIA { get; set; }
        public Nullable<decimal> CARE_VL_DESCONTO { get; set; }
        public Nullable<decimal> CARE_VL_JUROS { get; set; }
        public Nullable<decimal> CARE_VL_TAXAS { get; set; }
        public Nullable<int> CECU_CD_ID { get; set; }
        public Nullable<decimal> CARE_VL_SALDO { get; set; }
        public Nullable<int> CARE_IN_PAGA_PARCIAL { get; set; }
        public Nullable<decimal> CARE_VL_PARCIAL { get; set; }
        public Nullable<decimal> CARE_VL_VALOR_RECEBIDO { get; set; }
        public Nullable<int> TITA_CD_ID { get; set; }
        public Nullable<System.DateTime> PEVE_DT_PREVISTA { get; set; }
        public Nullable<int> CARE_IN_ABERTOS { get; set; }
        public Nullable<int> CARE_IN_LIQUIDA_NORMAL { get; set; }
        public Nullable<int> CARE_IN_PARCIAL { get; set; }
        public Nullable<int> CRMC_CD_ID { get; set; }
        public Nullable<int> CRM1_CD_ID { get; set; }
        public Nullable<int> CRPR_CD_ID { get; set; }
        public Nullable<decimal> CARE_VL_TOTAL { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CENTRO_CUSTO CENTRO_CUSTO { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        public virtual CONTA_BANCO CONTA_BANCO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER_ANEXO> CONTA_RECEBER_ANEXO { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        public virtual FORMA_PAGAMENTO FORMA_PAGAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER_PARCELA> CONTA_RECEBER_PARCELA { get; set; }
        public virtual PERIODICIDADE PERIODICIDADE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER_RATEIO> CONTA_RECEBER_RATEIO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual CRM CRM { get; set; }
        public virtual CRM_PROPOSTA CRM_PROPOSTA { get; set; }
    }
}
