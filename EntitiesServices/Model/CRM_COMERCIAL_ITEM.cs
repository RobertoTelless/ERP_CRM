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
    
    public partial class CRM_COMERCIAL_ITEM
    {
        public int CRCI_CD_ID { get; set; }
        public int CRMC_CD_ID { get; set; }
        public int PROD_CD_ID { get; set; }
        public Nullable<int> CRCI_QN_QUANTIDADE { get; set; }
        public int CRCI_IN_ATIVO { get; set; }
        public string CRCI_TX_OBSERVACAO { get; set; }
        public Nullable<decimal> CRCI_VL_VALOR { get; set; }
        public string CRCI_DS_JUSTIFICATIVA { get; set; }
        public Nullable<System.DateTime> CRCI_DT_JUSTIFICATIVA { get; set; }
        public Nullable<decimal> CRCI_VL_PRODUTO { get; set; }
        public Nullable<int> CRCI_QN_QUANTIDADE_REVISADA { get; set; }
        public Nullable<System.DateTime> CRCI_DT_APROVACAO { get; set; }
        public Nullable<int> CRCI_IN_TESTE { get; set; }
    
        public virtual CRM_COMERCIAL CRM_COMERCIAL { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }
    }
}
