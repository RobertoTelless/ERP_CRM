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
    
    public partial class CRM_COMERCIAL_ACAO
    {
        public int CRCA_CD_ID { get; set; }
        public int CRMC_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID2 { get; set; }
        public int TIAC_CD_ID { get; set; }
        public System.DateTime CRCA_DT_ACAO { get; set; }
        public string CRCA_NM_TITULO { get; set; }
        public string CRCA_DS_DESCRICAO { get; set; }
        public System.DateTime CRCA_DT_PREVISTA { get; set; }
        public int CRCA_IN_STATUS { get; set; }
        public int CRCA_IN_ATIVO { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CRM_COMERCIAL CRM_COMERCIAL { get; set; }
        public virtual TIPO_ACAO TIPO_ACAO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
    }
}
